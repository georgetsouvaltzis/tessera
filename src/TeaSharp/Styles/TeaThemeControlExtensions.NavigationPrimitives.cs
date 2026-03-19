using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to an <see cref="Accordion"/>.
    /// </summary>
    public static Accordion ApplyTheme(this Accordion control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.ExpandedItemStyle = theme.Accent.Secondary;
        control.BodyStyle = theme.Text.Secondary;
        control.DisabledItemStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to an <see cref="Accordion"/>.
    /// </summary>
    public static Accordion ApplyTheme(
        this Accordion control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to an <see cref="Accordion"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Accordion ApplyThemeDefaults(this Accordion control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.ExpandedItemStyle = ApplyDefault(control.ExpandedItemStyle, theme.Accent.Secondary);
        control.BodyStyle = ApplyDefault(control.BodyStyle, theme.Text.Secondary);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to an <see cref="Accordion"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Accordion ApplyThemeDefaults(
        this Accordion control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="MultiSelect"/>.
    /// </summary>
    public static MultiSelect ApplyTheme(this MultiSelect control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.CheckedItemStyle = theme.Accent.Primary;
        control.DisabledItemStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="MultiSelect"/>.
    /// </summary>
    public static MultiSelect ApplyTheme(
        this MultiSelect control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="MultiSelect"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static MultiSelect ApplyThemeDefaults(this MultiSelect control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.CheckedItemStyle = ApplyDefault(control.CheckedItemStyle, theme.Accent.Primary);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="MultiSelect"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static MultiSelect ApplyThemeDefaults(
        this MultiSelect control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="RadioGroup"/>.
    /// </summary>
    public static RadioGroup ApplyTheme(this RadioGroup control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledItemStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="RadioGroup"/>.
    /// </summary>
    public static RadioGroup ApplyTheme(
        this RadioGroup control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="RadioGroup"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static RadioGroup ApplyThemeDefaults(this RadioGroup control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="RadioGroup"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static RadioGroup ApplyThemeDefaults(
        this RadioGroup control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
