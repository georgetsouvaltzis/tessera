using Tessera.Controls;

namespace Tessera.Styles;

public static partial class TesseraThemeControlExtensions
{
    /// <summary>
    /// Applies theme defaults to a <see cref="Button"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Button ApplyThemeDefaults(this Button control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        var buttonSurface = ResolveButtonSurface(theme);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.FocusedLabelStyle = ApplyDefault(
            control.FocusedLabelStyle,
            control.LabelStyle.IsEmpty ? theme.Text.Primary.WithBold() : TesseraStyle.Empty);
        control.DisabledLabelStyle = ApplyDefault(control.DisabledLabelStyle, theme.Text.Muted);
        control.SurfaceStyle = ApplyDefault(control.SurfaceStyle, buttonSurface);
        control.FocusedSurfaceStyle = ApplyDefault(control.FocusedSurfaceStyle, buttonSurface);
        control.PressedLabelStyle = ApplyDefault(
            control.PressedLabelStyle,
            theme.Text.Primary.WithBold());
        control.PressedSurfaceStyle = ApplyDefault(control.PressedSurfaceStyle, ResolvePressedButtonSurface(theme, buttonSurface));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Button"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Button ApplyThemeDefaults(
        this Button control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Button"/>.
    /// </summary>
    public static Button ApplyTheme(this Button control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        var buttonSurface = ResolveButtonSurface(theme);
        control.LabelStyle = theme.Text.Primary;
        control.FocusedLabelStyle = theme.Text.Primary.WithBold();
        control.DisabledLabelStyle = theme.Text.Muted;
        control.SurfaceStyle = buttonSurface;
        control.FocusedSurfaceStyle = buttonSurface;
        control.PressedLabelStyle = theme.Text.Primary.WithBold();
        control.PressedSurfaceStyle = ResolvePressedButtonSurface(theme, buttonSurface);
        return control;
    }

    private static TesseraStyle ResolveButtonSurface(TesseraTheme theme)
    {
        return theme.Surface.Overlay.IsEmpty
            ? theme.Surface.Panel
            : theme.Surface.Overlay;
    }

    private static TesseraStyle ResolvePressedButtonSurface(TesseraTheme theme, TesseraStyle fallback)
    {
        return theme.Selection.Background.IsEmpty
            ? fallback
            : theme.Selection.Background;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Button"/>.
    /// </summary>
    public static Button ApplyTheme(
        this Button control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="ListView{T}"/>.
    /// </summary>
    public static ListView<T> ApplyTheme<T>(this ListView<T> control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.DefaultRowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusMarker = theme.Focus.Marker;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="ListView{T}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ListView<T> ApplyThemeDefaults<T>(this ListView<T> control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.DefaultRowStyle = ApplyDefault(control.DefaultRowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="ListView{T}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ListView<T> ApplyThemeDefaults<T>(
        this ListView<T> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="ListView{T}"/>.
    /// </summary>
    public static ListView<T> ApplyTheme<T>(
        this ListView<T> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="StatusBar"/>.
    /// </summary>
    public static StatusBar ApplyTheme(this StatusBar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.LeftTextStyle = theme.Text.Primary;
        control.RightTextStyle = theme.Text.Secondary;
        control.FillStyle = theme.Surface.Panel;
        return control;
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="StatusBar"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static StatusBar ApplyThemeDefaults(this StatusBar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.LeftTextStyle = ApplyDefault(control.LeftTextStyle, theme.Text.Primary);
        control.RightTextStyle = ApplyDefault(control.RightTextStyle, theme.Text.Secondary);
        control.FillStyle = ApplyDefault(control.FillStyle, theme.Surface.Panel);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="StatusBar"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static StatusBar ApplyThemeDefaults(
        this StatusBar control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="StatusBar"/>.
    /// </summary>
    public static StatusBar ApplyTheme(
        this StatusBar control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="TextInput"/>.
    /// </summary>
    public static TextInput ApplyTheme(this TextInput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="TextInput"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TextInput ApplyThemeDefaults(this TextInput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="TextInput"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TextInput ApplyThemeDefaults(
        this TextInput control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="TextInput"/>.
    /// </summary>
    public static TextInput ApplyTheme(
        this TextInput control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Table"/>.
    /// </summary>
    public static Table ApplyTheme(this Table control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.RowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusMarker = theme.Focus.Marker;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Table"/>.
    /// </summary>
    public static Table ApplyTheme(
        this Table control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Table"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Table ApplyThemeDefaults(this Table control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.RowStyle = ApplyDefault(control.RowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Table"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Table ApplyThemeDefaults(
        this Table control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Tabs"/>.
    /// </summary>
    public static Tabs ApplyTheme(this Tabs control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Tabs"/>.
    /// </summary>
    public static Tabs ApplyTheme(
        this Tabs control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyTheme(resolved);
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Tabs"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Tabs ApplyThemeDefaults(this Tabs control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Tabs"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Tabs ApplyThemeDefaults(
        this Tabs control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        var resolved = overrides.Resolve(control, baseTheme, state);
        return control.ApplyThemeDefaults(resolved);
    }
}
