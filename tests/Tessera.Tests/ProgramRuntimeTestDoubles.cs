using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Commands;
using Tessera.Core.Messages;
using Tessera.Core.Rendering;
using Tessera.Core.Terminal;
using ModelView = Tessera.Core.Abstractions.ScreenOutput;

namespace Tessera.Tests;

internal abstract class TestRuntimeModel
{
    public virtual Effect? Init()
    {
        return null;
    }

    public abstract Effect? Update(IMessage message);

    public abstract ModelView Render();
}

internal static class TestDoubleAsync
{
    public static ValueTask Completed(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }
}

internal sealed class InitQuitModel : TestRuntimeModel
{
    public override Effect? Init()
    {
        return Effects.Quit;
    }

    public override Effect? Update(IMessage message)
    {
        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("quit");
    }
}

internal sealed class IdleModel : TestRuntimeModel
{
    public override Effect? Update(IMessage message)
    {
        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("idle");
    }
}

internal sealed class SequenceModel : TestRuntimeModel
{
    public List<int> Values { get; } = [];

    public override Effect? Init()
    {
        return Effects.Sequence(
            Effects.FromMessage(new NumberMsg(1)),
            Effects.FromMessage(new NumberMsg(2)),
            Effects.Quit);
    }

    public override Effect? Update(IMessage message)
    {
        if (message is NumberMsg number)
        {
            Values.Add(number.Value);
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("sequence");
    }
}

internal sealed class BatchModel : TestRuntimeModel
{
    public int Count { get; private set; }

    public override Effect? Init()
    {
        return Effects.Batch(
            Effects.FromMessage(new NumberMsg(1)),
            Effects.FromMessage(new NumberMsg(2)));
    }

    public override Effect? Update(IMessage message)
    {
        if (message is NumberMsg)
        {
            Count++;
            if (Count == 2)
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("batch");
    }
}

internal sealed class CommandErrorCaptureModel : TestRuntimeModel
{
    private const string CommandFailureMessage = "command-failure-for-tests";

    public Exception? CapturedError { get; private set; }

    public override Effect? Init()
    {
        return _ => throw new InvalidOperationException(CommandFailureMessage);
    }

    public override Effect? Update(IMessage message)
    {
        if (message is EffectErrorMsg error)
        {
            CapturedError = error.Exception;
            return Effects.Quit;
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("command-error-capture");
    }
}

internal sealed class CommandFaultModel : TestRuntimeModel
{
    public const string FailureMessage = "command-failure-for-tests";

    public override Effect? Init()
    {
        return _ => throw new InvalidOperationException(FailureMessage);
    }

    public override Effect? Update(IMessage message)
    {
        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("command-fault");
    }
}

internal sealed class CommandRecoveryModel : TestRuntimeModel
{
    public int? RecoveredValue { get; private set; }

    public override Effect? Init()
    {
        return _ => throw new InvalidOperationException(CommandFaultModel.FailureMessage);
    }

    public override Effect? Update(IMessage message)
    {
        if (message is NumberMsg number)
        {
            RecoveredValue = number.Value;
            return Effects.Quit;
        }

        if (message is EffectErrorMsg)
        {
            return Effects.Quit;
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("command-recovery");
    }
}

internal sealed class BurstUpdateModel : TestRuntimeModel
{
    private readonly int _targetCount;

    public BurstUpdateModel(int targetCount)
    {
        _targetCount = Math.Max(1, targetCount);
    }

    public int Count { get; private set; }

    public override Effect? Init()
    {
        var commands = new List<Effect?>(_targetCount);
        for (var i = 0; i < _targetCount; i++)
        {
            commands.Add(Effects.FromMessage(new NumberMsg(i + 1)));
        }

        return Effects.Batch([.. commands]);
    }

    public override Effect? Update(IMessage message)
    {
        if (message is NumberMsg)
        {
            Count++;
            if (Count >= _targetCount)
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From($"burst-{Count}");
    }
}

internal sealed class ResizeTrackingModel : TestRuntimeModel
{
    public List<(int W, int H)> Seen { get; } = [];

    public override Effect? Init()
    {
        return null;
    }

    public override Effect? Update(IMessage message)
    {
        if (message is WindowSizeMsg ws)
        {
            Seen.Add((ws.Width, ws.Height));
            if (Seen.Count >= 2)
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("resize");
    }
}

internal sealed class CapabilityTrackingModel : TestRuntimeModel
{
    public TerminalCapabilityProfile? Seen { get; private set; }

    public override Effect? Init()
    {
        return null;
    }

    public override Effect? Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen = capabilities.Profile;
            return Effects.Quit;
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("capabilities");
    }
}

internal sealed class ColorProfileTrackingModel : TestRuntimeModel
{
    public TerminalColorProfile Seen { get; private set; } = TerminalColorProfile.Unknown;

    public override Effect? Init()
    {
        return null;
    }

