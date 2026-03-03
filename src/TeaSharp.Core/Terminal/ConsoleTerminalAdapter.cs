using System.Runtime.InteropServices;
using System.Diagnostics;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Terminal;

public sealed class ConsoleTerminalAdapter : ITerminalAdapter
{
    private const int StdInputHandle = -10;
    private const int StdOutputHandle = -11;
    private const uint EnableProcessedInput = 0x0001;
    private const uint EnableLineInput = 0x0002;
    private const uint EnableEchoInput = 0x0004;
    private const uint EnableVirtualTerminalProcessing = 0x0004;
    private const uint DisableNewlineAutoReturn = 0x0008;
    private const uint EnableVirtualTerminalInput = 0x0200;

    private readonly bool _treatControlAsInputOriginal;
    private readonly bool _ownsInput;
    private readonly bool _ownsOutput;
    private uint _originalInputMode;
    private uint _originalOutputMode;
    private string? _unixSttyState;
    private bool _prepared;

    public ConsoleTerminalAdapter()
    {
        var stdIn = Console.OpenStandardInput();
        var stdOut = Console.OpenStandardOutput();
        var inputInteractive = !Console.IsInputRedirected;
        var outputInteractive = !Console.IsOutputRedirected;

        if (!OperatingSystem.IsWindows() && Console.IsInputRedirected && TryOpenTty(FileAccess.ReadWrite, out var ttyIn))
        {
            stdIn = ttyIn;
            inputInteractive = true;
            _ownsInput = true;
        }

        if (!OperatingSystem.IsWindows() && Console.IsOutputRedirected && TryOpenTty(FileAccess.Write, out var ttyOut))
        {
            stdOut = ttyOut;
            outputInteractive = true;
            _ownsOutput = true;
        }

        Input = stdIn;
        Output = stdOut;
        IsInputInteractive = inputInteractive;
        IsOutputInteractive = outputInteractive;
        _treatControlAsInputOriginal = Console.TreatControlCAsInput;
    }

    public Stream Input { get; }

    public Stream Output { get; }

    public bool IsInputInteractive { get; }

    public bool IsOutputInteractive { get; }

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        if (_prepared)
        {
            return ValueTask.CompletedTask;
        }

        Console.TreatControlCAsInput = true;

        if (OperatingSystem.IsWindows())
        {
            TryEnableWindowsVtModes();
        }
        else
        {
            TryEnableUnixRawMode();
        }

        _prepared = true;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        Console.TreatControlCAsInput = _treatControlAsInputOriginal;

        if (OperatingSystem.IsWindows())
        {
            TryRestoreWindowsModes();
        }
        else
        {
            TryRestoreUnixMode();
        }

