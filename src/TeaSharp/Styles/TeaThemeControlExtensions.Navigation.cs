using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="Breadcrumb"/>.
    /// </summary>
    public static Breadcrumb ApplyTheme(this Breadcrumb control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.SeparatorStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Breadcrumb"/>.
    /// </summary>
    public static Breadcrumb ApplyTheme(
        this Breadcrumb control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Breadcrumb"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Breadcrumb ApplyThemeDefaults(this Breadcrumb control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Breadcrumb"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Breadcrumb ApplyThemeDefaults(
        this Breadcrumb control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Paginator"/>.
    /// </summary>
    public static Paginator ApplyTheme(this Paginator control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.LabelStyle = theme.Text.Primary;
        control.ActivePageLabelStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledNavigationLabelStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Paginator"/>.
    /// </summary>
    public static Paginator ApplyTheme(
        this Paginator control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Paginator"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Paginator ApplyThemeDefaults(this Paginator control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.ActivePageLabelStyle = ApplyDefault(
            control.ActivePageLabelStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledNavigationLabelStyle = ApplyDefault(control.DisabledNavigationLabelStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Paginator"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Paginator ApplyThemeDefaults(
        this Paginator control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    public static Toolbar ApplyTheme(this Toolbar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedItemStyle = theme.Focus.Ring;
        control.SeparatorStyle = theme.Text.Muted;
        return control;
    }

    public static Toolbar ApplyTheme(
        this Toolbar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Toolbar ApplyThemeDefaults(this Toolbar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedItemStyle = ApplyDefault(control.FocusedItemStyle, theme.Focus.Ring);
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        return control;
    }

    public static Toolbar ApplyThemeDefaults(
        this Toolbar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static CommandBar ApplyTheme(this CommandBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledItemStyle = theme.Text.Muted;
        control.SeparatorStyle = theme.Text.Muted;
        return control;
    }

    public static CommandBar ApplyTheme(
        this CommandBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static CommandBar ApplyThemeDefaults(this CommandBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        return control;
    }

    public static CommandBar ApplyThemeDefaults(
        this CommandBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static SearchBox ApplyTheme(this SearchBox control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.MatchCounterStyle = theme.Text.Secondary;
        control.MatchHighlightStyle = theme.Accent.Primary;
        control.NavigationLabelStyle = theme.Accent.Secondary;
        control.DisabledNavigationLabelStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    public static SearchBox ApplyTheme(
        this SearchBox control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static SearchBox ApplyThemeDefaults(this SearchBox control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.MatchCounterStyle = ApplyDefault(control.MatchCounterStyle, theme.Text.Secondary);
        control.MatchHighlightStyle = ApplyDefault(control.MatchHighlightStyle, theme.Accent.Primary);
        control.NavigationLabelStyle = ApplyDefault(control.NavigationLabelStyle, theme.Accent.Secondary);
        control.DisabledNavigationLabelStyle = ApplyDefault(control.DisabledNavigationLabelStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static SearchBox ApplyThemeDefaults(
        this SearchBox control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
