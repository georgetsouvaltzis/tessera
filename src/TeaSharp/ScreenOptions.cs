namespace TeaSharp;

public sealed class ScreenOptions
{
    public static ScreenOptions Empty { get; } = new();

    public bool? AltScreen { get; init; }

    public bool? EnableBracketedPaste { get; init; }

    public bool? EnableFocusReporting { get; init; }

    public bool? EnableSynchronizedUpdates { get; init; }

    public MouseTrackingMode? MouseTracking { get; init; }

    public string? CursorColor { get; init; }

    public string? ForegroundColor { get; init; }

    public string? BackgroundColor { get; init; }

    public string? WindowTitle { get; init; }

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
        };
    }

    internal global::TeaSharp.Core.Abstractions.TerminalOutput ToTerminalOutput()
    {
        return new global::TeaSharp.Core.Abstractions.TerminalOutput
        {
            AltScreen = AltScreen ?? false,
            EnableBracketedPaste = EnableBracketedPaste ?? false,
            EnableFocusReporting = EnableFocusReporting ?? false,
            EnableSynchronizedUpdates = EnableSynchronizedUpdates ?? false,
            MouseMode = MouseTracking switch
            {
                MouseTrackingMode.CellMotion => global::TeaSharp.Core.Abstractions.MouseMode.CellMotion,
                MouseTrackingMode.AllMotion => global::TeaSharp.Core.Abstractions.MouseMode.AllMotion,
                _ => global::TeaSharp.Core.Abstractions.MouseMode.None,
            },
            CursorColor = CursorColor,
            ForegroundColor = ForegroundColor,
            BackgroundColor = BackgroundColor,
            WindowTitle = WindowTitle,
        };
    }
}
