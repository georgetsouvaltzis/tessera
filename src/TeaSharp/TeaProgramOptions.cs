using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;

namespace TeaSharp;

/// <summary>
/// Defines the stable application-facing runtime options used by <see cref="Tea.CreateProgram(IScreen)"/> and <see cref="Tea.CreateProgram(IScreen, TeaProgramOptions)"/>.
/// </summary>
public sealed class TeaProgramOptions
{
    /// <summary>
    /// Filters incoming messages before they reach the application screen.
    /// </summary>
    public Func<IScreen, IMessage, IMessage?>? MessageFilter { get; init; }

    /// <summary>
    /// Sets the maximum render frame rate for the program loop.
    /// </summary>
    public int MaxFps { get; init; } = 60;

    /// <summary>
    /// Enables render coalescing so bursts of messages can share a frame.
    /// </summary>
    public bool AdaptiveFramePacing { get; init; } = true;

    /// <summary>
    /// Disables terminal rendering.
    /// </summary>
    public bool DisableRenderer { get; init; }

    /// <summary>
    /// Disables input processing.
    /// </summary>
    public bool DisableInput { get; init; }

    /// <summary>
    /// Uses console key events when available instead of raw byte decoding.
    /// </summary>
    public bool UseConsoleKeyEvents { get; init; } = true;

    /// <summary>
    /// Converts effect exceptions into messages instead of letting them tear down the program.
    /// </summary>
    public bool CatchEffectExceptions { get; init; } = true;

    /// <summary>
    /// Maps an effect exception to an application message when effect exception recovery is enabled.
    /// </summary>
    public Func<Exception, IMessage?>? MapEffectException { get; init; }

    /// <summary>
    /// Controls how long the input loop waits before treating a trailing escape byte as standalone.
    /// </summary>
    public TimeSpan EscapeTimeout { get; init; } = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Enables terminal resize signal handling when the runtime supports it.
    /// </summary>
    public bool EnableResizeSignals { get; init; } = true;

    /// <summary>
    /// Sets the steady-state polling interval for resize fallback monitoring.
    /// </summary>
    public TimeSpan ResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(120);

    /// <summary>
    /// Sets the minimum interval used by adaptive resize polling.
    /// </summary>
    public TimeSpan MinResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(16);

    internal ProgramOptions ToProgramOptions()
    {
        return new ProgramOptions
        {
            MessageFilter = MessageFilter,
            MaxFps = MaxFps,
            AdaptiveFramePacing = AdaptiveFramePacing,
            DisableRenderer = DisableRenderer,
            DisableInput = DisableInput,
            UseConsoleKeyEvents = UseConsoleKeyEvents,
            CatchEffectExceptions = CatchEffectExceptions,
            MapEffectException = MapEffectException,
            EscapeTimeout = EscapeTimeout,
            EnableResizeSignals = EnableResizeSignals,
            ResizePollInterval = ResizePollInterval,
            MinResizePollInterval = MinResizePollInterval,
        };
    }
}
