namespace TeaSharp.Styles;

internal static class TeaThemeMerge
{
    public static TeaTheme Merge(TeaTheme source, TeaTheme overlay)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(overlay);

        if (IsEmpty(overlay))
        {
            return source;
        }

        return new TeaTheme
        {
            Text = IsEmpty(overlay.Text)
                ? source.Text
                : new TeaThemeTextTokens
                {
                    Primary = source.Text.Primary.Merge(overlay.Text.Primary),
                    Secondary = source.Text.Secondary.Merge(overlay.Text.Secondary),
                    Muted = source.Text.Muted.Merge(overlay.Text.Muted),
                    Inverse = source.Text.Inverse.Merge(overlay.Text.Inverse),
                },
            Surface = IsEmpty(overlay.Surface)
                ? source.Surface
                : new TeaThemeSurfaceTokens
                {
                    Base = source.Surface.Base.Merge(overlay.Surface.Base),
                    Panel = source.Surface.Panel.Merge(overlay.Surface.Panel),
                    Overlay = source.Surface.Overlay.Merge(overlay.Surface.Overlay),
                },
            Border = IsEmpty(overlay.Border)
                ? source.Border
                : new TeaThemeBorderTokens
                {
                    Default = source.Border.Default.Merge(overlay.Border.Default),
                    Strong = source.Border.Strong.Merge(overlay.Border.Strong),
                    Focused = source.Border.Focused.Merge(overlay.Border.Focused),
                    Error = source.Border.Error.Merge(overlay.Border.Error),
                },
            State = IsEmpty(overlay.State)
                ? source.State
                : new TeaThemeStateTokens
                {
                    Success = source.State.Success.Merge(overlay.State.Success),
                    Warning = source.State.Warning.Merge(overlay.State.Warning),
                    Error = source.State.Error.Merge(overlay.State.Error),
                    Info = source.State.Info.Merge(overlay.State.Info),
                },
            Accent = IsEmpty(overlay.Accent)
                ? source.Accent
                : new TeaThemeAccentTokens
                {
                    Primary = source.Accent.Primary.Merge(overlay.Accent.Primary),
                    Secondary = source.Accent.Secondary.Merge(overlay.Accent.Secondary),
                },
            Selection = IsEmpty(overlay.Selection)
                ? source.Selection
                : new TeaThemeSelectionTokens
                {
                    Foreground = source.Selection.Foreground.Merge(overlay.Selection.Foreground),
                    Background = source.Selection.Background.Merge(overlay.Selection.Background),
                },
            Focus = IsEmpty(overlay.Focus)
                ? source.Focus
                : new TeaThemeFocusTokens
                {
                    Ring = source.Focus.Ring.Merge(overlay.Focus.Ring),
                    Title = source.Focus.Title.Merge(overlay.Focus.Title),
                    Border = source.Focus.Border.Merge(overlay.Focus.Border),
                },
        };
    }

    private static bool IsEmpty(TeaTheme theme)
    {
        return IsEmpty(theme.Text)
            && IsEmpty(theme.Surface)
            && IsEmpty(theme.Border)
            && IsEmpty(theme.State)
            && IsEmpty(theme.Accent)
            && IsEmpty(theme.Selection)
            && IsEmpty(theme.Focus);
    }

    private static bool IsEmpty(TeaThemeTextTokens tokens) =>
        tokens.Primary.IsEmpty
        && tokens.Secondary.IsEmpty
        && tokens.Muted.IsEmpty
        && tokens.Inverse.IsEmpty;

    private static bool IsEmpty(TeaThemeSurfaceTokens tokens) =>
        tokens.Base.IsEmpty
        && tokens.Panel.IsEmpty
        && tokens.Overlay.IsEmpty;

    private static bool IsEmpty(TeaThemeBorderTokens tokens) =>
        tokens.Default.IsEmpty
        && tokens.Strong.IsEmpty
        && tokens.Focused.IsEmpty
        && tokens.Error.IsEmpty;

    private static bool IsEmpty(TeaThemeStateTokens tokens) =>
        tokens.Success.IsEmpty
        && tokens.Warning.IsEmpty
        && tokens.Error.IsEmpty
        && tokens.Info.IsEmpty;

    private static bool IsEmpty(TeaThemeAccentTokens tokens) =>
        tokens.Primary.IsEmpty
        && tokens.Secondary.IsEmpty;

    private static bool IsEmpty(TeaThemeSelectionTokens tokens) =>
        tokens.Foreground.IsEmpty
        && tokens.Background.IsEmpty;

    private static bool IsEmpty(TeaThemeFocusTokens tokens) =>
        tokens.Ring.IsEmpty
        && tokens.Title.IsEmpty
        && tokens.Border.IsEmpty;
}
