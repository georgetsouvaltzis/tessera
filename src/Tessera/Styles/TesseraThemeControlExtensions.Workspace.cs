using Tessera.Controls;

namespace Tessera.Styles;

public static partial class TesseraThemeControlExtensions
{
    public static DockWorkspace ApplyTheme(this DockWorkspace control, TesseraTheme theme)
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
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    public static DockWorkspace ApplyTheme(
        this DockWorkspace control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DockWorkspace ApplyThemeDefaults(this DockWorkspace control, TesseraTheme theme)
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
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static DockWorkspace ApplyThemeDefaults(
        this DockWorkspace control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static PaneTabs ApplyTheme(this PaneTabs control, TesseraTheme theme)
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
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    public static PaneTabs ApplyTheme(
        this PaneTabs control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static PaneTabs ApplyThemeDefaults(this PaneTabs control, TesseraTheme theme)
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
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static PaneTabs ApplyThemeDefaults(
        this PaneTabs control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Heatmap ApplyTheme(this Heatmap control, TesseraTheme theme)
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
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    public static Heatmap ApplyTheme(
        this Heatmap control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Heatmap ApplyThemeDefaults(this Heatmap control, TesseraTheme theme)
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
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static Heatmap ApplyThemeDefaults(
        this Heatmap control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static ProcessListView ApplyTheme(this ProcessListView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.HeaderStyle = theme.Text.Secondary;
        control.RowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.StatusStyle = theme.Text.Secondary;
        control.MutedRowStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    public static ProcessListView ApplyTheme(
        this ProcessListView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static ProcessListView ApplyThemeDefaults(this ProcessListView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.RowStyle = ApplyDefault(control.RowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(control.SelectedRowStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.StatusStyle = ApplyDefault(control.StatusStyle, theme.Text.Secondary);
        control.MutedRowStyle = ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static ProcessListView ApplyThemeDefaults(
        this ProcessListView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static TreeMapChart ApplyTheme(this TreeMapChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.NodeStyle = theme.Text.Primary;
        control.LowNodeStyle = theme.Text.Secondary;
        control.MidNodeStyle = theme.Accent.Secondary;
        control.HighNodeStyle = theme.Accent.Primary;
        control.PeakNodeStyle = theme.State.Success;
        control.HoveredNodeStyle = theme.Accent.Secondary;
        control.SelectedNodeStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedNodeStyle = theme.Focus.Ring;
        control.DisabledNodeStyle = theme.Text.Muted;
        control.LabelStyle = theme.Text.Secondary;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    public static TreeMapChart ApplyTheme(
        this TreeMapChart control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static TreeMapChart ApplyThemeDefaults(this TreeMapChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.NodeStyle = ApplyDefault(control.NodeStyle, theme.Text.Primary);
        control.LowNodeStyle = ApplyDefault(control.LowNodeStyle, theme.Text.Secondary);
        control.MidNodeStyle = ApplyDefault(control.MidNodeStyle, theme.Accent.Secondary);
        control.HighNodeStyle = ApplyDefault(control.HighNodeStyle, theme.Accent.Primary);
        control.PeakNodeStyle = ApplyDefault(control.PeakNodeStyle, theme.State.Success);
        control.HoveredNodeStyle = ApplyDefault(control.HoveredNodeStyle, theme.Accent.Secondary);
        control.SelectedNodeStyle = ApplyDefault(control.SelectedNodeStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedNodeStyle = ApplyDefault(control.FocusedSelectedNodeStyle, theme.Focus.Ring);
        control.DisabledNodeStyle = ApplyDefault(control.DisabledNodeStyle, theme.Text.Muted);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Secondary);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static TreeMapChart ApplyThemeDefaults(
        this TreeMapChart control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static PaletteEditor ApplyTheme(this PaletteEditor control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.SwatchStyle = theme.Text.Primary;
        control.HoveredSwatchStyle = theme.Accent.Secondary;
        control.SelectedSwatchStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedSwatchStyle = theme.Focus.Ring;
        control.MutedSwatchStyle = theme.Text.Muted;
        control.PreviewSwatchStyle = theme.Accent.Primary;
        control.DisabledSwatchStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    public static PaletteEditor ApplyTheme(
        this PaletteEditor control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static PaletteEditor ApplyThemeDefaults(this PaletteEditor control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.SwatchStyle = ApplyDefault(control.SwatchStyle, theme.Text.Primary);
        control.HoveredSwatchStyle = ApplyDefault(control.HoveredSwatchStyle, theme.Accent.Secondary);
        control.SelectedSwatchStyle = ApplyDefault(control.SelectedSwatchStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedSwatchStyle = ApplyDefault(control.FocusedSelectedSwatchStyle, theme.Focus.Ring);
        control.MutedSwatchStyle = ApplyDefault(control.MutedSwatchStyle, theme.Text.Muted);
        control.PreviewSwatchStyle = ApplyDefault(control.PreviewSwatchStyle, theme.Accent.Primary);
        control.DisabledSwatchStyle = ApplyDefault(control.DisabledSwatchStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static PaletteEditor ApplyThemeDefaults(
        this PaletteEditor control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static TerminalPanel ApplyTheme(this TerminalPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.StandardOutputStyle = theme.Text.Primary;
        control.StandardErrorStyle = theme.State.Error;
        control.CommandStyle = theme.Accent.Primary;
        control.SystemStyle = theme.Text.Secondary;
        control.MarkerStyle = theme.Text.Secondary;
        control.HoveredLineStyle = theme.Accent.Secondary;
        control.SelectedLineStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedLineStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyStyle = theme.Text.Muted;
        return control;
    }

    public static TerminalPanel ApplyTheme(
        this TerminalPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static TerminalPanel ApplyThemeDefaults(this TerminalPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.StandardOutputStyle = ApplyDefault(control.StandardOutputStyle, theme.Text.Primary);
        control.StandardErrorStyle = ApplyDefault(control.StandardErrorStyle, theme.State.Error);
        control.CommandStyle = ApplyDefault(control.CommandStyle, theme.Accent.Primary);
        control.SystemStyle = ApplyDefault(control.SystemStyle, theme.Text.Secondary);
        control.MarkerStyle = ApplyDefault(control.MarkerStyle, theme.Text.Secondary);
        control.HoveredLineStyle = ApplyDefault(control.HoveredLineStyle, theme.Accent.Secondary);
        control.SelectedLineStyle = ApplyDefault(control.SelectedLineStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedLineStyle = ApplyDefault(control.FocusedSelectedLineStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        return control;
    }

    public static TerminalPanel ApplyThemeDefaults(
        this TerminalPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
