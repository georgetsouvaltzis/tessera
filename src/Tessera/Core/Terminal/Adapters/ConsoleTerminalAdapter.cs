using System.ComponentModel;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;
using Tessera.Core.Terminal.Adapters.Internal;

namespace Tessera.Core.Terminal.Adapters;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ConsoleTerminalAdapter : ITerminalAdapter
{
    private readonly bool _ownsInput;
    private readonly bool _ownsOutput;
    private readonly bool _treatControlAsInputOriginal;
    private readonly UnixRawModeSession _unixRawMode = new();
    private bool _isWindowsVirtualTerminalInputEnabled;
    private uint _originalInputMode;
    private uint _originalOutputMode;
    private bool _prepared;

    public ConsoleTerminalAdapter()
    {
        var stdIn = Console.OpenStandardInput();
        var stdOut = Console.OpenStandardOutput();
        var inputInteractive = !Console.IsInputRedirected;
        var outputInteractive = !Console.IsOutputRedirected;

        if (!OperatingSystem.IsWindows() && TryOpenTty(FileAccess.ReadWrite, out var ttyIn))
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

    public bool IsRawModeActive => _unixRawMode.IsRawModeActive;

    public bool IsVirtualTerminalInputEnabled =>
        !OperatingSystem.IsWindows() || _isWindowsVirtualTerminalInputEnabled;

    public string RawModeDiagnostics => _unixRawMode.RawModeDiagnostics;

    public string RawModeError => _unixRawMode.RawModeError;

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
            _isWindowsVirtualTerminalInputEnabled =
                WindowsConsoleSession.TryEnableVirtualTerminalModes(ref _originalInputMode, ref _originalOutputMode);
        }
        else
        {
            _unixRawMode.TryEnable(IsInputInteractive);
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
            WindowsConsoleSession.TryRestoreModes(_originalInputMode, _originalOutputMode);
        }
        else
        {
            _unixRawMode.Restore();
        }

        _isWindowsVirtualTerminalInputEnabled = false;
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
            await TryDisposeOwnedStreamAsync(Input).ConfigureAwait(false);
        }

        if (_ownsOutput && !ReferenceEquals(Output, Input))
        {
            await TryDisposeOwnedStreamAsync(Output).ConfigureAwait(false);
        }
    }

    public static async Task StreamConsoleKeyEventsAsync(Action<IMessage> onEvent, CancellationToken cancellationToken)
    {
        var burst = new ConsolePasteBurstBuffer();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!Console.KeyAvailable)
                {
                    burst.FlushIfIdle(onEvent);
                    await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                var key = Console.ReadKey(true);
                if (ConsoleClipboardReader.IsPasteShortcut(key)
                    && ConsoleClipboardReader.TryReadText(out var clipboard)
                    && !string.IsNullOrEmpty(clipboard))
                {
                    burst.Flush(onEvent);
                    onEvent(new PasteStartMsg());
                    onEvent(new PasteMsg(clipboard));
                    onEvent(new PasteEndMsg());
                    continue;
                }

                var message = ConsoleKeyMessageMapper.Map(key);
                if (burst.TryBuffer(message, onEvent))
                {
                    continue;
                }

                burst.Flush(onEvent);
                if (message is not null)
                {
                    onEvent(message);
                }
            }
            catch (OperationCanceledException)
            {
                burst.Flush(onEvent);
                break;
            }
            catch (InvalidOperationException)
            {
                burst.Flush(onEvent);
                break;
            }
        }

        burst.Flush(onEvent);
    }

    internal IDisposable? TryRegisterResizeSignal(Action onResize)
    {
        return WindowsConsoleSession.TryRegisterResizeSignal(IsInputInteractive, onResize);
    }

    private static async Task TryDisposeOwnedStreamAsync(Stream stream)
    {
        try
        {
            var disposeTask = stream.DisposeAsync().AsTask();
            var completed = await Task.WhenAny(disposeTask, Task.Delay(120)).ConfigureAwait(false);
            if (ReferenceEquals(completed, disposeTask))
            {
                await disposeTask.ConfigureAwait(false);
            }
        }
        catch
        {
            // Stream disposal is best-effort during terminal teardown.
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
}