    public override Effect? Update(IMessage message)
    {
        if (message is ColorProfileMsg colorProfile)
        {
            Seen = colorProfile.Profile;
            return Effects.Quit;
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("color-profile");
    }
}

internal sealed class CapabilityRefinementModel : TestRuntimeModel
{
    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public override Effect? Init()
    {
        return Effects.FromMessage(new ModeReportMsg(2026, ModeReportState.Reset));
    }

    public override Effect? Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
            if (Seen.Count >= 2)
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("capability-refinement");
    }
}

internal sealed class UnsupportedModeReportRefinementModel : TestRuntimeModel
{
    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public override Effect? Init()
    {
        return Effects.FromMessage(new ModeReportMsg(1006, ModeReportState.Unsupported));
    }

    public override Effect? Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
            if (Seen.Count >= 2)
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("capability-unsupported-refinement");
    }
}

internal sealed class CapabilityProbeTimeoutModel : TestRuntimeModel
{
    private readonly TimeSpan _safetyQuitDelay;

    public CapabilityProbeTimeoutModel(TimeSpan safetyQuitDelay)
    {
        _safetyQuitDelay = safetyQuitDelay;
    }

    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public override Effect? Init()
    {
        return Effects.Tick(_safetyQuitDelay, _ => new QuitMsg());
    }

    public override Effect? Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
            if (capabilities.Profile.Source.Contains("+probe-timeout", StringComparison.Ordinal))
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("capability-probe-timeout");
    }
}

internal sealed class CapabilityProbeResponseModel : TestRuntimeModel
{
    private readonly TimeSpan _quitDelay;
    private readonly IReadOnlyList<ModeReportMsg> _reports;

    public CapabilityProbeResponseModel(TimeSpan quitDelay, IReadOnlyList<ModeReportMsg> reports)
    {
        _quitDelay = quitDelay;
        _reports = reports;
    }

    public List<TerminalCapabilityProfile> Seen { get; } = [];

    public override Effect? Init()
    {
        var commands = new List<Effect?>(_reports.Count + 1);
        foreach (var report in _reports)
        {
            commands.Add(Effects.FromMessage(report));
        }

        commands.Add(Effects.Tick(_quitDelay, _ => new QuitMsg()));
        return Effects.Sequence([.. commands]);
    }

    public override Effect? Update(IMessage message)
    {
        if (message is TerminalCapabilitiesMsg capabilities)
        {
            Seen.Add(capabilities.Profile);
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("capability-probe-response");
    }
}

internal sealed class TimedQuitModel : TestRuntimeModel
{
    private readonly TimeSpan _delay;

    public TimedQuitModel(TimeSpan delay)
    {
        _delay = delay;
    }

    public override Effect? Init()
    {
        return Effects.Tick(_delay, _ => new QuitMsg());
    }

    public override Effect? Update(IMessage message)
    {
        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("timed-quit");
    }
}

internal sealed class TimedQuitProbeViewModel : TestRuntimeModel
{
    private readonly TimeSpan _delay;

    public TimedQuitProbeViewModel(TimeSpan delay)
    {
        _delay = delay;
    }

    public override Effect? Init()
    {
        return Effects.Tick(_delay, _ => new QuitMsg());
    }

    public override Effect? Update(IMessage message)
    {
        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("timed-probe-view") with
        {
            Terminal = new TerminalOutput
            {
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                EnableSynchronizedUpdates = true,
                MouseMode = MouseMode.AllMotion
            }
        };
    }
}

internal sealed class ConcurrencyTrackingModel : TestRuntimeModel
{
    private readonly int _commandCount;
    private readonly TimeSpan _delay;
    private int _activeCommands;
    private int _maxActiveCommands;
    private int _receivedMessages;

    public ConcurrencyTrackingModel(int commandCount, TimeSpan delay)
    {
        _commandCount = Math.Max(1, commandCount);
        _delay = delay;
    }

    public int MaxActiveCommands => Volatile.Read(ref _maxActiveCommands);

    public override Effect? Init()
    {
        var commands = new Effect?[_commandCount];
        for (var i = 0; i < _commandCount; i++)
        {
            commands[i] = async cancellationToken =>
            {
                var active = Interlocked.Increment(ref _activeCommands);
                TrackMax(active);
                try
                {
                    await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    Interlocked.Decrement(ref _activeCommands);
                }

                return new NumberMsg(1);
            };
        }

        return Effects.Batch(commands);
    }

    public override Effect? Update(IMessage message)
    {
        if (message is NumberMsg)
        {
            var received = Interlocked.Increment(ref _receivedMessages);
            if (received >= _commandCount)
            {
                return Effects.Quit;
            }
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("concurrency-tracking");
    }

    private void TrackMax(int active)
    {
        while (true)
        {
            var currentMax = Volatile.Read(ref _maxActiveCommands);
            if (active <= currentMax)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _maxActiveCommands, active, currentMax) == currentMax)
            {
                return;
            }
        }
    }
}

internal sealed class RawOutputInitModel : TestRuntimeModel
{
    public override Effect? Init()
    {
        return Effects.Sequence(
            Effects.Raw("raw-sequence"),
            Effects.Quit);
    }

