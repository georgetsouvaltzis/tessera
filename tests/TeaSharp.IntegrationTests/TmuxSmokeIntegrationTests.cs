using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class TmuxSmokeIntegrationTests
{
    private static readonly string RepoRoot = ResolveRepoRoot();
    private static readonly string FixtureProjectPath = Path.Combine("tests", "TeaSharp.IntegrationFixtureApp");
    private const string FixtureProcessName = "TeaSharp.IntegrationFixtureApp";

    [Test]
    public async Task TmuxSmokeArrowKeysUpdateCounterAndQQuits()
    {
        if (!CommandSucceeds("tmux", "-V"))
        {
            Assert.Ignore("tmux is not available in this environment.");
            return;
        }

        var session = $"teasharp_smoke_{Guid.NewGuid():N}".Substring(0, 24);
        try
        {
            RunChecked("tmux", $"new-session -d -s {session} /bin/zsh -lc \"{BuildAppRunCommand()}\"");
            await Task.Delay(1600);

            var boot = CapturePane(session);
            StringAssert.Contains("Counter", boot, "App should render counter on boot.");
            StringAssert.Contains("Count: 0", boot, "Counter should start at zero.");

            SendKeys(session, "Up");
            await Task.Delay(240);
            var incremented = CapturePane(session);
            StringAssert.Contains("Count: 1", incremented, "Up key should increment the counter.");

            SendKeys(session, "Down");
            await Task.Delay(240);
            var decremented = CapturePane(session);
            StringAssert.Contains("Count: 0", decremented, "Down key should decrement the counter.");

            Assert.That(PaneHasExamplesChildProcess(session), Is.True, "App process should be active before quit check.");

            SendKeys(session, "q");
            await Task.Delay(600);
            Assert.That(PaneHasExamplesChildProcess(session), Is.False, "Single 'q' should terminate example process without extra keys.");
        }
        finally
        {
            _ = RunBestEffort("tmux", $"kill-session -t {session}");
        }
    }

    [Test]
    public async Task TmuxSmokeRepeatedArrowKeysAccumulateChanges()
    {
        if (!CommandSucceeds("tmux", "-V"))
        {
            Assert.Ignore("tmux is not available in this environment.");
            return;
        }

        var session = $"teasharp_showcase_{Guid.NewGuid():N}".Substring(0, 24);
        try
        {
            RunChecked("tmux", $"new-session -d -s {session} /bin/zsh -lc \"{BuildAppRunCommand()}\"");
            await Task.Delay(1600);

            SendKeys(session, "Up");
            SendKeys(session, "Up");
            SendKeys(session, "Up");
            await Task.Delay(400);
            var afterUps = CapturePane(session);
            StringAssert.Contains("Count: 3", afterUps, "Multiple Up keys should accumulate.");

            SendKeys(session, "Down");
            await Task.Delay(240);
            var afterDown = CapturePane(session);
            StringAssert.Contains("Count: 2", afterDown, "Down key should decrement from accumulated count.");

            SendKeys(session, "q");
            await Task.Delay(600);
            Assert.That(PaneHasExamplesChildProcess(session), Is.False, "Single 'q' should terminate app.");
        }
        finally
        {
            _ = RunBestEffort("tmux", $"kill-session -t {session}");
        }
    }

    private static void SendKeys(string session, string keys)
    {
        RunChecked("tmux", $"send-keys -t {session} {keys}");
    }

    private static string CapturePane(string session)
    {
        var result = RunChecked("tmux", $"capture-pane -pt {session} -S -220");
        return result.StdOut;
    }

    private static int PanePid(string session)
    {
        var result = RunChecked("tmux", $"display-message -p -t {session} '#{{pane_pid}}'");
        var raw = result.StdOut.Trim().Trim('\'', '"');
        if (int.TryParse(raw, out var panePid))
        {
            return panePid;
        }

        throw new InvalidOperationException($"Unexpected tmux pane_pid value: '{result.StdOut}'.");
    }

    private static bool PaneHasExamplesChildProcess(string session)
    {
        var panePid = PanePid(session);
        return CommandSucceeds("pgrep", $"-P {panePid} -f {FixtureProcessName}");
    }

    private static bool SessionExists(string session)
    {
        return CommandSucceeds("tmux", $"has-session -t {session}");
    }

    private static bool CommandSucceeds(string fileName, string arguments)
    {
        var result = RunBestEffort(fileName, arguments);
        return result.ExitCode == 0;
    }

    private static CommandResult RunChecked(string fileName, string arguments)
    {
        var result = RunBestEffort(fileName, arguments);
        if (result.ExitCode == 0)
        {
            return result;
        }

        throw new InvalidOperationException(
            $"Command failed: {fileName} {arguments}\nstdout:\n{result.StdOut}\nstderr:\n{result.StdErr}");
    }

    private static CommandResult RunBestEffort(string fileName, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = RepoRoot,
        };

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Failed to start process: {fileName}");
        var stdOut = process.StandardOutput.ReadToEnd();
        var stdErr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new CommandResult(process.ExitCode, stdOut, stdErr);
    }

    private static string BuildAppRunCommand()
    {
        var command = $"cd {QuoteForShell(RepoRoot)} && dotnet run --project {QuoteForShell(FixtureProjectPath)} --no-build; exec /bin/zsh -i";
        return command.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private static string QuoteForShell(string value)
    {
        return value.Contains(' ', StringComparison.Ordinal)
            ? $"\"{value}\""
            : value;
    }

    private static string ResolveRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "TeaSharp.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate TeaSharp.slnx from the integration test output directory.");
    }

    private sealed record CommandResult(int ExitCode, string StdOut, string StdErr);
}
