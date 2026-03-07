using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Commands;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using ModelView = TeaSharp.Core.Abstractions.View;

namespace TeaSharp.Tests;

internal sealed class InitQuitModel : IModel
{
    public Command? Init() => Commands.Quit;

    public UpdateResult Update(IMessage message) => new(this, null);

    public ModelView View() => ModelView.From("quit");
}

internal sealed class IdleModel : IModel
{
    public Command? Init() => null;

    public UpdateResult Update(IMessage message) => new(this, null);

    public ModelView View() => ModelView.From("idle");
}

internal sealed class SequenceModel : IModel
{
    public List<int> Values { get; } = [];

    public Command? Init() => Commands.Sequence(
        Commands.FromMessage(new NumberMsg(1)),
        Commands.FromMessage(new NumberMsg(2)),
        Commands.Quit);

    public UpdateResult Update(IMessage message)
    {
        if (message is NumberMsg number)
        {
            Values.Add(number.Value);
        }

        return new(this, null);
    }

    public ModelView View() => ModelView.From("sequence");
}

internal sealed class BatchModel : IModel
{
    public int Count { get; private set; }

    public Command? Init() => Commands.Batch(
        Commands.FromMessage(new NumberMsg(1)),
        Commands.FromMessage(new NumberMsg(2)));

    public UpdateResult Update(IMessage message)
    {
        if (message is NumberMsg)
        {
            Count++;
            if (Count == 2)
            {
                return new(this, Commands.Quit);
            }
        }

        return new(this, null);
    }

    public ModelView View() => ModelView.From("batch");
}

internal sealed class CommandErrorCaptureModel : IModel
{
    private const string CommandFailureMessage = "command-failure-for-tests";

    public Exception? CapturedError { get; private set; }

    public Command? Init() => _ => throw new InvalidOperationException(CommandFailureMessage);

