using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
///     Represents tessera theme control extensions dashboard metrics apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsDashboardMetricsApplyExtensions
{
    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static BulletChart ApplyTheme(this BulletChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.RangeStyle = theme.Text.Muted;
        control.WarningRangeStyle = theme.State.Warning;
        control.CriticalRangeStyle = theme.State.Error;
        control.ValueBarStyle = theme.Accent.Primary;
        control.TargetMarkerStyle = theme.Focus.Ring;
        control.ValueLabelStyle = theme.Text.Secondary;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static BulletChart ApplyTheme(
        this BulletChart control,
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
    public static DashboardGrid ApplyTheme(this DashboardGrid control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyleText = theme.Text.Secondary;
        control.FocusedTitleStyleText = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.TileStyleText = theme.Text.Primary;
        control.SelectedTileStyleText = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredTileStyleText = theme.Accent.Secondary;
        control.DisabledTileStyleText = theme.Text.Muted;
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
    public static DashboardGrid ApplyTheme(
        this DashboardGrid control,
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
    public static HealthBoard ApplyTheme(this HealthBoard control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.ServiceStyle = theme.Text.Primary;
        control.HealthyServiceStyle = theme.State.Success;
        control.DegradedServiceStyle = theme.State.Warning;
        control.OutageServiceStyle = theme.State.Error;
        control.HoveredServiceStyle = theme.Accent.Secondary;
        control.SelectedServiceStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedServiceStyle = theme.Focus.Ring;
        control.AcknowledgedServiceStyle = theme.Text.Muted;
        control.MutedServiceStyle = theme.Text.Muted;
        control.DisabledServiceStyle = theme.Text.Muted;
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
    public static HealthBoard ApplyTheme(
        this HealthBoard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
///     Represents tessera theme control extensions dashboard metrics default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsDashboardMetricsDefaultExtensions
{
    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static BulletChart ApplyThemeDefaults(this BulletChart control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.RangeStyle = TesseraThemeControlExtensions.ApplyDefault(control.RangeStyle, theme.Text.Muted);
        control.WarningRangeStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.WarningRangeStyle, theme.State.Warning);
        control.CriticalRangeStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.CriticalRangeStyle, theme.State.Error);
        control.ValueBarStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueBarStyle, theme.Accent.Primary);
        control.TargetMarkerStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.TargetMarkerStyle, theme.Focus.Ring);
        control.ValueLabelStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.ValueLabelStyle, theme.Text.Secondary);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static BulletChart ApplyThemeDefaults(
        this BulletChart control,
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
    public static DashboardGrid ApplyThemeDefaults(this DashboardGrid control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.TitleStyleText, theme.Text.Secondary);
        control.FocusedTitleStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyleText, theme.Focus.Title);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.TileStyleText = TesseraThemeControlExtensions.ApplyDefault(control.TileStyleText, theme.Text.Primary);
        control.SelectedTileStyleText = TesseraThemeControlExtensions.ApplyDefault(control.SelectedTileStyleText,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredTileStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredTileStyleText, theme.Accent.Secondary);
        control.DisabledTileStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledTileStyleText, theme.Text.Muted);
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
    public static DashboardGrid ApplyThemeDefaults(
        this DashboardGrid control,
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
    public static HealthBoard ApplyThemeDefaults(this HealthBoard control, TesseraTheme theme)
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
        control.ServiceStyle = TesseraThemeControlExtensions.ApplyDefault(control.ServiceStyle, theme.Text.Primary);
        control.HealthyServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HealthyServiceStyle, theme.State.Success);
        control.DegradedServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DegradedServiceStyle, theme.State.Warning);
        control.OutageServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.OutageServiceStyle, theme.State.Error);
        control.HoveredServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredServiceStyle, theme.Accent.Secondary);
        control.SelectedServiceStyle = TesseraThemeControlExtensions.ApplyDefault(control.SelectedServiceStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedServiceStyle, theme.Focus.Ring);
        control.AcknowledgedServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.AcknowledgedServiceStyle, theme.Text.Muted);
        control.MutedServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.MutedServiceStyle, theme.Text.Muted);
        control.DisabledServiceStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledServiceStyle, theme.Text.Muted);
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
    public static HealthBoard ApplyThemeDefaults(
        this HealthBoard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
