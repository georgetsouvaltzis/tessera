using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
/// Represents tessera theme control extensions navigation apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsNavigationApplyExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="Breadcrumb"/>.
    /// </summary>
    public static Breadcrumb ApplyTheme(this Breadcrumb control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
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
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Paginator"/>.
    /// </summary>
    public static Paginator ApplyTheme(this Paginator control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
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
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static Toolbar ApplyTheme(this Toolbar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedItemStyle = theme.Focus.Ring;
        control.SeparatorStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static Toolbar ApplyTheme(
        this Toolbar control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static CommandBar ApplyTheme(this CommandBar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.ItemStyle = theme.Text.Primary;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledItemStyle = theme.Text.Muted;
        control.SeparatorStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static CommandBar ApplyTheme(
        this CommandBar control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static SearchBox ApplyTheme(this SearchBox control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
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

    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static SearchBox ApplyTheme(
        this SearchBox control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="SearchResultsView"/>.
    /// </summary>
    public static SearchResultsView ApplyTheme(this SearchResultsView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.DefaultRowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.PressedRowStyle = theme.Focus.Ring.Merge(theme.Selection.Foreground);
        control.DisabledRowStyle = theme.Text.Muted;
        control.ErrorRowStyle = theme.State.Error;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="SearchResultsView"/>.
    /// </summary>
    public static SearchResultsView ApplyTheme(
        this SearchResultsView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="JumpList"/>.
    /// </summary>
    public static JumpList ApplyTheme(this JumpList control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.ItemStyle = theme.Text.Primary;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedItemStyle = theme.Focus.Ring;
        control.DisabledItemStyle = theme.Text.Muted;
        control.PinnedMarkerStyle = theme.Accent.Primary;
        control.RecentMarkerStyle = theme.Accent.Secondary;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="JumpList"/>.
    /// </summary>
    public static JumpList ApplyTheme(
        this JumpList control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="SideNavRail"/>.
    /// </summary>
    public static SideNavRail ApplyTheme(this SideNavRail control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedItemStyle = theme.Focus.Ring;
        control.DisabledItemStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="SideNavRail"/>.
    /// </summary>
    public static SideNavRail ApplyTheme(
        this SideNavRail control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
/// Represents tessera theme control extensions navigation default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsNavigationDefaultExtensions
{
    /// <summary>
    /// Applies theme defaults to a <see cref="Breadcrumb"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Breadcrumb ApplyThemeDefaults(this Breadcrumb control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.SeparatorStyle = TesseraThemeControlExtensions.ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Breadcrumb"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Breadcrumb ApplyThemeDefaults(
        this Breadcrumb control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Paginator"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Paginator ApplyThemeDefaults(this Paginator control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.LabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.ActivePageLabelStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.ActivePageLabelStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledNavigationLabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledNavigationLabelStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Paginator"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Paginator ApplyThemeDefaults(
        this Paginator control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static Toolbar ApplyThemeDefaults(this Toolbar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedItemStyle, theme.Focus.Ring);
        control.SeparatorStyle = TesseraThemeControlExtensions.ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static Toolbar ApplyThemeDefaults(
        this Toolbar control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static CommandBar ApplyThemeDefaults(this CommandBar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.SeparatorStyle = TesseraThemeControlExtensions.ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static CommandBar ApplyThemeDefaults(
        this CommandBar control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static SearchBox ApplyThemeDefaults(this SearchBox control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.ValueTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.MatchCounterStyle = TesseraThemeControlExtensions.ApplyDefault(control.MatchCounterStyle, theme.Text.Secondary);
        control.MatchHighlightStyle = TesseraThemeControlExtensions.ApplyDefault(control.MatchHighlightStyle, theme.Accent.Primary);
        control.NavigationLabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.NavigationLabelStyle, theme.Accent.Secondary);
        control.DisabledNavigationLabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledNavigationLabelStyle, theme.Text.Muted);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static SearchBox ApplyThemeDefaults(
        this SearchBox control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="SearchResultsView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static SearchResultsView ApplyThemeDefaults(this SearchResultsView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.DefaultRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.DefaultRowStyle, theme.Text.Primary);
        control.HoveredRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.PressedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.PressedRowStyle, theme.Focus.Ring.Merge(theme.Selection.Foreground));
        control.DisabledRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.ErrorRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorRowStyle, theme.State.Error);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(
            control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="SearchResultsView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static SearchResultsView ApplyThemeDefaults(
        this SearchResultsView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="JumpList"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static JumpList ApplyThemeDefaults(this JumpList control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedItemStyle, theme.Focus.Ring);
        control.DisabledItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.PinnedMarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.PinnedMarkerStyle, theme.Accent.Primary);
        control.RecentMarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.RecentMarkerStyle, theme.Accent.Secondary);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="JumpList"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static JumpList ApplyThemeDefaults(
        this JumpList control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="SideNavRail"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static SideNavRail ApplyThemeDefaults(this SideNavRail control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedItemStyle, theme.Focus.Ring);
        control.DisabledItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(
            control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="SideNavRail"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static SideNavRail ApplyThemeDefaults(
        this SideNavRail control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
