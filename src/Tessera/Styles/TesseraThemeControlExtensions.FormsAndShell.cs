using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
/// Represents tessera theme control extensions forms and shell apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsFormsAndShellApplyExtensions
{
    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static Form ApplyTheme(this Form control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.LabelStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.RequiredMarkerStyle = theme.State.Error;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
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
    public static Form ApplyTheme(
        this Form control,
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
    public static FieldSet ApplyTheme(this FieldSet control, TesseraTheme theme)
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
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
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
    public static FieldSet ApplyTheme(
        this FieldSet control,
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
    public static SplitView ApplyTheme(this SplitView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.DividerStyle = theme.Text.Muted;
        control.FocusedDividerStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
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
    public static SplitView ApplyTheme(
        this SplitView control,
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
    public static ResizablePaneGroup ApplyTheme(this ResizablePaneGroup control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyleText = theme.Text.Secondary;
        control.FocusedTitleStyleText = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.DividerStyleText = theme.Text.Muted;
        control.FocusedDividerStyleText = theme.Focus.Ring;
        control.PaneStyleText = theme.Text.Primary;
        control.SelectedPaneStyleText = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledStyleText = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
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
    public static ResizablePaneGroup ApplyTheme(
        this ResizablePaneGroup control,
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
    public static InspectorPanel ApplyTheme(this InspectorPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.SectionStyle = theme.Text.Secondary;
        control.SelectedSectionStyle = theme.Accent.Primary;
        control.KeyStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.DetailStyle = theme.Text.Muted;
        control.MarkerStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
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
    public static InspectorPanel ApplyTheme(
        this InspectorPanel control,
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
    public static Wizard ApplyTheme(this Wizard control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.StepStyle = theme.Text.Primary;
        control.ActiveStepStyle = theme.Accent.Primary;
        control.FocusedActiveStepStyle = theme.Focus.Ring;
        control.CompletedStepStyle = theme.State.Success;
        control.PendingStepStyle = theme.Text.Secondary;
        control.HoveredStepStyle = theme.Accent.Secondary;
        control.DisabledStepStyle = theme.Text.Muted;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
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
    public static Wizard ApplyTheme(
        this Wizard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Executes apply theme t model.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme t model.</returns>
    public static DataForm<TModel> ApplyTheme<TModel>(this DataForm<TModel> control, TesseraTheme theme)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.LabelStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.PlaceholderStyle = theme.Text.Muted;
        control.SelectedFieldStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedFieldStyle = theme.Focus.Ring;
        control.HoveredFieldStyle = theme.Accent.Secondary;
        control.ReadOnlyFieldStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.ErrorStyle = theme.State.Error;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Executes apply theme t model.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme t model.</returns>
    public static DataForm<TModel> ApplyTheme<TModel>(
        this DataForm<TModel> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
/// Represents tessera theme control extensions forms and shell default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsFormsAndShellDefaultExtensions
{
    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static Form ApplyThemeDefaults(this Form control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.LabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.LabelStyle, theme.Text.Secondary);
        control.ValueStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.RequiredMarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.RequiredMarkerStyle, theme.State.Error);
        control.HoveredRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedRowStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
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
    public static Form ApplyThemeDefaults(
        this Form control,
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
    public static FieldSet ApplyThemeDefaults(this FieldSet control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedItemStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedItemStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
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
    public static FieldSet ApplyThemeDefaults(
        this FieldSet control,
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
    public static SplitView ApplyThemeDefaults(this SplitView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.DividerStyle = TesseraThemeControlExtensions.ApplyDefault(control.DividerStyle, theme.Text.Muted);
        control.FocusedDividerStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedDividerStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static SplitView ApplyThemeDefaults(
        this SplitView control,
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
    public static ResizablePaneGroup ApplyThemeDefaults(this ResizablePaneGroup control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyleText = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyleText, theme.Text.Secondary);
        control.FocusedTitleStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyleText, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.DividerStyleText = TesseraThemeControlExtensions.ApplyDefault(control.DividerStyleText, theme.Text.Muted);
        control.FocusedDividerStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedDividerStyleText, theme.Focus.Ring);
        control.PaneStyleText = TesseraThemeControlExtensions.ApplyDefault(control.PaneStyleText, theme.Text.Primary);
        control.SelectedPaneStyleText = TesseraThemeControlExtensions.ApplyDefault(control.SelectedPaneStyleText, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledStyleText = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyleText, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
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
    public static ResizablePaneGroup ApplyThemeDefaults(
        this ResizablePaneGroup control,
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
    public static InspectorPanel ApplyThemeDefaults(this InspectorPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.SectionStyle = TesseraThemeControlExtensions.ApplyDefault(control.SectionStyle, theme.Text.Secondary);
        control.SelectedSectionStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedSectionStyle, theme.Accent.Primary);
        control.KeyStyle = TesseraThemeControlExtensions.ApplyDefault(control.KeyStyle, theme.Text.Secondary);
        control.ValueStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.DetailStyle = TesseraThemeControlExtensions.ApplyDefault(control.DetailStyle, theme.Text.Muted);
        control.MarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.MarkerStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedRowStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
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
    public static InspectorPanel ApplyThemeDefaults(
        this InspectorPanel control,
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
    public static Wizard ApplyThemeDefaults(this Wizard control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.StepStyle = TesseraThemeControlExtensions.ApplyDefault(control.StepStyle, theme.Text.Primary);
        control.ActiveStepStyle = TesseraThemeControlExtensions.ApplyDefault(control.ActiveStepStyle, theme.Accent.Primary);
        control.FocusedActiveStepStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedActiveStepStyle, theme.Focus.Ring);
        control.CompletedStepStyle = TesseraThemeControlExtensions.ApplyDefault(control.CompletedStepStyle, theme.State.Success);
        control.PendingStepStyle = TesseraThemeControlExtensions.ApplyDefault(control.PendingStepStyle, theme.Text.Secondary);
        control.HoveredStepStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredStepStyle, theme.Accent.Secondary);
        control.DisabledStepStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStepStyle, theme.Text.Muted);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
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
    public static Wizard ApplyThemeDefaults(
        this Wizard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Executes apply theme defaults t model.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults t model.</returns>
    public static DataForm<TModel> ApplyThemeDefaults<TModel>(this DataForm<TModel> control, TesseraTheme theme)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.LabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.LabelStyle, theme.Text.Secondary);
        control.ValueStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.PlaceholderStyle = TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderStyle, theme.Text.Muted);
        control.SelectedFieldStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedFieldStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedFieldStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedFieldStyle, theme.Focus.Ring);
        control.HoveredFieldStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredFieldStyle, theme.Accent.Secondary);
        control.ReadOnlyFieldStyle = TesseraThemeControlExtensions.ApplyDefault(control.ReadOnlyFieldStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.ErrorStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorStyle, theme.State.Error);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Executes apply theme defaults t model.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults t model.</returns>
    public static DataForm<TModel> ApplyThemeDefaults<TModel>(
        this DataForm<TModel> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
