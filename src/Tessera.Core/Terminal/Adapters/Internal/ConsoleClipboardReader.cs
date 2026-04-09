using System.Diagnostics;

namespace Tessera.Core.Terminal.Adapters.Internal;

internal static class ConsoleClipboardReader
{
    public static bool IsPasteShortcut(ConsoleKeyInfo key)
    {
        var isCtrlV = key.Key == ConsoleKey.V && (key.Modifiers & ConsoleModifiers.Control) != 0;
        var isShiftInsert = key.Key == ConsoleKey.Insert && (key.Modifiers & ConsoleModifiers.Shift) != 0;
        return isCtrlV || isShiftInsert;
    }

    public static bool TryReadText(out string text)
    {
        if (OperatingSystem.IsMacOS())
        {
            return TryRunClipboardProcess("/usr/bin/pbpaste", [], out text)
                || TryRunClipboardProcess("pbpaste", [], out text);
        }

        if (OperatingSystem.IsWindows())
        {
            return TryRunClipboardProcess("powershell", ["-NoProfile", "-Command", "Get-Clipboard -Raw"], out text)
                || TryRunClipboardProcess("pwsh", ["-NoProfile", "-Command", "Get-Clipboard -Raw"], out text);
        }

        return TryRunClipboardProcess("wl-paste", ["-n"], out text)
            || TryRunClipboardProcess("xclip", ["-selection", "clipboard", "-o"], out text)
            || TryRunClipboardProcess("xsel", ["--clipboard", "--output"], out text);
    }

    private static bool TryRunClipboardProcess(string fileName, string[] args, out string text)
    {
        text = string.Empty;

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            };

            foreach (var arg in args)
            {
                process.StartInfo.ArgumentList.Add(arg);
            }

            if (!process.Start())
            {
                return false;
            }

            if (!process.WaitForExit(350))
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Clipboard helper already timed out; failed kill is safe to ignore.
                }

                return false;
            }

            if (process.ExitCode != 0)
            {
                return false;
            }

            text = process.StandardOutput.ReadToEnd();
            return !string.IsNullOrEmpty(text);
        }
        catch
        {
            // Clipboard helpers are optional and platform-dependent.
            return false;
        }
    }
}
