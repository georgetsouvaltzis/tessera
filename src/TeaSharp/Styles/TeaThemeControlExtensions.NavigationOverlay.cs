using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static Choice ApplyTheme(this Choice control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueStyle = theme.Text.Primary;
        control.OptionStyle = theme.Text.Primary;
        control.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredOptionStyle = theme.Accent.Secondary;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.OptionStyle = ApplyDefault(control.OptionStyle, theme.Text.Primary);
        control.SelectedOptionStyle = ApplyDefault(
            control.SelectedOptionStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredOptionStyle = ApplyDefault(control.HoveredOptionStyle, theme.Accent.Secondary);
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.OptionStyle = theme.Text.Primary;
        control.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredOptionStyle = theme.Accent.Secondary;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.OptionStyle = ApplyDefault(control.OptionStyle, theme.Text.Primary);
        control.SelectedOptionStyle = ApplyDefault(
            control.SelectedOptionStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredOptionStyle = ApplyDefault(control.HoveredOptionStyle, theme.Accent.Secondary);
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BranchStyle = theme.Accent.Primary;
        control.LeafStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
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

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BranchStyle = ApplyDefault(control.BranchStyle, theme.Accent.Primary);
        control.LeafStyle = ApplyDefault(control.LeafStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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

        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.FocusedItemStyle = theme.Focus.Ring;
        control.DisabledItemStyle = theme.Text.Muted;
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

        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.FocusedItemStyle = ApplyDefault(control.FocusedItemStyle, theme.Focus.Ring);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
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

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.DisabledItemStyle = theme.Text.Muted;
        control.MutedItemStyle = theme.Text.Muted;
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

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.MutedItemStyle = ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
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

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.QueryTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.MutedItemStyle = theme.Text.Muted;
        control.DisabledItemStyle = theme.Text.Muted;
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

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.QueryTextStyle = ApplyDefault(control.QueryTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.MutedItemStyle = ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
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

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.UnreadItemStyle = theme.Accent.Primary;
        control.MutedItemStyle = theme.Text.Muted;
        control.InfoItemStyle = theme.State.Info;
        control.SuccessItemStyle = theme.State.Success;
        control.WarningItemStyle = theme.State.Warning;
        control.ErrorItemStyle = theme.State.Error;
        control.DisabledItemStyle = theme.Text.Muted;
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

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.UnreadItemStyle = ApplyDefault(control.UnreadItemStyle, theme.Accent.Primary);
        control.MutedItemStyle = ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.InfoItemStyle = ApplyDefault(control.InfoItemStyle, theme.State.Info);
        control.SuccessItemStyle = ApplyDefault(control.SuccessItemStyle, theme.State.Success);
        control.WarningItemStyle = ApplyDefault(control.WarningItemStyle, theme.State.Warning);
        control.ErrorItemStyle = ApplyDefault(control.ErrorItemStyle, theme.State.Error);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
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
