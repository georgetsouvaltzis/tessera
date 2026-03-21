using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static Form ApplyTheme(this Form control, TeaTheme theme)
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
        return control;
    }

    public static Form ApplyTheme(
        this Form control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Form ApplyThemeDefaults(this Form control, TeaTheme theme)
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
        return control;
    }

    public static Form ApplyThemeDefaults(
        this Form control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static FieldSet ApplyTheme(this FieldSet control, TeaTheme theme)
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
        return control;
    }

    public static FieldSet ApplyTheme(
        this FieldSet control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static FieldSet ApplyThemeDefaults(this FieldSet control, TeaTheme theme)
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
        return control;
    }

    public static FieldSet ApplyThemeDefaults(
        this FieldSet control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static SplitView ApplyTheme(this SplitView control, TeaTheme theme)
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
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static SplitView ApplyThemeDefaults(this SplitView control, TeaTheme theme)
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
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static InspectorPanel ApplyTheme(this InspectorPanel control, TeaTheme theme)
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
        return control;
    }

    public static InspectorPanel ApplyTheme(
        this InspectorPanel control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static InspectorPanel ApplyThemeDefaults(this InspectorPanel control, TeaTheme theme)
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
        return control;
    }

    public static InspectorPanel ApplyThemeDefaults(
        this InspectorPanel control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Wizard ApplyTheme(this Wizard control, TeaTheme theme)
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
        return control;
    }

    public static Wizard ApplyTheme(
        this Wizard control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Wizard ApplyThemeDefaults(this Wizard control, TeaTheme theme)
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
        return control;
    }

    public static Wizard ApplyThemeDefaults(
        this Wizard control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static DataForm<TModel> ApplyTheme<TModel>(this DataForm<TModel> control, TeaTheme theme)
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
        return control;
    }

    public static DataForm<TModel> ApplyTheme<TModel>(
        this DataForm<TModel> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DataForm<TModel> ApplyThemeDefaults<TModel>(this DataForm<TModel> control, TeaTheme theme)
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
        return control;
    }

    public static DataForm<TModel> ApplyThemeDefaults<TModel>(
        this DataForm<TModel> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
