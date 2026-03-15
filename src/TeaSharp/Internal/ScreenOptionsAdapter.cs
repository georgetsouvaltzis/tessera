namespace TeaSharp.Internal;

internal static class ScreenOptionsAdapter
{
    public static global::TeaSharp.Core.Abstractions.TerminalOutput ToTerminalOutput(this ScreenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new global::TeaSharp.Core.Abstractions.TerminalOutput
        {
            AltScreen = options.AltScreen ?? false,
            EnableBracketedPaste = options.EnableBracketedPaste ?? false,
            EnableFocusReporting = options.EnableFocusReporting ?? false,
            EnableSynchronizedUpdates = options.EnableSynchronizedUpdates ?? false,
            MouseMode = options.MouseTracking switch
            {
                MouseTrackingMode.CellMotion => global::TeaSharp.Core.Abstractions.MouseMode.CellMotion,
                MouseTrackingMode.AllMotion => global::TeaSharp.Core.Abstractions.MouseMode.AllMotion,
                _ => global::TeaSharp.Core.Abstractions.MouseMode.None,
            },
            CursorColor = options.CursorColor,
            ForegroundColor = options.ForegroundColor,
            BackgroundColor = options.BackgroundColor,
            WindowTitle = options.WindowTitle,
        };
    }
}
