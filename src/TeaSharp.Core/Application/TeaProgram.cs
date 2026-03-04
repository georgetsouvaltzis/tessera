using System.Threading.Channels;
using System.Runtime.InteropServices;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

public sealed class TeaProgram
{
    private readonly ProgramOptions _options;
    private readonly Channel<IMessage> _messages;
    private readonly Channel<Command> _commands;
    private readonly object _stateLock = new();

    private ITerminalAdapter? _terminal;
    private IProgramRenderer? _renderer;
    private TerminalReader? _reader;
    private CancellationTokenSource? _cts;
    private bool _running;

    public TeaProgram(IModel initialModel, ProgramOptions? options = null)
    {
        Model = initialModel ?? throw new ArgumentNullException(nameof(initialModel));
        _options = options ?? new ProgramOptions();
        _messages = Channel.CreateUnbounded<IMessage>();
        _commands = Channel.CreateUnbounded<Command>();
    }

    public IModel Model { get; private set; }

    public void Send(IMessage message)
    {
        if (message is null)
        {
            return;
        }

        _messages.Writer.TryWrite(message);
    }

    public async Task<IModel> RunAsync(CancellationToken cancellationToken = default)
    {
        IDisposable? resizeSignalRegistration = null;

        lock (_stateLock)
        {
            if (_running)
            {
                throw new InvalidOperationException("Program is already running.");
            }

            _running = true;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        var token = _cts.Token;

        try
        {
            _terminal = _options.Terminal ?? new ConsoleTerminalAdapter();
            var capabilities = _options.TerminalCapabilities ?? TerminalCapabilityDetector.Detect();
            _renderer = _options.DisableRenderer
                ? new NullRenderer()
                : _options.Renderer ?? new AnsiDiffRenderer(capabilities);

            await _terminal.PrepareAsync(token).ConfigureAwait(false);
            await _renderer.InitializeAsync(_terminal.Output, token).ConfigureAwait(false);

            Task? resizeLoop = null;
            if (_terminal.IsOutputInteractive)
            {
                var size = await _terminal.GetSizeAsync(token).ConfigureAwait(false);
                _renderer.Resize(size.Width, size.Height);
                Send(new WindowSizeMsg(size.Width, size.Height));
                (resizeLoop, resizeSignalRegistration) = StartResizeLoop(size, token);
            }

            var commandLoop = Task.Run(() => CommandLoopAsync(token), token);
            var inputLoop = StartInputLoop(token);

            if (Model.Init() is { } initCommand)
            {
                await _commands.Writer.WriteAsync(initCommand, token).ConfigureAwait(false);
            }

            await RenderAsync(Model.View(), token).ConfigureAwait(false);

            var minFrame = TimeSpan.FromSeconds(1.0 / Math.Clamp(_options.MaxFps, 1, 120));
            var lastRender = DateTimeOffset.MinValue;

            while (await _messages.Reader.WaitToReadAsync(token).ConfigureAwait(false))
            {
                while (_messages.Reader.TryRead(out var message))
                {
                    var filtered = _options.Filter is null
                        ? message
                        : _options.Filter(Model, message);
                    if (filtered is null)
                    {
                        continue;
                    }

                    if (filtered is QuitMsg)
                    {
                        await ShutdownAsync(kill: false, token).ConfigureAwait(false);
                        _cts?.Cancel();
                        await AwaitBackgroundLoops(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
                        return Model;
                    }

                    if (filtered is InterruptMsg)
                    {
                        throw new TeaProgramInterruptedException();
                    }

                    switch (filtered)
                    {
                        case WindowSizeMsg ws:
                            _renderer.Resize(ws.Width, ws.Height);
                            break;
                        case BatchMsg batch:
                            foreach (var command in batch.Commands)
                            {
                                await _commands.Writer.WriteAsync(command, token).ConfigureAwait(false);
                            }
                            continue;
                        case SequenceMsg sequence:
                            _ = Task.Run(() => RunSequenceAsync(sequence.Commands, token), token);
                            continue;
                    }

                    var update = Model.Update(filtered);
                    Model = update.Model;

                    if (update.Command is not null)
                    {
                        await _commands.Writer.WriteAsync(update.Command, token).ConfigureAwait(false);
                    }

                    var now = DateTimeOffset.UtcNow;
                    var elapsed = now - lastRender;
                    if (elapsed < minFrame)
                    {
                        var delay = minFrame - elapsed;
                        await Task.Delay(delay, token).ConfigureAwait(false);
                    }

                    await RenderAsync(Model.View(), token).ConfigureAwait(false);
                    lastRender = DateTimeOffset.UtcNow;
                }
            }

            await ShutdownAsync(kill: false, token).ConfigureAwait(false);
            _cts?.Cancel();
            await AwaitBackgroundLoops(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
            return Model;
        }
        finally
        {
            resizeSignalRegistration?.Dispose();

            if (_terminal is not null || _renderer is not null)
            {
                await ShutdownAsync(kill: true, CancellationToken.None).ConfigureAwait(false);
            }

            lock (_stateLock)
            {
                _running = false;
            }

            _cts?.Dispose();
            _cts = null;
        }
    }

    public async Task StopAsync(bool kill = false, CancellationToken cancellationToken = default)
    {
        if (_cts is null)
        {
            return;
        }

        _cts.Cancel();
        await ShutdownAsync(kill, cancellationToken).ConfigureAwait(false);
    }

    private Task? StartInputLoop(CancellationToken token)
    {
        if (_options.DisableInput || _terminal is null || !_terminal.IsInputInteractive)
        {
            return null;
        }

        if (_terminal is ConsoleTerminalAdapter consoleTerminal
            && (_options.UseConsoleKeyEvents || !consoleTerminal.IsRawModeActive))
        {
            return Task.Run(() => consoleTerminal.StreamConsoleKeyEventsAsync(token, Send), token);
        }

        _reader = new TerminalReader(_terminal.Input, new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _reader.StreamEventsAsync(token, Send), token);
    }

    private (Task? Loop, IDisposable? SignalRegistration) StartResizeLoop(TerminalSize initialSize, CancellationToken token)
    {
        if (_terminal is null || !_terminal.IsOutputInteractive)
        {
            return (null, null);
        }

        var signalTicks = Channel.CreateUnbounded<bool>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });

        var registration = TryRegisterResizeSignal(() => signalTicks.Writer.TryWrite(true));

        var loop = Task.Run(async () =>
        {
            var last = initialSize;
            var interval = _options.ResizePollInterval < TimeSpan.FromMilliseconds(16)
                ? TimeSpan.FromMilliseconds(16)
                : _options.ResizePollInterval;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var pollDelay = Task.Delay(interval, token);
                    var signalWait = signalTicks.Reader.WaitToReadAsync(token).AsTask();
                    await Task.WhenAny(pollDelay, signalWait).ConfigureAwait(false);

                    while (signalTicks.Reader.TryRead(out _))
                    {
                        // Drain queued resize signals.
                    }

                    var current = await _terminal.GetSizeAsync(token).ConfigureAwait(false);
                    if (current.Width != last.Width || current.Height != last.Height)
                    {
                        last = current;
                        Send(new WindowSizeMsg(current.Width, current.Height));
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // ignored: size monitoring best-effort.
                }
            }
        }, token);

        return (loop, registration);
    }

