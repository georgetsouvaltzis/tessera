using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tessera.Core.Terminal.Adapters.Internal;

internal sealed class UnixRawModeSession
{
    private const int Tcsanow = 0;
    private const int OpenReadWrite = 2;
    private const uint LinuxEcho = 0x00000008;
    private const uint LinuxICanon = 0x00000002;
    private const ulong DarwinEcho = 0x00000008;
    private const ulong DarwinICanon = 0x00000100;
    private const int LinuxVTime = 5;
    private const int LinuxVMin = 6;
    private const int DarwinVMin = 16;
    private const int DarwinVTime = 17;

    private string? _unixSttyState;
    private string? _unixRawModeProbe;
    private string? _unixRawModeError;
    private bool _unixRawModeEnabled;
    private int _unixTtyFd = -1;
    private bool _unixTermiosPrepared;
    private LinuxTermios _linuxOriginalTermios;
    private DarwinTermios _darwinOriginalTermios;

    public bool IsRawModeActive => _unixRawModeEnabled;

    public string RawModeDiagnostics => _unixRawModeProbe ?? "n/a";

    public string RawModeError => _unixRawModeError ?? "none";

    public void TryEnable(bool isInputInteractive)
    {
        if (!isInputInteractive)
        {
            _unixRawModeProbe = "input-not-interactive";
            _unixRawModeError = "input-not-interactive";
            return;
        }

        if (TryEnableWithTermios(out var termiosProbe, out var termiosError))
        {
            _unixRawModeProbe = termiosProbe;
            _unixRawModeError = termiosError ?? "none";
            _unixRawModeEnabled = true;
            return;
        }

        _unixSttyState = RunStty("-g", out var stateError);
        if (string.IsNullOrWhiteSpace(_unixSttyState))
        {
            _unixRawModeProbe = "stty-state-unavailable";
            _unixRawModeError = termiosError ?? stateError ?? "state-read-failed";
            return;
        }

        _ = RunStty("raw -echo", out var firstSetError);
        var probe = RunStty("-a", out var probeError);
        if (!IsRawProbeEnabled(probe))
        {
            _ = RunStty("-icanon min 1 time 0 -echo", out var fallbackSetError);
            probe = RunStty("-a", out probeError);
            _unixRawModeError = fallbackSetError ?? firstSetError;
        }
        else
        {
            _unixRawModeError = firstSetError;
        }

        _unixRawModeProbe = string.IsNullOrWhiteSpace(probe) ? "probe-unavailable" : probe;
        _unixRawModeEnabled = IsRawProbeEnabled(probe);
        if (!_unixRawModeEnabled && string.IsNullOrWhiteSpace(_unixRawModeError))
        {
            _unixRawModeError = probeError ?? "probe-check-failed";
        }
    }

    public void Restore()
    {
        if (_unixTermiosPrepared)
        {
            TryRestoreTermios();
        }

        if (!string.IsNullOrWhiteSpace(_unixSttyState))
        {
            _ = RunStty(_unixSttyState, out _);
            _unixSttyState = null;
        }

        _unixRawModeProbe = null;
        _unixRawModeError = null;
        _unixRawModeEnabled = false;
    }

    private bool TryEnableWithTermios(out string probe, out string? error)
    {
        probe = "termios-unavailable";
        error = null;

        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsLinux())
        {
            error = "termios-unsupported-os";
            return false;
        }

        _unixTtyFd = Open("/dev/tty", OpenReadWrite);
        if (_unixTtyFd < 0)
        {
            error = "termios-open-failed";
            _unixTtyFd = -1;
            return false;
        }

