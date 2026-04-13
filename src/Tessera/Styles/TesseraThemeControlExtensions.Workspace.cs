using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
///     Represents tessera theme control extensions workspace apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsWorkspaceApplyExtensions
{
    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static DockWorkspace ApplyTheme(
        this DockWorkspace control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static PaneTabs ApplyTheme(
        this PaneTabs control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static Heatmap ApplyTheme(
        this Heatmap control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static ProcessListView ApplyTheme(
        this ProcessListView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static TreeMapChart ApplyTheme(
        this TreeMapChart control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static PaletteEditor ApplyTheme(
        this PaletteEditor control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
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

    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme.</returns>
    public static TerminalPanel ApplyTheme(
        this TerminalPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
///     Represents tessera theme control extensions workspace default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsWorkspaceDefaultExtensions
{
    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static DockWorkspace ApplyThemeDefaults(this DockWorkspace control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.PaneTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PaneTitleStyle, theme.Text.Secondary);
        control.SelectedPaneTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedPaneTitleStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedPaneTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedPaneTitleStyle, theme.Focus.Ring);
        control.PaneBodyStyle = TesseraThemeControlExtensions.ApplyDefault(control.PaneBodyStyle, theme.Text.Primary);
        control.SelectedPaneBodyStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedPaneBodyStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredPaneStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredPaneStyle, theme.Accent.Secondary);
        control.MutedPaneStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedPaneStyle, theme.Text.Muted);
        control.DisabledPaneStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledPaneStyle, theme.Text.Muted);
        control.PaneBorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.PaneBorderStyleText, theme.Border.Default);
        control.FocusedPaneBorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedPaneBorderStyleText,
                theme.Border.Focused.Merge(theme.Focus.Border));
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static DockWorkspace ApplyThemeDefaults(
        this DockWorkspace control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static PaneTabs ApplyThemeDefaults(this PaneTabs control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TabStyle = TesseraThemeControlExtensions.ApplyDefault(control.TabStyle, theme.Text.Primary);
        control.SelectedTabStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedTabStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedTabStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedTabStyle, theme.Focus.Ring);
        control.HoveredTabStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredTabStyle, theme.Accent.Secondary);
        control.DisabledTabStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledTabStyle, theme.Text.Muted);
        control.SeparatorStyle = TesseraThemeControlExtensions.ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static PaneTabs ApplyThemeDefaults(
        this PaneTabs control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static Heatmap ApplyThemeDefaults(this Heatmap control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.CellStyle = TesseraThemeControlExtensions.ApplyDefault(control.CellStyle, theme.Text.Primary);
        control.HoveredCellStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredCellStyle, theme.Accent.Secondary);
        control.SelectedCellStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedCellStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedCellStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedCellStyle, theme.Focus.Ring);
        control.DisabledCellStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledCellStyle, theme.Text.Muted);
        control.LowCellStyle = TesseraThemeControlExtensions.ApplyDefault(control.LowCellStyle, theme.Text.Secondary);
        control.MidCellStyle = TesseraThemeControlExtensions.ApplyDefault(control.MidCellStyle, theme.Accent.Secondary);
        control.HighCellStyle = TesseraThemeControlExtensions.ApplyDefault(control.HighCellStyle, theme.Accent.Primary);
        control.PeakCellStyle = TesseraThemeControlExtensions.ApplyDefault(control.PeakCellStyle, theme.State.Success);
        control.HeaderStyle = TesseraThemeControlExtensions.ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.LegendStyle = TesseraThemeControlExtensions.ApplyDefault(control.LegendStyle, theme.Text.Secondary);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static Heatmap ApplyThemeDefaults(
        this Heatmap control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static ProcessListView ApplyThemeDefaults(this ProcessListView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.HeaderStyle = TesseraThemeControlExtensions.ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.RowStyle = TesseraThemeControlExtensions.ApplyDefault(control.RowStyle, theme.Text.Primary);
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.StatusStyle = TesseraThemeControlExtensions.ApplyDefault(control.StatusStyle, theme.Text.Secondary);
        control.MutedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static ProcessListView ApplyThemeDefaults(
        this ProcessListView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static TreeMapChart ApplyThemeDefaults(this TreeMapChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.NodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.NodeStyle, theme.Text.Primary);
        control.LowNodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.LowNodeStyle, theme.Text.Secondary);
        control.MidNodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.MidNodeStyle, theme.Accent.Secondary);
        control.HighNodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.HighNodeStyle, theme.Accent.Primary);
        control.PeakNodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.PeakNodeStyle, theme.State.Success);
        control.HoveredNodeStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredNodeStyle, theme.Accent.Secondary);
        control.SelectedNodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedNodeStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedNodeStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedNodeStyle, theme.Focus.Ring);
        control.DisabledNodeStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledNodeStyle, theme.Text.Muted);
        control.LabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.LabelStyle, theme.Text.Secondary);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static TreeMapChart ApplyThemeDefaults(
        this TreeMapChart control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static PaletteEditor ApplyThemeDefaults(this PaletteEditor control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.SwatchStyle = TesseraThemeControlExtensions.ApplyDefault(control.SwatchStyle, theme.Text.Primary);
        control.HoveredSwatchStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredSwatchStyle, theme.Accent.Secondary);
        control.SelectedSwatchStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedSwatchStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedSwatchStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedSwatchStyle, theme.Focus.Ring);
        control.MutedSwatchStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.MutedSwatchStyle, theme.Text.Muted);
        control.PreviewSwatchStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PreviewSwatchStyle, theme.Accent.Primary);
        control.DisabledSwatchStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledSwatchStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static PaletteEditor ApplyThemeDefaults(
        this PaletteEditor control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static TerminalPanel ApplyThemeDefaults(this TerminalPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.StandardOutputStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.StandardOutputStyle, theme.Text.Primary);
        control.StandardErrorStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.StandardErrorStyle, theme.State.Error);
        control.CommandStyle = TesseraThemeControlExtensions.ApplyDefault(control.CommandStyle, theme.Accent.Primary);
        control.SystemStyle = TesseraThemeControlExtensions.ApplyDefault(control.SystemStyle, theme.Text.Secondary);
        control.MarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.MarkerStyle, theme.Text.Secondary);
        control.HoveredLineStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredLineStyle, theme.Accent.Secondary);
        control.SelectedLineStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedLineStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedLineStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedLineStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="overrides">The overrides value.</param>
    /// <param name="baseTheme">The base theme value.</param>
    /// <param name="state">The state value.</param>
    /// <returns>The result of apply theme defaults.</returns>
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
