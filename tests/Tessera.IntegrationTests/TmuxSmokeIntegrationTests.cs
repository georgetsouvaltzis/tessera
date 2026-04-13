using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Tessera.IntegrationTests;

[TestFixture]
public sealed class TmuxSmokeIntegrationTests
{
    private static readonly string RepoRoot = ResolveRepoRoot();
    private static readonly string FixtureProjectPath = Path.Combine("tests", "Tessera.IntegrationFixtureApp");
    private const string FixtureProcessName = "Tessera.IntegrationFixtureApp";
    private static readonly Regex CsiRegex = new(@"\x1B\[[0-?]*[ -/]*[@-~]", RegexOptions.Compiled);
    private static readonly Regex OscRegex = new(@"\x1B\][^\x07]*(\x07|\x1B\\)", RegexOptions.Compiled);

    [Test]
    public async Task TmuxSmokeArrowKeysUpdateCounterAndQQuits()
    {
        if (!CommandSucceeds("tmux", "-V"))
        {
            Assert.Ignore("tmux is not available in this environment.");
            return;
        }

        var session = $"tessera_smoke_{Guid.NewGuid():N}".Substring(0, 24);
        try
        {
            RunChecked("tmux", $"new-session -d -s {session} /bin/zsh -lc \"{BuildAppRunCommand()}\"");
            var boot = await WaitForPaneContains(session, "Count: 0", TimeSpan.FromSeconds(5));
            StringAssert.Contains("Counter", boot, "App should render counter on boot.");

            SendKeys(session, "Up");
            var incremented = await WaitForPaneContains(session, "Count: 1", TimeSpan.FromSeconds(3));
            StringAssert.Contains("Count: 1", incremented, "Up key should increment the counter.");

            SendKeys(session, "Down");
            var decremented = await WaitForPaneContains(session, "Count: 0", TimeSpan.FromSeconds(3));
            StringAssert.Contains("Count: 0", decremented, "Down key should decrement the counter.");

            Assert.That(PaneHasExamplesChildProcess(session), Is.True,
                "App process should be active before quit check.");

            SendKeys(session, "q");
            var quitObserved = await WaitForCondition(
                () => !PaneHasExamplesChildProcess(session),
                TimeSpan.FromSeconds(3));
            Assert.That(
                quitObserved,
                Is.True,
                $"Single 'q' should terminate example process without extra keys.\nlast pane:\n{CapturePane(session)}");
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

        var session = $"tessera_showcase_{Guid.NewGuid():N}".Substring(0, 24);
        try
        {
            RunChecked("tmux", $"new-session -d -s {session} /bin/zsh -lc \"{BuildAppRunCommand()}\"");
            _ = await WaitForPaneContains(session, "Count: 0", TimeSpan.FromSeconds(5));

            SendKeys(session, "Up");
            SendKeys(session, "Up");
            SendKeys(session, "Up");
            var afterUps = await WaitForPaneContains(session, "Count: 3", TimeSpan.FromSeconds(3));
            StringAssert.Contains("Count: 3", afterUps, "Multiple Up keys should accumulate.");

            SendKeys(session, "Down");
            var afterDown = await WaitForPaneContains(session, "Count: 2", TimeSpan.FromSeconds(3));
            StringAssert.Contains("Count: 2", afterDown, "Down key should decrement from accumulated count.");

            SendKeys(session, "q");
            var quitObserved = await WaitForCondition(
                () => !PaneHasExamplesChildProcess(session),
                TimeSpan.FromSeconds(3));
            Assert.That(
                quitObserved,
                Is.True,
                $"Single 'q' should terminate app.\nlast pane:\n{CapturePane(session)}");
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
        var fromAlternate = RunBestEffort("tmux", $"capture-pane -ap -t {session} -S -220");
        if (fromAlternate.ExitCode == 0 && !string.IsNullOrWhiteSpace(fromAlternate.StdOut))
        {
            return NormalizePaneText(fromAlternate.StdOut);
        }

        var fallback = RunChecked("tmux", $"capture-pane -pt {session} -S -220");
        return NormalizePaneText(fallback.StdOut);
    }

    private static string NormalizePaneText(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var withoutOsc = OscRegex.Replace(value, string.Empty);
        var withoutCsi = CsiRegex.Replace(withoutOsc, string.Empty);
        return withoutCsi.Replace("\r", string.Empty, StringComparison.Ordinal);
    }

    private static async Task<string> WaitForPaneContains(string session, string expected, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        var last = string.Empty;
        while (DateTime.UtcNow < deadline)
        {
            if (!SessionExists(session))
            {
                break;
            }

            last = CapturePane(session);
            if (last.Contains(expected, StringComparison.Ordinal))
            {
                return last;
            }

            await Task.Delay(80);
        }

        throw new AssertionException($"Timed out waiting for pane text '{expected}'.\nlast pane:\n{last}");
    }

    private static async Task<bool> WaitForCondition(Func<bool> predicate, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (predicate())
            {
                return true;
            }

            await Task.Delay(60);
        }

        return false;
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
            WorkingDirectory = RepoRoot
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
        var command =
            $"cd {QuoteForShell(RepoRoot)} && dotnet run --project {QuoteForShell(FixtureProjectPath)} --no-build; exec /bin/zsh -i";
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
            if (File.Exists(Path.Combine(current.FullName, "Tessera.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate Tessera.slnx from the integration test output directory.");
    }

    private sealed record CommandResult(int ExitCode, string StdOut, string StdErr);
}