        if (OperatingSystem.IsMacOS())
        {
            if (TcGetAttrDarwin(_unixTtyFd, out _darwinOriginalTermios) != 0)
            {
                error = "termios-tcgetattr-failed";
                CloseUnixTty();
                return false;
            }

            var current = _darwinOriginalTermios;
            current.c_cc ??= new byte[20];
            current.c_lflag &= ~(DarwinEcho | DarwinICanon);
            current.c_cc[DarwinVMin] = 1;
            current.c_cc[DarwinVTime] = 0;

            if (TcSetAttrDarwin(_unixTtyFd, Tcsanow, ref current) != 0)
            {
                error = "termios-tcsetattr-failed";
                CloseUnixTty();
                return false;
            }

            if (TcGetAttrDarwin(_unixTtyFd, out var verify) != 0)
            {
                error = "termios-verify-failed";
                CloseUnixTty();
                return false;
            }

            var raw = (verify.c_lflag & (DarwinEcho | DarwinICanon)) == 0;
            probe = $"termios darwin lflag=0x{verify.c_lflag:X} raw={(raw ? "yes" : "no")}";
            if (!raw)
            {
                error = "termios-verify-not-raw";
                CloseUnixTty();
                return false;
            }

            _unixTermiosPrepared = true;
            return true;
        }

        if (TcGetAttrLinux(_unixTtyFd, out _linuxOriginalTermios) != 0)
        {
            error = "termios-tcgetattr-failed";
            CloseUnixTty();
            return false;
        }

        var linuxCurrent = _linuxOriginalTermios;
        linuxCurrent.c_cc ??= new byte[32];
        linuxCurrent.c_lflag &= ~(LinuxEcho | LinuxICanon);
        linuxCurrent.c_cc[LinuxVMin] = 1;
        linuxCurrent.c_cc[LinuxVTime] = 0;

        if (TcSetAttrLinux(_unixTtyFd, Tcsanow, ref linuxCurrent) != 0)
        {
            error = "termios-tcsetattr-failed";
            CloseUnixTty();
            return false;
        }

        if (TcGetAttrLinux(_unixTtyFd, out var verifyLinux) != 0)
        {
            error = "termios-verify-failed";
            CloseUnixTty();
            return false;
        }

        var linuxRaw = (verifyLinux.c_lflag & (LinuxEcho | LinuxICanon)) == 0;
        probe = $"termios linux lflag=0x{verifyLinux.c_lflag:X} raw={(linuxRaw ? "yes" : "no")}";
        if (!linuxRaw)
        {
            error = "termios-verify-not-raw";
            CloseUnixTty();
            return false;
        }

