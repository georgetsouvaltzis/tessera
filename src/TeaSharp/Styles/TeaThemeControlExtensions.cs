using TeaSharp.Controls;

namespace TeaSharp.Styles;

/// <summary>
/// Applies semantic theme tokens to style-enabled controls.
/// </summary>
public static class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies theme defaults to a <see cref="Button"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Button ApplyThemeDefaults(this Button control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.FocusedLabelStyle = ApplyDefault(control.FocusedLabelStyle, theme.Focus.Ring);
        control.DisabledLabelStyle = ApplyDefault(control.DisabledLabelStyle, theme.Text.Muted);
        control.PressedLabelStyle = ApplyDefault(
            control.PressedLabelStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Button"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Button ApplyThemeDefaults(
        this Button control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

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
    /// Applies theme defaults to a <see cref="ListView{T}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ListView<T> ApplyThemeDefaults<T>(this ListView<T> control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.DefaultRowStyle = ApplyDefault(control.DefaultRowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="ListView{T}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ListView<T> ApplyThemeDefaults<T>(
        this ListView<T> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
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
    /// Applies theme defaults to a <see cref="StatusBar"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static StatusBar ApplyThemeDefaults(this StatusBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.LeftTextStyle = ApplyDefault(control.LeftTextStyle, theme.Text.Primary);
        control.RightTextStyle = ApplyDefault(control.RightTextStyle, theme.Text.Secondary);
        control.FillStyle = ApplyDefault(control.FillStyle, theme.Surface.Panel);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="StatusBar"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static StatusBar ApplyThemeDefaults(
        this StatusBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
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
    /// Applies theme defaults to a <see cref="TextInput"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TextInput ApplyThemeDefaults(this TextInput control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="TextInput"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TextInput ApplyThemeDefaults(
        this TextInput control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
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

    /// <summary>
    /// Applies a resolved theme to a <see cref="Table"/>.
    /// </summary>
    public static Table ApplyTheme(this Table control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Table"/>.
    /// </summary>
    public static Table ApplyTheme(
        this Table control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Table"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Table ApplyThemeDefaults(this Table control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Table"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Table ApplyThemeDefaults(
        this Table control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Tabs"/>.
    /// </summary>
    public static Tabs ApplyTheme(this Tabs control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Tabs"/>.
    /// </summary>
    public static Tabs ApplyTheme(
        this Tabs control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Tabs"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Tabs ApplyThemeDefaults(this Tabs control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Tabs"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Tabs ApplyThemeDefaults(
        this Tabs control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

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

    public static DiffView ApplyTheme(this DiffView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.AddedLineStyle = theme.State.Success;
        control.RemovedLineStyle = theme.State.Error;
        control.UnchangedLineStyle = theme.Text.Primary;
        control.SelectedLineStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        return control;
    }

    public static DiffView ApplyTheme(
        this DiffView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DiffView ApplyThemeDefaults(this DiffView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.AddedLineStyle = ApplyDefault(control.AddedLineStyle, theme.State.Success);
        control.RemovedLineStyle = ApplyDefault(control.RemovedLineStyle, theme.State.Error);
        control.UnchangedLineStyle = ApplyDefault(control.UnchangedLineStyle, theme.Text.Primary);
        control.SelectedLineStyle = ApplyDefault(
            control.SelectedLineStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        return control;
    }

    public static DiffView ApplyThemeDefaults(
        this DiffView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static PropertyGrid ApplyTheme(this PropertyGrid control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.KeyStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        return control;
    }

    public static PropertyGrid ApplyTheme(
        this PropertyGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static PropertyGrid ApplyThemeDefaults(this PropertyGrid control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.KeyStyle = ApplyDefault(control.KeyStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        return control;
    }

    public static PropertyGrid ApplyThemeDefaults(
        this PropertyGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    private static TeaStyle ApplyDefault(TeaStyle current, TeaStyle fallback)
    {
        return current.IsEmpty ? fallback : current;
    }
}
