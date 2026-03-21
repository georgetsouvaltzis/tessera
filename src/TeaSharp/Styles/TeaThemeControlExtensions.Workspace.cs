using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static DockWorkspace ApplyTheme(this DockWorkspace control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.PaneTitleStyle = theme.Text.Secondary;
        control.SelectedPaneTitleStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedPaneTitleStyle = theme.Focus.Ring;
        control.PaneBodyStyle = theme.Text.Primary;
        control.SelectedPaneBodyStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredPaneStyle = theme.Accent.Secondary;
        control.MutedPaneStyle = theme.Text.Muted;
        control.DisabledPaneStyle = theme.Text.Muted;
        control.PaneBorderStyleText = theme.Border.Default;
        control.FocusedPaneBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        return control;
    }

    public static DockWorkspace ApplyTheme(
        this DockWorkspace control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DockWorkspace ApplyThemeDefaults(this DockWorkspace control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.PaneTitleStyle = ApplyDefault(control.PaneTitleStyle, theme.Text.Secondary);
        control.SelectedPaneTitleStyle = ApplyDefault(control.SelectedPaneTitleStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedPaneTitleStyle = ApplyDefault(control.FocusedSelectedPaneTitleStyle, theme.Focus.Ring);
        control.PaneBodyStyle = ApplyDefault(control.PaneBodyStyle, theme.Text.Primary);
        control.SelectedPaneBodyStyle = ApplyDefault(control.SelectedPaneBodyStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredPaneStyle = ApplyDefault(control.HoveredPaneStyle, theme.Accent.Secondary);
        control.MutedPaneStyle = ApplyDefault(control.MutedPaneStyle, theme.Text.Muted);
        control.DisabledPaneStyle = ApplyDefault(control.DisabledPaneStyle, theme.Text.Muted);
        control.PaneBorderStyleText = ApplyDefault(control.PaneBorderStyleText, theme.Border.Default);
        control.FocusedPaneBorderStyleText = ApplyDefault(control.FocusedPaneBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        return control;
    }

    public static DockWorkspace ApplyThemeDefaults(
        this DockWorkspace control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static PaneTabs ApplyTheme(this PaneTabs control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TabStyle = theme.Text.Primary;
        control.SelectedTabStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedTabStyle = theme.Focus.Ring;
        control.HoveredTabStyle = theme.Accent.Secondary;
        control.DisabledTabStyle = theme.Text.Muted;
        control.SeparatorStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        return control;
    }

    public static PaneTabs ApplyTheme(
        this PaneTabs control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static PaneTabs ApplyThemeDefaults(this PaneTabs control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TabStyle = ApplyDefault(control.TabStyle, theme.Text.Primary);
        control.SelectedTabStyle = ApplyDefault(control.SelectedTabStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedTabStyle = ApplyDefault(control.FocusedSelectedTabStyle, theme.Focus.Ring);
        control.HoveredTabStyle = ApplyDefault(control.HoveredTabStyle, theme.Accent.Secondary);
        control.DisabledTabStyle = ApplyDefault(control.DisabledTabStyle, theme.Text.Muted);
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        return control;
    }

    public static PaneTabs ApplyThemeDefaults(
        this PaneTabs control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Heatmap ApplyTheme(this Heatmap control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.CellStyle = theme.Text.Primary;
        control.HoveredCellStyle = theme.Accent.Secondary;
        control.SelectedCellStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedCellStyle = theme.Focus.Ring;
        control.DisabledCellStyle = theme.Text.Muted;
        control.LowCellStyle = theme.Text.Secondary;
        control.MidCellStyle = theme.Accent.Secondary;
        control.HighCellStyle = theme.Accent.Primary;
        control.PeakCellStyle = theme.State.Success;
        control.HeaderStyle = theme.Text.Secondary;
        control.LegendStyle = theme.Text.Secondary;
        control.EmptyStyle = theme.Text.Muted;
        return control;
    }

    public static Heatmap ApplyTheme(
        this Heatmap control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Heatmap ApplyThemeDefaults(this Heatmap control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.CellStyle = ApplyDefault(control.CellStyle, theme.Text.Primary);
        control.HoveredCellStyle = ApplyDefault(control.HoveredCellStyle, theme.Accent.Secondary);
        control.SelectedCellStyle = ApplyDefault(control.SelectedCellStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedCellStyle = ApplyDefault(control.FocusedSelectedCellStyle, theme.Focus.Ring);
        control.DisabledCellStyle = ApplyDefault(control.DisabledCellStyle, theme.Text.Muted);
        control.LowCellStyle = ApplyDefault(control.LowCellStyle, theme.Text.Secondary);
        control.MidCellStyle = ApplyDefault(control.MidCellStyle, theme.Accent.Secondary);
        control.HighCellStyle = ApplyDefault(control.HighCellStyle, theme.Accent.Primary);
        control.PeakCellStyle = ApplyDefault(control.PeakCellStyle, theme.State.Success);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.LegendStyle = ApplyDefault(control.LegendStyle, theme.Text.Secondary);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        return control;
    }

    public static Heatmap ApplyThemeDefaults(
        this Heatmap control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
