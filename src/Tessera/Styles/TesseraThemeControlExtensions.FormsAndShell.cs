using Tessera.Controls;

namespace Tessera.Styles;

public static partial class TesseraThemeControlExtensions
{
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

    public static Form ApplyTheme(
        this Form control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Form ApplyThemeDefaults(this Form control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.RequiredMarkerStyle = ApplyDefault(control.RequiredMarkerStyle, theme.State.Error);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(control.SelectedRowStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static Form ApplyThemeDefaults(
        this Form control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

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

    public static FieldSet ApplyTheme(
        this FieldSet control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static FieldSet ApplyThemeDefaults(this FieldSet control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = ApplyDefault(control.SelectedItemStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedItemStyle = ApplyDefault(control.FocusedSelectedItemStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static FieldSet ApplyThemeDefaults(
        this FieldSet control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

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

    public static SplitView ApplyTheme(
        this SplitView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static SplitView ApplyThemeDefaults(this SplitView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.DividerStyle = ApplyDefault(control.DividerStyle, theme.Text.Muted);
        control.FocusedDividerStyle = ApplyDefault(control.FocusedDividerStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        return control;
    }

    public static SplitView ApplyThemeDefaults(
        this SplitView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

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

    public static ResizablePaneGroup ApplyTheme(
        this ResizablePaneGroup control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static ResizablePaneGroup ApplyThemeDefaults(this ResizablePaneGroup control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyleText = ApplyDefault(control.TitleStyleText, theme.Text.Secondary);
        control.FocusedTitleStyleText = ApplyDefault(control.FocusedTitleStyleText, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.DividerStyleText = ApplyDefault(control.DividerStyleText, theme.Text.Muted);
        control.FocusedDividerStyleText = ApplyDefault(control.FocusedDividerStyleText, theme.Focus.Ring);
        control.PaneStyleText = ApplyDefault(control.PaneStyleText, theme.Text.Primary);
        control.SelectedPaneStyleText = ApplyDefault(control.SelectedPaneStyleText, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledStyleText = ApplyDefault(control.DisabledStyleText, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static ResizablePaneGroup ApplyThemeDefaults(
        this ResizablePaneGroup control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

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

    public static InspectorPanel ApplyTheme(
        this InspectorPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static InspectorPanel ApplyThemeDefaults(this InspectorPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.SectionStyle = ApplyDefault(control.SectionStyle, theme.Text.Secondary);
        control.SelectedSectionStyle = ApplyDefault(control.SelectedSectionStyle, theme.Accent.Primary);
        control.KeyStyle = ApplyDefault(control.KeyStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.DetailStyle = ApplyDefault(control.DetailStyle, theme.Text.Muted);
        control.MarkerStyle = ApplyDefault(control.MarkerStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(control.SelectedRowStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static InspectorPanel ApplyThemeDefaults(
        this InspectorPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

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

    public static Wizard ApplyTheme(
        this Wizard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Wizard ApplyThemeDefaults(this Wizard control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.StepStyle = ApplyDefault(control.StepStyle, theme.Text.Primary);
        control.ActiveStepStyle = ApplyDefault(control.ActiveStepStyle, theme.Accent.Primary);
        control.FocusedActiveStepStyle = ApplyDefault(control.FocusedActiveStepStyle, theme.Focus.Ring);
        control.CompletedStepStyle = ApplyDefault(control.CompletedStepStyle, theme.State.Success);
        control.PendingStepStyle = ApplyDefault(control.PendingStepStyle, theme.Text.Secondary);
        control.HoveredStepStyle = ApplyDefault(control.HoveredStepStyle, theme.Accent.Secondary);
        control.DisabledStepStyle = ApplyDefault(control.DisabledStepStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static Wizard ApplyThemeDefaults(
        this Wizard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

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

    public static DataForm<TModel> ApplyThemeDefaults<TModel>(this DataForm<TModel> control, TesseraTheme theme)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.PlaceholderStyle = ApplyDefault(control.PlaceholderStyle, theme.Text.Muted);
        control.SelectedFieldStyle = ApplyDefault(control.SelectedFieldStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedFieldStyle = ApplyDefault(control.FocusedSelectedFieldStyle, theme.Focus.Ring);
        control.HoveredFieldStyle = ApplyDefault(control.HoveredFieldStyle, theme.Accent.Secondary);
        control.ReadOnlyFieldStyle = ApplyDefault(control.ReadOnlyFieldStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.ErrorStyle = ApplyDefault(control.ErrorStyle, theme.State.Error);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

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
