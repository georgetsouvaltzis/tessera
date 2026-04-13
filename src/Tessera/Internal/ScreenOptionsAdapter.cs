using Tessera.Core.Abstractions;

namespace Tessera.Internal;

internal static class ScreenOptionsAdapter
{
    public static TerminalOutput ToTerminalOutput(this ScreenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new TerminalOutput
        {
            AltScreen = options.AltScreen ?? false,
            EnableBracketedPaste = options.EnableBracketedPaste ?? false,
            EnableFocusReporting = options.EnableFocusReporting ?? false,
            EnableSynchronizedUpdates = options.EnableSynchronizedUpdates ?? false,
            MouseMode = options.MouseTracking switch
            {
                MouseTrackingMode.CellMotion => MouseMode.CellMotion,
                MouseTrackingMode.AllMotion => MouseMode.AllMotion,
                _ => MouseMode.None
            },
            CursorColor = options.CursorColor,
            ForegroundColor = options.ForegroundColor,
            BackgroundColor = options.BackgroundColor,
            WindowTitle = options.WindowTitle,
            FontSpec = options.FontSpec,
            FontFamily = options.FontFamily,
            FontSize = options.FontSize,
            Iterm2Profile = options.Iterm2Profile
        };
    }

    public static ScreenOptions ToScreenOptions(this TerminalOutput output)
    {
        return new ScreenOptions
        {
            AltScreen = output.AltScreen,
            EnableBracketedPaste = output.EnableBracketedPaste,
            EnableFocusReporting = output.EnableFocusReporting,
            EnableSynchronizedUpdates = output.EnableSynchronizedUpdates,
            MouseTracking = output.MouseMode switch
            {
                MouseMode.CellMotion => MouseTrackingMode.CellMotion,
                MouseMode.AllMotion => MouseTrackingMode.AllMotion,
                _ => MouseTrackingMode.None
            },
            CursorColor = output.CursorColor,
            ForegroundColor = output.ForegroundColor,
            BackgroundColor = output.BackgroundColor,
            WindowTitle = output.WindowTitle,
            FontSpec = output.FontSpec,
            FontFamily = output.FontFamily,
            FontSize = output.FontSize,
            Iterm2Profile = output.Iterm2Profile
        };
    }
}