    private IDisposable? TryRegisterResizeSignal(Action onResize)
    {
        if (_options.ResizeSignalRegistrationFactory is not null)
        {
            return _options.ResizeSignalRegistrationFactory(onResize);
        }

        if (!_options.EnableResizeSignals || !(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()))
        {
            return null;
        }

        try
        {
            return PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ =>
            {
                try
                {
                    onResize();
                }
                catch
                {
                    // ignored: callback must not throw.
                }
            });
        }
        catch
        {
            return null;
        }
    }

    private async Task CommandLoopAsync(CancellationToken token)
    {
        while (await _commands.Reader.WaitToReadAsync(token).ConfigureAwait(false))
        {
            while (_commands.Reader.TryRead(out var command))
            {
                _ = Task.Run(() => ExecuteCommandAsync(command, token), token);
            }
        }
    }

    private async Task ExecuteCommandAsync(Command command, CancellationToken token)
    {
        try
        {
            var message = await command(token).ConfigureAwait(false);
            if (message is not null)
            {
                Send(message);
            }
        }
        catch (Exception ex) when (_options.CatchCommandExceptions)
        {
            Send(new CommandErrorMsg(ex));
        }
    }

    private async Task RunSequenceAsync(IReadOnlyList<Command> commands, CancellationToken token)
    {
        foreach (var command in commands)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await ExecuteCommandAsync(command, token).ConfigureAwait(false);
        }
    }

    private async Task RenderAsync(View view, CancellationToken token)
    {
        if (_renderer is null)
        {
            return;
        }

        _renderer.Render(view);
        await _renderer.FlushAsync(token).ConfigureAwait(false);
    }

    private async Task ShutdownAsync(bool kill, CancellationToken token)
    {
        _commands.Writer.TryComplete();
        _messages.Writer.TryComplete();

        if (_renderer is not null)
        {
            if (!kill)
            {
                await _renderer.ResetAsync(token).ConfigureAwait(false);
            }

            await _renderer.DisposeAsync().ConfigureAwait(false);
            _renderer = null;
        }

        if (_terminal is not null)
        {
            await _terminal.RestoreAsync(token).ConfigureAwait(false);
            await _terminal.DisposeAsync().ConfigureAwait(false);
            _terminal = null;
        }
    }

    private static async Task AwaitBackgroundLoops(Task commandLoop, Task? inputLoop, Task? resizeLoop)
    {
        try
        {
            await commandLoop.ConfigureAwait(false);
        }
        catch
        {
            // ignored: shutdown path.
        }

        if (inputLoop is not null)
        {
            try
            {
                await inputLoop.ConfigureAwait(false);
            }
            catch
            {
                // ignored: shutdown path.
            }
        }

        if (resizeLoop is not null)
        {
            try
            {
                await resizeLoop.ConfigureAwait(false);
            }
            catch
            {
                // ignored: shutdown path.
            }
        }
    }
}