    public override Effect? Update(IMessage message)
    {
        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("raw-output");
    }
}

internal sealed class MouseInterceptModel : TestRuntimeModel
{
    public int Intercepted { get; private set; }

    public override Effect? Init()
    {
        return null;
    }

    public override Effect? Update(IMessage message)
    {
        if (message is NumberMsg)
        {
            Intercepted++;
            return Effects.Quit;
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("mouse-intercept") with
        {
            Input = new InputHooks { OnMouse = _ => Effects.FromMessage(new NumberMsg(7)) }
        };
    }
}

internal sealed class CapabilityAwareRendererSpy : IProgramRenderer
{
    public List<TerminalCapabilityProfile> Updates { get; } = [];

    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
    {
        _ = output;
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public void Resize(int width, int height)
    {
        _ = width;
        _ = height;
    }

    public void UpdateCapabilities(TerminalCapabilityProfile capabilities)
    {
        Updates.Add(capabilities);
    }

    public void Render(ModelView output)
    {
        _ = output;
    }

    public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken)
    {
        _ = content;
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        return TestDoubleAsync.Completed(cancellationToken);
    }

    public ValueTask ResetAsync(CancellationToken cancellationToken)
    {
        return FlushAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

internal sealed class RenderCountingRendererSpy : IProgramRenderer
{
    public int RenderCalls { get; private set; }

    public int FlushCalls { get; private set; }

    public List<string> RawWrites { get; } = [];

    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
    {
        _ = output;
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public void Resize(int width, int height)
    {
        _ = width;
        _ = height;
    }

    public void UpdateCapabilities(TerminalCapabilityProfile capabilities)
    {
        _ = capabilities;
    }

    public void Render(ModelView output)
    {
        _ = output;
        RenderCalls++;
    }

    public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken)
    {
        _ = content;
        _ = cancellationToken;
        RawWrites.Add(content);
        return ValueTask.CompletedTask;
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        FlushCalls++;
        return ValueTask.CompletedTask;
    }

    public ValueTask ResetAsync(CancellationToken cancellationToken)
    {
        return TestDoubleAsync.Completed(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

internal sealed record NumberMsg(int Value) : IMessage;

internal sealed class QuitOnQModel : TestRuntimeModel
{
    public override Effect? Init()
    {
        return null;
    }

    public override Effect? Update(IMessage message)
    {
        if (message is KeyPressMsg key
            && key.IsCharacter('q', KeyModifiers.None))
        {
            return Effects.Quit;
        }

        return null;
    }

    public override ModelView Render()
    {
        return ModelView.From("quit-on-q");
    }
}

internal sealed class DisposeOrderingTerminalAdapter : ITerminalAdapter
{
    private readonly CancelAwareInputStream _input = new();

    public bool DisposeObservedCancellation { get; private set; }

    public Stream Input => _input;

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => true;

    public bool IsOutputInteractive => false;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        return PrepareAsync(cancellationToken);
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
        return PrepareAsync(cancellationToken);
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
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
        return PrepareAsync(cancellationToken);
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var call = Interlocked.Increment(ref _callCount);
        return call <= 1
            ? ValueTask.FromResult(new TerminalSize(80, 24))
            : ValueTask.FromResult(new TerminalSize(100, 40));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
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
        return PrepareAsync(cancellationToken);
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        lock (_sync)
        {
            return ValueTask.FromResult(_size);
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void SetSize(int width, int height)
    {
        lock (_sync)
        {
            _size = new TerminalSize(width, height);
        }
    }
}

internal sealed class InteractiveProbeTerminalAdapter : ITerminalAdapter
{
    private readonly MemoryStream _output = new();

    public string OutputText => Encoding.UTF8.GetString(_output.ToArray());

    public Stream Input { get; } = new MemoryStream();

    public Stream Output => _output;

    public bool IsInputInteractive => true;

    public bool IsOutputInteractive => true;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        return PrepareAsync(cancellationToken);
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

internal sealed class InteractiveInputTerminalAdapter : ITerminalAdapter
{
    private readonly MemoryStream _input;

    public InteractiveInputTerminalAdapter(string input)
    {
        _input = new MemoryStream(Encoding.UTF8.GetBytes(input));
    }

    public Stream Input => _input;

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => true;

    public bool IsOutputInteractive => false;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        return PrepareAsync(cancellationToken);
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync()
    {
        _input.Dispose();
        return ValueTask.CompletedTask;
    }
}

internal sealed class QuitOnFirstByteDecoder : IEventDecoder
{
    public int Calls { get; private set; }

    public DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired)
    {
        _ = timeoutExpired;
        Calls++;
        if (buffer.IsEmpty)
        {
            return new DecodeResult(0, null, false);
        }

        return new DecodeResult(1, new QuitMsg(), false);
    }
}

internal sealed class DelegateDisposable(Action dispose) : IDisposable
{
    public void Dispose()
    {
        dispose();
    }
}