        _prepared = false;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        try
        {
            return ValueTask.FromResult(new TerminalSize(Console.WindowWidth, Console.WindowHeight));
        }
        catch
        {
            return ValueTask.FromResult(new TerminalSize(80, 24));
        }
    }

    public async ValueTask DisposeAsync()
    {
        await RestoreAsync(CancellationToken.None).ConfigureAwait(false);

        if (_ownsInput)
        {
            await Input.DisposeAsync().ConfigureAwait(false);
        }

        if (_ownsOutput && !ReferenceEquals(Output, Input))
        {
            await Output.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task StreamConsoleKeyEventsAsync(CancellationToken cancellationToken, Action<IMessage> onEvent)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!Console.KeyAvailable)
                {
                    await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                var key = Console.ReadKey(intercept: true);
                var message = MapConsoleKey(key);
                if (message is not null)
                {
                    onEvent(message);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (InvalidOperationException)
            {
                break;
            }
        }
    }

    private void TryEnableWindowsVtModes()
    {
        var inputHandle = GetStdHandle(StdInputHandle);
        var outputHandle = GetStdHandle(StdOutputHandle);

        if (IsInvalidHandle(inputHandle) || IsInvalidHandle(outputHandle))
        {
            return;
        }

        if (GetConsoleMode(inputHandle, out var imode))
        {
            _originalInputMode = imode;
            var nextInputMode = imode | EnableVirtualTerminalInput;
            nextInputMode &= ~(EnableLineInput | EnableEchoInput | EnableProcessedInput);
            _ = SetConsoleMode(inputHandle, nextInputMode);
        }

        if (GetConsoleMode(outputHandle, out var omode))
        {
            _originalOutputMode = omode;
            _ = SetConsoleMode(outputHandle, omode | EnableVirtualTerminalProcessing | DisableNewlineAutoReturn);
        }
    }

    private void TryRestoreWindowsModes()
    {
        var inputHandle = GetStdHandle(StdInputHandle);
        var outputHandle = GetStdHandle(StdOutputHandle);

        if (!IsInvalidHandle(inputHandle) && _originalInputMode != 0)
        {
            _ = SetConsoleMode(inputHandle, _originalInputMode);
        }

        if (!IsInvalidHandle(outputHandle) && _originalOutputMode != 0)
        {
            _ = SetConsoleMode(outputHandle, _originalOutputMode);
        }
    }

    private void TryEnableUnixRawMode()
    {
        if (!IsInputInteractive)
        {
            return;
        }

        _unixSttyState = RunStty("-g");
        if (string.IsNullOrWhiteSpace(_unixSttyState))
        {
            return;
        }

        _ = RunStty("raw -echo");
    }

    private void TryRestoreUnixMode()
    {
        if (string.IsNullOrWhiteSpace(_unixSttyState))
        {
            return;
        }

        _ = RunStty(_unixSttyState);
        _unixSttyState = null;
    }

    private static string? RunStty(string arguments)
    {
        try
        {
            var escaped = arguments.Replace("'", "'\"'\"'", StringComparison.Ordinal);
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c 'stty {escaped} < /dev/tty'",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            };

            if (!process.Start())
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd();
            _ = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return null;
            }

            return output.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static bool TryOpenTty(FileAccess access, out Stream stream)
    {
        try
        {
            stream = new FileStream("/dev/tty", FileMode.Open, access, FileShare.ReadWrite);
            return true;
        }
        catch
        {
            stream = Stream.Null;
            return false;
        }
    }

    private static bool IsInvalidHandle(IntPtr handle)
    {
        return handle == IntPtr.Zero || handle == new IntPtr(-1);
    }

    private static IMessage? MapConsoleKey(ConsoleKeyInfo key)
    {
        var modifiers = KeyModifiers.None;
        if ((key.Modifiers & ConsoleModifiers.Control) != 0)
        {
            modifiers |= KeyModifiers.Ctrl;
        }

        if ((key.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            modifiers |= KeyModifiers.Alt;
        }

        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
        {
            modifiers |= KeyModifiers.Shift;
        }

        return key.Key switch
        {
            ConsoleKey.UpArrow => new KeyPressMsg(KeyCode.Up, "", modifiers),
            ConsoleKey.DownArrow => new KeyPressMsg(KeyCode.Down, "", modifiers),
            ConsoleKey.LeftArrow => new KeyPressMsg(KeyCode.Left, "", modifiers),
            ConsoleKey.RightArrow => new KeyPressMsg(KeyCode.Right, "", modifiers),
            ConsoleKey.Enter => new KeyPressMsg(KeyCode.Enter, "", modifiers),
            ConsoleKey.Tab => new KeyPressMsg(KeyCode.Tab, "", modifiers),
            ConsoleKey.Backspace => new KeyPressMsg(KeyCode.Backspace, "", modifiers),
            ConsoleKey.Escape => new KeyPressMsg(KeyCode.Escape, "", modifiers),
            _ => ToCharacterMessage(key, modifiers),
        };
    }

    private static IMessage? ToCharacterMessage(ConsoleKeyInfo key, KeyModifiers modifiers)
    {
        if (key.KeyChar == '\0')
        {
            return null;
        }

        return new KeyPressMsg(KeyCode.Character, key.KeyChar.ToString(), modifiers);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
}
