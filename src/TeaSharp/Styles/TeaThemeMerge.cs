namespace TeaSharp.Styles;

internal static class TeaThemeMerge
{
    public static TeaTheme Merge(TeaTheme source, TeaTheme overlay)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(overlay);

        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = source.Text.Primary.Merge(overlay.Text.Primary),
                Secondary = source.Text.Secondary.Merge(overlay.Text.Secondary),
                Muted = source.Text.Muted.Merge(overlay.Text.Muted),
                Inverse = source.Text.Inverse.Merge(overlay.Text.Inverse),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Base = source.Surface.Base.Merge(overlay.Surface.Base),
                Panel = source.Surface.Panel.Merge(overlay.Surface.Panel),
                Overlay = source.Surface.Overlay.Merge(overlay.Surface.Overlay),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = source.Border.Default.Merge(overlay.Border.Default),
                Strong = source.Border.Strong.Merge(overlay.Border.Strong),
                Focused = source.Border.Focused.Merge(overlay.Border.Focused),
                Error = source.Border.Error.Merge(overlay.Border.Error),
            },
            State = new TeaThemeStateTokens
            {
                Success = source.State.Success.Merge(overlay.State.Success),
                Warning = source.State.Warning.Merge(overlay.State.Warning),
                Error = source.State.Error.Merge(overlay.State.Error),
                Info = source.State.Info.Merge(overlay.State.Info),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = source.Accent.Primary.Merge(overlay.Accent.Primary),
                Secondary = source.Accent.Secondary.Merge(overlay.Accent.Secondary),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = source.Selection.Foreground.Merge(overlay.Selection.Foreground),
                Background = source.Selection.Background.Merge(overlay.Selection.Background),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = source.Focus.Ring.Merge(overlay.Focus.Ring),
                Title = source.Focus.Title.Merge(overlay.Focus.Title),
                Border = source.Focus.Border.Merge(overlay.Focus.Border),
            },
        };
    }
}