        _unixTermiosPrepared = true;
        return true;
    }

    private void TryRestoreTermios()
    {
        if (_unixTtyFd < 0)
        {
            _unixTermiosPrepared = false;
            return;
        }

        if (OperatingSystem.IsMacOS())
        {
            var restore = _darwinOriginalTermios;
            _ = TcSetAttrDarwin(_unixTtyFd, Tcsanow, ref restore);
        }
        else if (OperatingSystem.IsLinux())
        {
            var restore = _linuxOriginalTermios;
            _ = TcSetAttrLinux(_unixTtyFd, Tcsanow, ref restore);
        }

        CloseUnixTty();
        _unixTermiosPrepared = false;
    }

    private void CloseUnixTty()
    {
        if (_unixTtyFd >= 0)
        {
            _ = Close(_unixTtyFd);
            _unixTtyFd = -1;
        }
    }

    private static string? RunStty(string arguments, out string? error)
    {
        var sttyExecutable = ResolveSttyExecutable();
        var explicitTtyArgs = OperatingSystem.IsMacOS()
            ? new[] { "-f", "/dev/tty" }
            : new[] { "-F", "/dev/tty" };

        if (TryRunStty(sttyExecutable, arguments, explicitTtyArgs, out var output, out error))
        {
            return output.Trim();
        }

        if (TryRunStty(sttyExecutable, arguments, null, out output, out var fallbackError))
        {
            error = null;
            return output.Trim();
        }

        if (TryRunSttyWithShellRedirection(arguments, out output, out fallbackError))
        {
            error = null;
            return output.Trim();
        }

        error = fallbackError ?? error;
        return null;
    }

    private static bool IsRawProbeEnabled(string? probe)
    {
        if (string.IsNullOrWhiteSpace(probe))
        {
            return false;
        }

        var normalized = probe
            .Replace(";", " ", StringComparison.Ordinal)
            .Replace(":", " ", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal);
        var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var hasNoEcho = tokens.Contains("-echo", StringComparer.Ordinal) && !tokens.Contains("echo", StringComparer.Ordinal);
        var hasNonCanonical = tokens.Contains("-icanon", StringComparer.Ordinal)
            || tokens.Contains("raw", StringComparer.Ordinal)
            || tokens.Contains("cbreak", StringComparer.Ordinal)
            || (normalized.Contains("min = 1", StringComparison.Ordinal) && normalized.Contains("time = 0", StringComparison.Ordinal));

        return hasNoEcho && hasNonCanonical;
    }

    private static bool TryRunStty(string sttyExecutable, string arguments, string[]? explicitTtyArgs, out string output, out string? error)
    {
        output = string.Empty;
        error = null;

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = sttyExecutable,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            };

            if (explicitTtyArgs is not null)
            {
                foreach (var arg in explicitTtyArgs)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }
            }

            foreach (var arg in arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                process.StartInfo.ArgumentList.Add(arg);
            }

            if (!process.Start())
            {
                error = "stty-start-failed";
                return false;
            }

            output = process.StandardOutput.ReadToEnd();
            var stdErr = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                error = string.IsNullOrWhiteSpace(stdErr)
                    ? $"stty-exit-{process.ExitCode}"
                    : $"stty-exit-{process.ExitCode}: {stdErr}";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            error = $"stty-exception: {ex.Message}";
            return false;
        }
    }

    private static bool TryRunSttyWithShellRedirection(string arguments, out string output, out string? error)
    {
        output = string.Empty;
        error = null;

        try
        {
            var escaped = arguments.Replace("'", "'\"'\"'", StringComparison.Ordinal);
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    ArgumentList = { "-lc", $"stty {escaped} < /dev/tty" },
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            };

            if (!process.Start())
            {
                error = "stty-shell-start-failed";
                return false;
            }

            output = process.StandardOutput.ReadToEnd();
            var stdErr = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                error = string.IsNullOrWhiteSpace(stdErr)
                    ? $"stty-shell-exit-{process.ExitCode}"
                    : $"stty-shell-exit-{process.ExitCode}: {stdErr}";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            error = $"stty-shell-exception: {ex.Message}";
            return false;
        }
    }

    private static string ResolveSttyExecutable()
    {
        if (File.Exists("/bin/stty"))
        {
            return "/bin/stty";
        }

        if (File.Exists("/usr/bin/stty"))
        {
            return "/usr/bin/stty";
        }

        return "stty";
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LinuxTermios
    {
        public uint c_iflag;
        public uint c_oflag;
        public uint c_cflag;
        public uint c_lflag;
        public byte c_line;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] c_cc;
        public uint c_ispeed;
        public uint c_ospeed;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DarwinTermios
    {
        public ulong c_iflag;
        public ulong c_oflag;
        public ulong c_cflag;
        public ulong c_lflag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] c_cc;
        public ulong c_ispeed;
        public ulong c_ospeed;
    }

    [SuppressMessage(
        "Interoperability",
        "CA2101:Specify marshaling for P/Invoke string arguments",
        Justification = "libc open consumes a POSIX path on Unix platforms; analyzer-compliant LibraryImport alternatives require unsafe code, which Tessera forbids.")]
    [DllImport("libc", EntryPoint = "open", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern int Open(string path, int flags);

    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int Close(int fd);

    [DllImport("libc", EntryPoint = "tcgetattr", SetLastError = true)]
    private static extern int TcGetAttrLinux(int fd, out LinuxTermios termios);

    [DllImport("libc", EntryPoint = "tcsetattr", SetLastError = true)]
    private static extern int TcSetAttrLinux(int fd, int optionalActions, ref LinuxTermios termios);

    [DllImport("libc", EntryPoint = "tcgetattr", SetLastError = true)]
    private static extern int TcGetAttrDarwin(int fd, out DarwinTermios termios);

    [DllImport("libc", EntryPoint = "tcsetattr", SetLastError = true)]
    private static extern int TcSetAttrDarwin(int fd, int optionalActions, ref DarwinTermios termios);
}
