using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class TmuxSmokeIntegrationTests
{
    private const string RepoRoot = "/Users/georgetsouvaltzis/Projects/playground/teasharp";
    private const string AppRunCommand = "cd /Users/georgetsouvaltzis/Projects/playground/teasharp && dotnet run --project examples/TeaSharp.Examples --no-build; exec /bin/zsh -i";

    [Test]
    public async Task TmuxSmoke_CommandModeAndSingleQQuit_WorkAsExpected()
    {
        if (!CommandSucceeds("tmux", "-V"))
        {
            Assert.Ignore("tmux is not available in this environment.");
            return;
        }

        var session = $"teasharp_smoke_{Guid.NewGuid():N}".Substring(0, 24);
        try
        {
            RunChecked("tmux", $"new-session -d -s {session} /bin/zsh -lc \"{AppRunCommand}\"");
            await Task.Delay(1600);

            var boot = CapturePane(session);
            StringAssert.Contains("TeaSharp Dashboard", boot, "App should render dashboard on boot.");

            SendKeys(session, "3");
            await Task.Delay(240);
            var showcase = CapturePane(session);
            StringAssert.Contains("page=showcase", showcase, "Key '3' should switch to showcase page.");

            SendKeys(session, "Tab");
            SendKeys(session, ":");
            await Task.Delay(240);
            var cmd = CapturePane(session);
            StringAssert.Contains("focus=command", cmd, "Single ':' should focus command input.");
            StringAssert.Contains("mode=cmd", cmd, "Single ':' should enable command mode.");

            SendKeys(session, "C-[");
            await Task.Delay(240);
            var nav = CapturePane(session);
            StringAssert.Contains("mode=nav", nav, "Esc should exit command mode.");

            Assert.That(PaneHasExamplesChildProcess(session), Is.True, "App process should be active before quit check.");

            SendKeys(session, ":");
            await Task.Delay(240);
            var cmdAgain = CapturePane(session);
            StringAssert.Contains("mode=cmd", cmdAgain, "Colon should re-enter command mode before q-input check.");

            SendKeys(session, "q");
            await Task.Delay(360);
            Assert.That(
                PaneHasExamplesChildProcess(session),
                Is.True,
                "Plain 'q' in command mode should stay in app and not trigger global quit.");
            var cmdWithQ = CapturePane(session);
            StringAssert.Contains("mode=cmd", cmdWithQ, "Command mode should remain active after plain 'q'.");
            StringAssert.Contains("Command * [CMD]", cmdWithQ, "Command pane should stay focused after plain 'q'.");

            SendKeys(session, "C-[");
            await Task.Delay(240);
            var navAgain = CapturePane(session);
            StringAssert.Contains("mode=nav", navAgain, "Esc should return to navigation mode before quit key.");

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
    public async Task TmuxSmoke_ShowcaseHotkeysAndPaneCycling_WorkAsExpected()
    {
        if (!CommandSucceeds("tmux", "-V"))
        {
            Assert.Ignore("tmux is not available in this environment.");
            return;
        }

        var session = $"teasharp_showcase_{Guid.NewGuid():N}".Substring(0, 24);
        try
        {
            RunChecked("tmux", $"new-session -d -s {session} /bin/zsh -lc \"{AppRunCommand}\"");
            await Task.Delay(1600);

            SendKeys(session, "3");
            await Task.Delay(240);
            var showcase = CapturePane(session);
            StringAssert.Contains("page=showcase", showcase, "Showcase page should render after key '3'.");

            SendKeys(session, "Tab");
            SendKeys(session, "t");
            SendKeys(session, "m");
            SendKeys(session, "p");
            SendKeys(session, "P");
            await Task.Delay(400);
            var afterHotkeys = CapturePane(session);

            StringAssert.Contains("mode=nav", afterHotkeys, "Showcase hotkeys should keep navigation mode active.");
            StringAssert.Contains("pane=", afterHotkeys, "Pane label should remain visible after cycling hotkeys.");
            StringAssert.Contains("page=showcase", afterHotkeys, "App should remain on showcase page after hotkeys.");
            Assert.That(PaneHasExamplesChildProcess(session), Is.True, "App process should remain alive after showcase hotkeys.");

            SendKeys(session, "q");
            await Task.Delay(600);
            Assert.That(PaneHasExamplesChildProcess(session), Is.False, "Single 'q' from navigation mode should terminate app.");
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
        return CommandSucceeds("pgrep", $"-P {panePid} -f TeaSharp.Examples");
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

    private sealed record CommandResult(int ExitCode, string StdOut, string StdErr);
}
