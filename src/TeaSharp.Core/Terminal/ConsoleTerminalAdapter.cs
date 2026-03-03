using System.Runtime.InteropServices;

namespace TeaSharp.Core.Terminal;

public sealed class ConsoleTerminalAdapter : ITerminalAdapter
{
    private const int StdInputHandle = -10;
    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;
    private const uint DisableNewlineAutoReturn = 0x0008;
    private const uint EnableVirtualTerminalInput = 0x0200;

    private readonly bool _treatControlAsInputOriginal;
    private uint _originalInputMode;
    private uint _originalOutputMode;
    private bool _prepared;

    public ConsoleTerminalAdapter()
    {
        Input = Console.OpenStandardInput();
        Output = Console.OpenStandardOutput();
        IsInputInteractive = !Console.IsInputRedirected;
        IsOutputInteractive = !Console.IsOutputRedirected;
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
        // Do not close process-wide standard streams.
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
            _ = SetConsoleMode(inputHandle, imode | EnableVirtualTerminalInput);
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

    private static bool IsInvalidHandle(IntPtr handle)
    {
        return handle == IntPtr.Zero || handle == new IntPtr(-1);
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
