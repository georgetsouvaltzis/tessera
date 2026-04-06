using Tessera.Controls;

namespace Tessera.Styles;

public static partial class TesseraThemeControlExtensions
{
    public static Badge ApplyTheme(this Badge control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TextStyle = theme.Text.Primary;
        control.FocusedTextStyle = theme.Focus.Ring;
        control.SuccessTextStyle = theme.State.Success;
        control.WarningTextStyle = theme.State.Warning;
        control.ErrorTextStyle = theme.State.Error;
        return control;
    }

    public static Badge ApplyTheme(
        this Badge control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Badge ApplyThemeDefaults(this Badge control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TextStyle = ApplyDefault(control.TextStyle, theme.Text.Primary);
        control.FocusedTextStyle = ApplyDefault(control.FocusedTextStyle, theme.Focus.Ring);
        control.SuccessTextStyle = ApplyDefault(control.SuccessTextStyle, theme.State.Success);
        control.WarningTextStyle = ApplyDefault(control.WarningTextStyle, theme.State.Warning);
        control.ErrorTextStyle = ApplyDefault(control.ErrorTextStyle, theme.State.Error);
        return control;
    }

    public static Badge ApplyThemeDefaults(
        this Badge control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static LogView ApplyTheme(this LogView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.EntryStyle = theme.Text.Primary;
        control.PausedTitleStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    public static LogView ApplyTheme(
        this LogView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static LogView ApplyThemeDefaults(this LogView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.EntryStyle = ApplyDefault(control.EntryStyle, theme.Text.Primary);
        control.PausedTitleStyle = ApplyDefault(control.PausedTitleStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static LogView ApplyThemeDefaults(
        this LogView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static MarkdownView ApplyTheme(this MarkdownView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.ContentStyle = theme.Text.Primary;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    public static MarkdownView ApplyTheme(
        this MarkdownView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static MarkdownView ApplyThemeDefaults(this MarkdownView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ContentStyle = ApplyDefault(control.ContentStyle, theme.Text.Primary);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static MarkdownView ApplyThemeDefaults(
        this MarkdownView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static MiniLog ApplyTheme(this MiniLog control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.EntryStyle = theme.Text.Primary;
        return control;
    }

    public static MiniLog ApplyTheme(
        this MiniLog control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static MiniLog ApplyThemeDefaults(this MiniLog control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.EntryStyle = ApplyDefault(control.EntryStyle, theme.Text.Primary);
        return control;
    }

    public static MiniLog ApplyThemeDefaults(
        this MiniLog control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
