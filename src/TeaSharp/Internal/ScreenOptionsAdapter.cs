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
            FontSpec = options.FontSpec,
            FontFamily = options.FontFamily,
            FontSize = options.FontSize,
            Iterm2Profile = options.Iterm2Profile,
        };
    }

    public static ScreenOptions ToScreenOptions(this global::TeaSharp.Core.Abstractions.TerminalOutput output)
    {
        return new ScreenOptions
        {
            AltScreen = output.AltScreen,
            EnableBracketedPaste = output.EnableBracketedPaste,
            EnableFocusReporting = output.EnableFocusReporting,
            EnableSynchronizedUpdates = output.EnableSynchronizedUpdates,
            MouseTracking = output.MouseMode switch
            {
                global::TeaSharp.Core.Abstractions.MouseMode.CellMotion => MouseTrackingMode.CellMotion,
                global::TeaSharp.Core.Abstractions.MouseMode.AllMotion => MouseTrackingMode.AllMotion,
                _ => MouseTrackingMode.None,
            },
            CursorColor = output.CursorColor,
            ForegroundColor = output.ForegroundColor,
            BackgroundColor = output.BackgroundColor,
            WindowTitle = output.WindowTitle,
            FontSpec = output.FontSpec,
            FontFamily = output.FontFamily,
            FontSize = output.FontSize,
            Iterm2Profile = output.Iterm2Profile,
        };
    }
}