    public UpdateResult Update(IMessage message)
    {
        if (message is CommandErrorMsg error)
        {
            CapturedError = error.Exception;
            return new UpdateResult(this, Commands.Quit);
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("command-error-capture");
}

internal sealed class CommandFaultModel : IModel
{
    public const string FailureMessage = "command-failure-for-tests";

    public Command? Init() => _ => throw new InvalidOperationException(FailureMessage);

    public UpdateResult Update(IMessage message) => new(this, null);

    public ModelView View() => ModelView.From("command-fault");
}

internal sealed class ResizeTrackingModel : IModel
{
    public List<(int W, int H)> Seen { get; } = [];

    public Command? Init() => null;

    public UpdateResult Update(IMessage message)
    {
        if (message is WindowSizeMsg ws)
        {
            Seen.Add((ws.Width, ws.Height));
            if (Seen.Count >= 2)
            {
                return new UpdateResult(this, Commands.Quit);
            }
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("resize");
}

internal sealed class CapabilityTrackingModel : IModel
{
    public TerminalCapabilityProfile? Seen { get; private set; }

    public Command? Init() => null;

    public UpdateResult Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen = capabilities.Profile;
            return new UpdateResult(this, Commands.Quit);
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("capabilities");
}

internal sealed class CapabilityRefinementModel : IModel
{
    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public Command? Init() => Commands.FromMessage(new ModeReportMsg(2026, ModeReportState.Reset));

    public UpdateResult Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
            if (Seen.Count >= 2)
            {
                return new UpdateResult(this, Commands.Quit);
            }
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("capability-refinement");
}

internal sealed class UnsupportedModeReportRefinementModel : IModel
{
    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public Command? Init() => Commands.FromMessage(new ModeReportMsg(1006, ModeReportState.Unsupported));

    public UpdateResult Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
            if (Seen.Count >= 2)
            {
                return new UpdateResult(this, Commands.Quit);
            }
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("capability-unsupported-refinement");
}

internal sealed class CapabilityProbeTimeoutModel : IModel
{
    private readonly TimeSpan _safetyQuitDelay;
    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public CapabilityProbeTimeoutModel(TimeSpan safetyQuitDelay)
    {
        _safetyQuitDelay = safetyQuitDelay;
    }

    public Command? Init() => Commands.Tick(_safetyQuitDelay, _ => new QuitMsg());

    public UpdateResult Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
            if (capabilities.Profile.Source.Contains("+probe-timeout", StringComparison.Ordinal))
            {
                return new UpdateResult(this, Commands.Quit);
            }
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("capability-probe-timeout");
}

internal sealed class CapabilityProbeResponseModel : IModel
{
    private readonly TimeSpan _quitDelay;
    private readonly IReadOnlyList<ModeReportMsg> _reports;
    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public CapabilityProbeResponseModel(TimeSpan quitDelay, IReadOnlyList<ModeReportMsg> reports)
    {
        _quitDelay = quitDelay;
        _reports = reports;
    }

    public Command? Init()
    {
        var commands = new List<Command?>(_reports.Count + 1);
        foreach (var report in _reports)
        {
            commands.Add(Commands.FromMessage(report));
        }

        commands.Add(Commands.Tick(_quitDelay, _ => new QuitMsg()));
        return Commands.Sequence([.. commands]);
    }

    public UpdateResult Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("capability-probe-response");
}

internal sealed class TimedQuitModel : IModel
{
    private readonly TimeSpan _delay;

    public TimedQuitModel(TimeSpan delay)
    {
        _delay = delay;
    }

    public Command? Init() => Commands.Tick(_delay, _ => new QuitMsg());

    public UpdateResult Update(IMessage message) => new(this, null);

    public ModelView View() => ModelView.From("timed-quit");
}

internal sealed record NumberMsg(int Value) : IMessage;

internal sealed class QuitOnQModel : IModel
{
    public Command? Init() => null;

    public UpdateResult Update(IMessage message)
    {
        if (message is KeyPressMsg key
            && key.Code == KeyCode.Character
            && string.Equals(key.Text, "q", StringComparison.Ordinal))
        {
            return new UpdateResult(this, Commands.Quit);
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("quit-on-q");
}

internal sealed class DisposeOrderingTerminalAdapter : ITerminalAdapter
{
    private readonly CancelAwareInputStream _input = new();

    public Stream Input => _input;

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => true;

    public bool IsOutputInteractive => false;

    public bool DisposeObservedCancellation { get; private set; }

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync()
    {
        DisposeObservedCancellation = _input.CancellationObserved;
        if (!DisposeObservedCancellation)
        {
            throw new InvalidOperationException("Terminal disposed before input cancellation was observed.");
        }

        return ValueTask.CompletedTask;
    }

    private sealed class CancelAwareInputStream : Stream
    {
        private int _readCount;
        public bool CancellationObserved { get; private set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_readCount == 0)
            {
                _readCount++;
                buffer.Span[0] = (byte)'q';
                return ValueTask.FromResult(1);
            }

            var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            cancellationToken.Register(() =>
            {
                CancellationObserved = true;
                completion.TrySetCanceled(cancellationToken);
            });
            return new ValueTask<int>(completion.Task);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}

internal sealed class FakeTerminalAdapter : ITerminalAdapter
{
    public Stream Input { get; } = new MemoryStream();

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => false;

    public bool IsOutputInteractive => false;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class ResizingFakeTerminal : ITerminalAdapter
{
    private int _callCount;

    public Stream Input { get; } = new MemoryStream();

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => false;

    public bool IsOutputInteractive => true;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var call = Interlocked.Increment(ref _callCount);
        return call <= 1
            ? ValueTask.FromResult(new TerminalSize(80, 24))
            : ValueTask.FromResult(new TerminalSize(100, 40));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class SignalDrivenFakeTerminal : ITerminalAdapter
{
    private readonly object _sync = new();
    private TerminalSize _size;

    public SignalDrivenFakeTerminal(TerminalSize initialSize)
    {
        _size = initialSize;
    }

    public Stream Input { get; } = new MemoryStream();

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => false;

    public bool IsOutputInteractive => true;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        lock (_sync)
        {
            return ValueTask.FromResult(_size);
        }
    }

    public void SetSize(int width, int height)
    {
        lock (_sync)
        {
            _size = new TerminalSize(width, height);
        }
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class InteractiveProbeTerminalAdapter : ITerminalAdapter
{
    private readonly MemoryStream _output = new();

    public Stream Input { get; } = new MemoryStream();

    public Stream Output => _output;

    public bool IsInputInteractive => true;

    public bool IsOutputInteractive => true;

    public string OutputText => Encoding.UTF8.GetString(_output.ToArray());

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class DelegateDisposable(Action dispose) : IDisposable
{
    public void Dispose()
    {
        dispose();
    }
}
