using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static Sparkline ApplyTheme(this Sparkline control, TeaTheme theme)
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

    public static Sparkline ApplyTheme(
        this Sparkline control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Sparkline ApplyThemeDefaults(this Sparkline control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DataStyle = ApplyDefault(control.DataStyle, theme.Accent.Primary);
        control.MetaStyle = ApplyDefault(control.MetaStyle, theme.Text.Secondary);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static Sparkline ApplyThemeDefaults(
        this Sparkline control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static AreaPlot ApplyTheme(this AreaPlot control, TeaTheme theme)
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

    public static AreaPlot ApplyTheme(
        this AreaPlot control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static AreaPlot ApplyThemeDefaults(this AreaPlot control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FillStyle = ApplyDefault(control.FillStyle, theme.Accent.Primary);
        control.LineStyle = ApplyDefault(control.LineStyle, theme.Accent.Secondary);
        control.BaselineStyle = ApplyDefault(control.BaselineStyle, theme.Text.Muted);
        control.MetaStyle = ApplyDefault(control.MetaStyle, theme.Text.Secondary);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static AreaPlot ApplyThemeDefaults(
        this AreaPlot control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static ScatterPlot ApplyTheme(this ScatterPlot control, TeaTheme theme)
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

    public static ScatterPlot ApplyTheme(
        this ScatterPlot control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static ScatterPlot ApplyThemeDefaults(this ScatterPlot control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.PointStyle = ApplyDefault(control.PointStyle, theme.Accent.Primary);
        control.AxisStyle = ApplyDefault(control.AxisStyle, theme.Text.Muted);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.LegendStyle = ApplyDefault(control.LegendStyle, theme.Text.Secondary);
        return control;
    }

    public static ScatterPlot ApplyThemeDefaults(
        this ScatterPlot control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Histogram ApplyTheme(this Histogram control, TeaTheme theme)
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

    public static Histogram ApplyTheme(
        this Histogram control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Histogram ApplyThemeDefaults(this Histogram control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BarStyle = ApplyDefault(control.BarStyle, theme.Accent.Primary);
        control.AxisStyle = ApplyDefault(control.AxisStyle, theme.Text.Muted);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.LegendStyle = ApplyDefault(control.LegendStyle, theme.Text.Secondary);
        return control;
    }

    public static Histogram ApplyThemeDefaults(
        this Histogram control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static LinePlot ApplyTheme(this LinePlot control, TeaTheme theme)
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

    public static LinePlot ApplyTheme(
        this LinePlot control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static LinePlot ApplyThemeDefaults(this LinePlot control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.StatsStyle = ApplyDefault(control.StatsStyle, theme.Text.Secondary);
        control.LegendStyle = ApplyDefault(control.LegendStyle, theme.Accent.Secondary);
        control.AxisStyle = ApplyDefault(control.AxisStyle, theme.Text.Muted);
        control.GridStyle = ApplyDefault(control.GridStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static LinePlot ApplyThemeDefaults(
        this LinePlot control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static PlotPanel ApplyTheme(this PlotPanel control, TeaTheme theme)
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

    public static PlotPanel ApplyTheme(
        this PlotPanel control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static PlotPanel ApplyThemeDefaults(this PlotPanel control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static PlotPanel ApplyThemeDefaults(
        this PlotPanel control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
