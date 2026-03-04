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

internal sealed record NumberMsg(int Value) : IMessage;

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

internal sealed class DelegateDisposable(Action dispose) : IDisposable
{
    public void Dispose()
    {
        dispose();
    }
}
