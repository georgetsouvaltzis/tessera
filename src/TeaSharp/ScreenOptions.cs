namespace TeaSharp;

/// <summary>
/// Configures terminal behavior for a rendered screen.
/// </summary>
/// <remarks>
/// These options describe terminal-facing behavior for rendered output, such as alternate screen usage,
/// mouse tracking, focus reporting, and default colors. Use <see cref="TeaRuntimeOptions"/> for runtime-loop
/// behavior instead.
/// </remarks>
public sealed class ScreenOptions
{
    /// <summary>
    /// Gets an empty set of screen options.
    /// </summary>
    public static ScreenOptions Empty { get; } = new();

    /// <summary>
    /// Gets or sets whether the screen should use the terminal alternate buffer.
    /// </summary>
    public bool? AltScreen { get; init; }

    /// <summary>
    /// Gets or sets whether bracketed paste mode should be enabled.
    /// </summary>
    public bool? EnableBracketedPaste { get; init; }

    /// <summary>
    /// Gets or sets whether terminal focus reporting should be enabled.
    /// </summary>
    public bool? EnableFocusReporting { get; init; }

    /// <summary>
    /// Gets or sets whether synchronized terminal updates should be enabled.
    /// </summary>
    public bool? EnableSynchronizedUpdates { get; init; }

    /// <summary>
    /// Gets or sets the mouse tracking mode for the screen.
    /// </summary>
    public MouseTrackingMode? MouseTracking { get; init; }

    /// <summary>
    /// Gets or sets the terminal cursor color.
    /// </summary>
    public string? CursorColor { get; init; }

    /// <summary>
    /// Gets or sets the default foreground color.
    /// </summary>
    public string? ForegroundColor { get; init; }

    /// <summary>
    /// Gets or sets the default background color.
    /// </summary>
    public string? BackgroundColor { get; init; }

    /// <summary>
    /// Gets or sets the terminal window title.
    /// </summary>
    public string? WindowTitle { get; init; }

    /// <summary>
    /// Gets or sets an experimental best-effort terminal font request.
    /// </summary>
    /// <remarks>
    /// This is terminal-dependent and optional. Set this explicitly to opt in.
    /// TeaSharp emits an OSC 50 request when supported by the terminal, but does not guarantee application.
    /// </remarks>
    public string? FontSpec { get; init; }

    internal ScreenOptions Merge(ScreenOptions? overrides)
    {
        if (overrides is null)
        {
            return this;
        }

        return new ScreenOptions
        {
            AltScreen = overrides.AltScreen ?? AltScreen,
            EnableBracketedPaste = overrides.EnableBracketedPaste ?? EnableBracketedPaste,
            EnableFocusReporting = overrides.EnableFocusReporting ?? EnableFocusReporting,
            EnableSynchronizedUpdates = overrides.EnableSynchronizedUpdates ?? EnableSynchronizedUpdates,
            MouseTracking = overrides.MouseTracking ?? MouseTracking,
            CursorColor = overrides.CursorColor ?? CursorColor,
            ForegroundColor = overrides.ForegroundColor ?? ForegroundColor,
            BackgroundColor = overrides.BackgroundColor ?? BackgroundColor,
            WindowTitle = overrides.WindowTitle ?? WindowTitle,
            FontSpec = overrides.FontSpec ?? FontSpec,
        };
    }

}
