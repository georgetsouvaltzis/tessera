using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static BulletChart ApplyTheme(this BulletChart control, TeaTheme theme)
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

    public static BulletChart ApplyTheme(
        this BulletChart control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static BulletChart ApplyThemeDefaults(this BulletChart control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.RangeStyle = ApplyDefault(control.RangeStyle, theme.Text.Muted);
        control.WarningRangeStyle = ApplyDefault(control.WarningRangeStyle, theme.State.Warning);
        control.CriticalRangeStyle = ApplyDefault(control.CriticalRangeStyle, theme.State.Error);
        control.ValueBarStyle = ApplyDefault(control.ValueBarStyle, theme.Accent.Primary);
        control.TargetMarkerStyle = ApplyDefault(control.TargetMarkerStyle, theme.Focus.Ring);
        control.ValueLabelStyle = ApplyDefault(control.ValueLabelStyle, theme.Text.Secondary);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static BulletChart ApplyThemeDefaults(
        this BulletChart control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static DashboardGrid ApplyTheme(this DashboardGrid control, TeaTheme theme)
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

    public static DashboardGrid ApplyTheme(
        this DashboardGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DashboardGrid ApplyThemeDefaults(this DashboardGrid control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyleText = ApplyDefault(control.TitleStyleText, theme.Text.Secondary);
        control.FocusedTitleStyleText = ApplyDefault(control.FocusedTitleStyleText, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.TileStyleText = ApplyDefault(control.TileStyleText, theme.Text.Primary);
        control.SelectedTileStyleText = ApplyDefault(control.SelectedTileStyleText, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredTileStyleText = ApplyDefault(control.HoveredTileStyleText, theme.Accent.Secondary);
        control.DisabledTileStyleText = ApplyDefault(control.DisabledTileStyleText, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static DashboardGrid ApplyThemeDefaults(
        this DashboardGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static HealthBoard ApplyTheme(this HealthBoard control, TeaTheme theme)
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

    public static HealthBoard ApplyTheme(
        this HealthBoard control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static HealthBoard ApplyThemeDefaults(this HealthBoard control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.ServiceStyle = ApplyDefault(control.ServiceStyle, theme.Text.Primary);
        control.HealthyServiceStyle = ApplyDefault(control.HealthyServiceStyle, theme.State.Success);
        control.DegradedServiceStyle = ApplyDefault(control.DegradedServiceStyle, theme.State.Warning);
        control.OutageServiceStyle = ApplyDefault(control.OutageServiceStyle, theme.State.Error);
        control.HoveredServiceStyle = ApplyDefault(control.HoveredServiceStyle, theme.Accent.Secondary);
        control.SelectedServiceStyle = ApplyDefault(control.SelectedServiceStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedServiceStyle = ApplyDefault(control.FocusedSelectedServiceStyle, theme.Focus.Ring);
        control.AcknowledgedServiceStyle = ApplyDefault(control.AcknowledgedServiceStyle, theme.Text.Muted);
        control.MutedServiceStyle = ApplyDefault(control.MutedServiceStyle, theme.Text.Muted);
        control.DisabledServiceStyle = ApplyDefault(control.DisabledServiceStyle, theme.Text.Muted);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    public static HealthBoard ApplyThemeDefaults(
        this HealthBoard control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
