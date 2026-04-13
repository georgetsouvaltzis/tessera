using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
///     Represents tessera theme control extensions navigation primitives apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsNavigationPrimitivesApplyExtensions
{
    /// <summary>
    ///     Applies a resolved theme to an <see cref="Accordion" />.
    /// </summary>
    public static Accordion ApplyTheme(this Accordion control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.ExpandedItemStyle = theme.Accent.Secondary;
        control.BodyStyle = theme.Text.Secondary;
        control.DisabledItemStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to an <see cref="Accordion" />.
    /// </summary>
    public static Accordion ApplyTheme(
        this Accordion control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="MultiSelect" />.
    /// </summary>
    public static MultiSelect ApplyTheme(this MultiSelect control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.CheckedItemStyle = theme.Accent.Primary;
        control.DisabledItemStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="MultiSelect" />.
    /// </summary>
    public static MultiSelect ApplyTheme(
        this MultiSelect control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="RadioGroup" />.
    /// </summary>
    public static RadioGroup ApplyTheme(this RadioGroup control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledItemStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="RadioGroup" />.
    /// </summary>
    public static RadioGroup ApplyTheme(
        this RadioGroup control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
///     Represents tessera theme control extensions navigation primitives default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsNavigationPrimitivesDefaultExtensions
{
    /// <summary>
    ///     Applies theme defaults to an <see cref="Accordion" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static Accordion ApplyThemeDefaults(this Accordion control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.ExpandedItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.ExpandedItemStyle, theme.Accent.Secondary);
        control.BodyStyle = TesseraThemeControlExtensions.ApplyDefault(control.BodyStyle, theme.Text.Secondary);
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to an <see cref="Accordion" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static Accordion ApplyThemeDefaults(
        this Accordion control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="MultiSelect" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static MultiSelect ApplyThemeDefaults(this MultiSelect control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.CheckedItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.CheckedItemStyle, theme.Accent.Primary);
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="MultiSelect" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static MultiSelect ApplyThemeDefaults(
        this MultiSelect control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="RadioGroup" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static RadioGroup ApplyThemeDefaults(this RadioGroup control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="RadioGroup" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static RadioGroup ApplyThemeDefaults(
        this RadioGroup control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
