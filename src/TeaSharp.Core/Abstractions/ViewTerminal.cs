namespace TeaSharp.Core.Abstractions;

public readonly record struct ViewTerminal
{
    public bool AltScreen { get; init; }
    public bool EnableBracketedPaste { get; init; }
    public bool EnableFocusReporting { get; init; }
    public bool EnableSynchronizedUpdates { get; init; }
    public MouseMode MouseMode { get; init; }
    public KeyboardEnhancementOptions KeyboardEnhancements { get; init; }
    public string? CursorColor { get; init; }
    public string? ForegroundColor { get; init; }
    public string? BackgroundColor { get; init; }
    public TerminalProgress? Progress { get; init; }
    public string? WindowTitle { get; init; }
}
