using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;

namespace TeaSharp;

public sealed class TeaProgramOptions
{
    public Func<IModel, IMessage, IMessage?>? Filter { get; init; }

    public int MaxFps { get; init; } = 60;

    public bool AdaptiveFramePacing { get; init; } = true;

    public bool DisableRenderer { get; init; }

    public bool DisableInput { get; init; }

    public bool UseConsoleKeyEvents { get; init; } = true;

    public bool CatchCommandExceptions { get; init; } = true;

    public Func<Exception, IMessage?>? RecoverCommandException { get; init; }

    public TimeSpan EscapeTimeout { get; init; } = TimeSpan.FromMilliseconds(50);

    public bool EnableResizeSignals { get; init; } = true;

    public TimeSpan ResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(120);

    public TimeSpan MinResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(16);

    internal ProgramOptions ToProgramOptions()
    {
        return new ProgramOptions
        {
            Filter = Filter,
            MaxFps = MaxFps,
            AdaptiveFramePacing = AdaptiveFramePacing,
            DisableRenderer = DisableRenderer,
            DisableInput = DisableInput,
            UseConsoleKeyEvents = UseConsoleKeyEvents,
            CatchCommandExceptions = CatchCommandExceptions,
            RecoverCommandException = RecoverCommandException,
            EscapeTimeout = EscapeTimeout,
            EnableResizeSignals = EnableResizeSignals,
            ResizePollInterval = ResizePollInterval,
            MinResizePollInterval = MinResizePollInterval,
        };
    }
}
