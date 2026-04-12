using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
/// Represents tessera theme control extensions plotting apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsPlottingApplyExtensions
{
    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static TelemetryChart ApplyTheme(this TelemetryChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FillStyle = theme.Accent.Primary;
        control.MetaStyle = theme.Text.Secondary;
        control.EmptyTextStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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
    public static TelemetryChart ApplyTheme(
        this TelemetryChart control,
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
    public static Sparkline ApplyTheme(this Sparkline control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DataStyle = theme.Accent.Primary;
        control.MetaStyle = theme.Text.Secondary;
        control.EmptyTextStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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
    public static Sparkline ApplyTheme(
        this Sparkline control,
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
    public static AreaPlot ApplyTheme(this AreaPlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FillStyle = theme.Accent.Primary;
        control.LineStyle = theme.Accent.Secondary;
        control.BaselineStyle = theme.Text.Muted;
        control.MetaStyle = theme.Text.Secondary;
        control.EmptyTextStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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
    public static AreaPlot ApplyTheme(
        this AreaPlot control,
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
    public static ScatterPlot ApplyTheme(this ScatterPlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.PointStyle = theme.Accent.Primary;
        control.AxisStyle = theme.Text.Muted;
        control.LabelStyle = theme.Text.Primary;
        control.LegendStyle = theme.Text.Secondary;
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
    public static ScatterPlot ApplyTheme(
        this ScatterPlot control,
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
    public static Histogram ApplyTheme(this Histogram control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BarStyle = theme.Accent.Primary;
        control.AxisStyle = theme.Text.Muted;
        control.LabelStyle = theme.Text.Primary;
        control.LegendStyle = theme.Text.Secondary;
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
    public static Histogram ApplyTheme(
        this Histogram control,
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
    public static LinePlot ApplyTheme(this LinePlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.StatsStyle = theme.Text.Secondary;
        control.LegendStyle = theme.Accent.Secondary;
        control.AxisStyle = theme.Text.Muted;
        control.GridStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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
    public static LinePlot ApplyTheme(
        this LinePlot control,
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
    public static PlotPanel ApplyTheme(this PlotPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.EmptyTextStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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
    public static PlotPanel ApplyTheme(
        this PlotPanel control,
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
    public static BoxPlot ApplyTheme(this BoxPlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.SeriesStyle = theme.Text.Primary;
        control.HoveredSeriesStyle = theme.Accent.Secondary;
        control.SelectedSeriesStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedSeriesStyle = theme.Focus.Ring;
        control.QuartileStyle = theme.Accent.Primary;
        control.MedianStyle = theme.Focus.Ring;
        control.WhiskerStyle = theme.Text.Secondary;
        control.MutedSeriesStyle = theme.Text.Muted;
        control.DisabledSeriesStyle = theme.Text.Muted;
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
    public static BoxPlot ApplyTheme(
        this BoxPlot control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
/// Represents tessera theme control extensions plotting default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsPlottingDefaultExtensions
{
    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static TelemetryChart ApplyThemeDefaults(this TelemetryChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FillStyle = TesseraThemeControlExtensions.ApplyDefault(control.FillStyle, theme.Accent.Primary);
        control.MetaStyle = TesseraThemeControlExtensions.ApplyDefault(control.MetaStyle, theme.Text.Secondary);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static TelemetryChart ApplyThemeDefaults(
        this TelemetryChart control,
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
    public static Sparkline ApplyThemeDefaults(this Sparkline control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DataStyle = TesseraThemeControlExtensions.ApplyDefault(control.DataStyle, theme.Accent.Primary);
        control.MetaStyle = TesseraThemeControlExtensions.ApplyDefault(control.MetaStyle, theme.Text.Secondary);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static Sparkline ApplyThemeDefaults(
        this Sparkline control,
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
    public static AreaPlot ApplyThemeDefaults(this AreaPlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FillStyle = TesseraThemeControlExtensions.ApplyDefault(control.FillStyle, theme.Accent.Primary);
        control.LineStyle = TesseraThemeControlExtensions.ApplyDefault(control.LineStyle, theme.Accent.Secondary);
        control.BaselineStyle = TesseraThemeControlExtensions.ApplyDefault(control.BaselineStyle, theme.Text.Muted);
        control.MetaStyle = TesseraThemeControlExtensions.ApplyDefault(control.MetaStyle, theme.Text.Secondary);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static AreaPlot ApplyThemeDefaults(
        this AreaPlot control,
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
    public static ScatterPlot ApplyThemeDefaults(this ScatterPlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.PointStyle = TesseraThemeControlExtensions.ApplyDefault(control.PointStyle, theme.Accent.Primary);
        control.AxisStyle = TesseraThemeControlExtensions.ApplyDefault(control.AxisStyle, theme.Text.Muted);
        control.LabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.LegendStyle = TesseraThemeControlExtensions.ApplyDefault(control.LegendStyle, theme.Text.Secondary);
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
    public static ScatterPlot ApplyThemeDefaults(
        this ScatterPlot control,
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
    public static Histogram ApplyThemeDefaults(this Histogram control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BarStyle = TesseraThemeControlExtensions.ApplyDefault(control.BarStyle, theme.Accent.Primary);
        control.AxisStyle = TesseraThemeControlExtensions.ApplyDefault(control.AxisStyle, theme.Text.Muted);
        control.LabelStyle = TesseraThemeControlExtensions.ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.LegendStyle = TesseraThemeControlExtensions.ApplyDefault(control.LegendStyle, theme.Text.Secondary);
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
    public static Histogram ApplyThemeDefaults(
        this Histogram control,
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
    public static LinePlot ApplyThemeDefaults(this LinePlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.StatsStyle = TesseraThemeControlExtensions.ApplyDefault(control.StatsStyle, theme.Text.Secondary);
        control.LegendStyle = TesseraThemeControlExtensions.ApplyDefault(control.LegendStyle, theme.Accent.Secondary);
        control.AxisStyle = TesseraThemeControlExtensions.ApplyDefault(control.AxisStyle, theme.Text.Muted);
        control.GridStyle = TesseraThemeControlExtensions.ApplyDefault(control.GridStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static LinePlot ApplyThemeDefaults(
        this LinePlot control,
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
    public static PlotPanel ApplyThemeDefaults(this PlotPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static PlotPanel ApplyThemeDefaults(
        this PlotPanel control,
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
    public static BoxPlot ApplyThemeDefaults(this BoxPlot control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.SeriesStyle = TesseraThemeControlExtensions.ApplyDefault(control.SeriesStyle, theme.Text.Primary);
        control.HoveredSeriesStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredSeriesStyle, theme.Accent.Secondary);
        control.SelectedSeriesStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedSeriesStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedSeriesStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedSeriesStyle, theme.Focus.Ring);
        control.QuartileStyle = TesseraThemeControlExtensions.ApplyDefault(control.QuartileStyle, theme.Accent.Primary);
        control.MedianStyle = TesseraThemeControlExtensions.ApplyDefault(control.MedianStyle, theme.Focus.Ring);
        control.WhiskerStyle = TesseraThemeControlExtensions.ApplyDefault(control.WhiskerStyle, theme.Text.Secondary);
        control.MutedSeriesStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedSeriesStyle, theme.Text.Muted);
        control.DisabledSeriesStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledSeriesStyle, theme.Text.Muted);
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
    public static BoxPlot ApplyThemeDefaults(
        this BoxPlot control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
