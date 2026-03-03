using System.Threading.Channels;
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
            _renderer = _options.DisableRenderer
                ? new NullRenderer()
                : _options.Renderer ?? new AnsiDiffRenderer();

            await _terminal.PrepareAsync(token).ConfigureAwait(false);
            await _renderer.InitializeAsync(_terminal.Output, token).ConfigureAwait(false);

            if (_terminal.IsOutputInteractive)
            {
                var size = await _terminal.GetSizeAsync(token).ConfigureAwait(false);
                _renderer.Resize(size.Width, size.Height);
                Send(new WindowSizeMsg(size.Width, size.Height));
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
                        await AwaitBackgroundLoops(commandLoop, inputLoop).ConfigureAwait(false);
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
            await AwaitBackgroundLoops(commandLoop, inputLoop).ConfigureAwait(false);
            return Model;
        }
        finally
        {
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

        if (_terminal is ConsoleTerminalAdapter consoleTerminal && _options.UseConsoleKeyEvents)
        {
            return Task.Run(() => consoleTerminal.StreamConsoleKeyEventsAsync(token, Send), token);
        }

        _reader = new TerminalReader(_terminal.Input, new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _reader.StreamEventsAsync(token, Send), token);
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

    private static async Task AwaitBackgroundLoops(Task commandLoop, Task? inputLoop)
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
    }
}
