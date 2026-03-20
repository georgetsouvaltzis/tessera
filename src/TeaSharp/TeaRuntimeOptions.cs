using System.ComponentModel;
using TeaSharp.Styles;

namespace TeaSharp;

/// <summary>
/// Configures runtime behavior for a TeaSharp application.
/// </summary>
/// <remarks>
/// These options control the application loop itself: pacing, input, resize handling, and failure behavior.
/// Use <see cref="Screen"/> for terminal capabilities and per-screen presentation defaults.
/// </remarks>
public sealed class TeaRuntimeOptions
{
    /// <summary>
    /// Gets or sets the maximum render rate in frames per second.
    /// </summary>
    public int MaxFps { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether the runtime may lower frame pacing when the application is idle.
    /// </summary>
    public bool AdaptiveFramePacing { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether rendering is disabled.
    /// </summary>
    public bool DisableRenderer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether input processing is disabled.
    /// </summary>
    public bool DisableInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether console key events should be used when available.
    /// </summary>
    public bool UseConsoleKeyEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether unhandled effect exceptions should be caught by the runtime.
    /// </summary>
    public bool CatchEffectExceptions { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout used when disambiguating escape-key input.
    /// </summary>
    public TimeSpan EscapeTimeout { get; set; } = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Gets or sets a value indicating whether resize signals should be monitored.
    /// </summary>
    public bool EnableResizeSignals { get; set; } = true;

    /// <summary>
    /// Gets or sets how often the runtime polls for resize changes.
    /// </summary>
    public TimeSpan ResizePollInterval { get; set; } = TimeSpan.FromMilliseconds(120);

    /// <summary>
    /// Gets or sets the minimum interval allowed for resize polling.
    /// </summary>
    public TimeSpan MinResizePollInterval { get; set; } = TimeSpan.FromMilliseconds(16);

    /// <summary>
    /// Gets or sets the screen options applied to the application runtime.
    /// </summary>
    public ScreenOptions Screen { get; set; } = ScreenOptions.Empty;

    /// <summary>
    /// Gets or sets the optional global theme applied by controls that support semantic theming.
    /// </summary>
    public TeaTheme? Theme { get; set; }

    /// <summary>
    /// Gets or sets optional hierarchical theme overrides applied on top of <see cref="Theme"/>.
    /// </summary>
    public TeaThemeOverrides? ThemeOverrides { get; set; }
}
