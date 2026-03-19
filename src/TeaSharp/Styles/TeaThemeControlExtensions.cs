using TeaSharp.Controls;

namespace TeaSharp.Styles;

/// <summary>
/// Applies semantic theme tokens to style-enabled controls.
/// </summary>
public static class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="Button"/>.
    /// </summary>
    public static Button ApplyTheme(this Button control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.LabelStyle = theme.Text.Primary;
        control.FocusedLabelStyle = theme.Focus.Ring;
        control.DisabledLabelStyle = theme.Text.Muted;
        control.PressedLabelStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Button"/>.
    /// </summary>
    public static Button ApplyTheme(
        this Button control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="ListView{T}"/>.
    /// </summary>
    public static ListView<T> ApplyTheme<T>(this ListView<T> control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.DefaultRowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="ListView{T}"/>.
    /// </summary>
    public static ListView<T> ApplyTheme<T>(
        this ListView<T> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="StatusBar"/>.
    /// </summary>
    public static StatusBar ApplyTheme(this StatusBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.LeftTextStyle = theme.Text.Primary;
        control.RightTextStyle = theme.Text.Secondary;
        control.FillStyle = theme.Surface.Panel;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="StatusBar"/>.
    /// </summary>
    public static StatusBar ApplyTheme(
        this StatusBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="TextInput"/>.
    /// </summary>
    public static TextInput ApplyTheme(this TextInput control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.FocusedTitleStyle = theme.Focus.Title;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="TextInput"/>.
    /// </summary>
    public static TextInput ApplyTheme(
        this TextInput control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }
}
