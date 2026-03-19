using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static Choice ApplyTheme(this Choice control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static Choice ApplyTheme(
        this Choice control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Choice ApplyThemeDefaults(this Choice control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static Choice ApplyThemeDefaults(
        this Choice control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static ComboBox ApplyTheme(this ComboBox control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static ComboBox ApplyTheme(
        this ComboBox control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static ComboBox ApplyThemeDefaults(this ComboBox control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static ComboBox ApplyThemeDefaults(
        this ComboBox control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static TreeView ApplyTheme(this TreeView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static TreeView ApplyTheme(
        this TreeView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static TreeView ApplyThemeDefaults(this TreeView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static TreeView ApplyThemeDefaults(
        this TreeView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static MenuBar ApplyTheme(this MenuBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static MenuBar ApplyTheme(
        this MenuBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static MenuBar ApplyThemeDefaults(this MenuBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static MenuBar ApplyThemeDefaults(
        this MenuBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static ContextMenu ApplyTheme(this ContextMenu control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static ContextMenu ApplyTheme(
        this ContextMenu control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static ContextMenu ApplyThemeDefaults(this ContextMenu control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static ContextMenu ApplyThemeDefaults(
        this ContextMenu control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static CommandPalette ApplyTheme(this CommandPalette control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static CommandPalette ApplyTheme(
        this CommandPalette control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static CommandPalette ApplyThemeDefaults(this CommandPalette control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static CommandPalette ApplyThemeDefaults(
        this CommandPalette control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Notifications ApplyTheme(this Notifications control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static Notifications ApplyTheme(
        this Notifications control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Notifications ApplyThemeDefaults(this Notifications control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);
        return control;
    }

    public static Notifications ApplyThemeDefaults(
        this Notifications control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
