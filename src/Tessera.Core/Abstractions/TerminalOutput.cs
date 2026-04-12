namespace Tessera.Core.Abstractions;

/// <summary>
/// Describes terminal-side mode changes and metadata updates to apply with a frame.
/// </summary>
public readonly record struct TerminalOutput
{
    /// <summary>
    /// Enters the terminal alternate screen buffer when <see langword="true" />.
    /// </summary>
    public bool AltScreen { get; init; }

    /// <summary>
    /// Enables bracketed paste reporting when supported.
    /// </summary>
    public bool EnableBracketedPaste { get; init; }

    /// <summary>
    /// Enables focus-in and focus-out reporting when supported.
    /// </summary>
    public bool EnableFocusReporting { get; init; }

    /// <summary>
    /// Enables synchronized terminal updates when supported.
    /// </summary>
    public bool EnableSynchronizedUpdates { get; init; }

    /// <summary>
    /// Gets the requested mouse reporting mode.
    /// </summary>
    public MouseMode MouseMode { get; init; }

    /// <summary>
    /// Gets requested keyboard protocol enhancements.
    /// </summary>
    public KeyboardEnhancementOptions KeyboardEnhancements { get; init; }

    /// <summary>
    /// Gets the requested cursor color.
    /// </summary>
    public string? CursorColor { get; init; }

    /// <summary>
    /// Gets the requested terminal foreground color.
    /// </summary>
    public string? ForegroundColor { get; init; }

    /// <summary>
    /// Gets the requested terminal background color.
    /// </summary>
    public string? BackgroundColor { get; init; }

    /// <summary>
    /// Gets the requested taskbar or dock progress state.
    /// </summary>
    public TerminalProgress? Progress { get; init; }

    /// <summary>
    /// Gets the requested terminal window title.
    /// </summary>
    public string? WindowTitle { get; init; }

    /// <summary>
    /// Gets the requested terminal font specification string.
    /// </summary>
    public string? FontSpec { get; init; }

    /// <summary>
    /// Gets the requested terminal font family.
    /// </summary>
    public string? FontFamily { get; init; }

    /// <summary>
    /// Gets the requested terminal font size.
    /// </summary>
    public int? FontSize { get; init; }

    /// <summary>
    /// Gets the requested iTerm2 profile name.
    /// </summary>
    public string? Iterm2Profile { get; init; }
}
