using Tessera.Controls;

namespace Tessera.Styles;

public static partial class TesseraThemeControlExtensions
{
    /// <summary>
    /// Applies resolved theme tokens to a <see cref="JsonTreeView" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static JsonTreeView ApplyTheme(this JsonTreeView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ContainerStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.MutedStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="JsonTreeView" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static JsonTreeView ApplyTheme(
        this JsonTreeView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="JsonTreeView" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static JsonTreeView ApplyThemeDefaults(this JsonTreeView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ContainerStyle = ApplyDefault(control.ContainerStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="JsonTreeView" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static JsonTreeView ApplyThemeDefaults(
        this JsonTreeView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to a <see cref="CommandOutput" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static CommandOutput ApplyTheme(this CommandOutput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.StdOutStyle = theme.Text.Primary;
        control.StdErrStyle = theme.State.Error;
        control.SystemStyle = theme.Accent.Secondary;
        control.HoveredLineStyle = theme.Accent.Secondary;
        control.SelectedLineStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedLineStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.TimestampStyle = theme.Text.Secondary;
        control.EmptyStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="CommandOutput" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static CommandOutput ApplyTheme(
        this CommandOutput control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="CommandOutput" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static CommandOutput ApplyThemeDefaults(this CommandOutput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.StdOutStyle = ApplyDefault(control.StdOutStyle, theme.Text.Primary);
        control.StdErrStyle = ApplyDefault(control.StdErrStyle, theme.State.Error);
        control.SystemStyle = ApplyDefault(control.SystemStyle, theme.Accent.Secondary);
        control.HoveredLineStyle = ApplyDefault(control.HoveredLineStyle, theme.Accent.Secondary);
        control.SelectedLineStyle = ApplyDefault(
            control.SelectedLineStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedLineStyle = ApplyDefault(control.FocusedSelectedLineStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.TimestampStyle = ApplyDefault(control.TimestampStyle, theme.Text.Secondary);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="CommandOutput" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static CommandOutput ApplyThemeDefaults(
        this CommandOutput control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to a <see cref="LogTailPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static LogTailPanel ApplyTheme(this LogTailPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.EntryStyle = theme.Text.Primary;
        control.HoveredEntryStyle = theme.Accent.Secondary;
        control.SelectedEntryStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedEntryStyle = theme.Focus.Ring;
        control.MutedEntryStyle = theme.Text.Muted;
        control.TraceEntryStyle = theme.Text.Muted;
        control.DebugEntryStyle = theme.Text.Secondary;
        control.InfoEntryStyle = theme.State.Info;
        control.WarningEntryStyle = theme.State.Warning;
        control.ErrorEntryStyle = theme.State.Error;
        control.CriticalEntryStyle = theme.State.Error;
        control.DisabledEntryStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="LogTailPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static LogTailPanel ApplyTheme(
        this LogTailPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="LogTailPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static LogTailPanel ApplyThemeDefaults(this LogTailPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.EntryStyle = ApplyDefault(control.EntryStyle, theme.Text.Primary);
        control.HoveredEntryStyle = ApplyDefault(control.HoveredEntryStyle, theme.Accent.Secondary);
        control.SelectedEntryStyle = ApplyDefault(
            control.SelectedEntryStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedEntryStyle = ApplyDefault(control.FocusedSelectedEntryStyle, theme.Focus.Ring);
        control.MutedEntryStyle = ApplyDefault(control.MutedEntryStyle, theme.Text.Muted);
        control.TraceEntryStyle = ApplyDefault(control.TraceEntryStyle, theme.Text.Muted);
        control.DebugEntryStyle = ApplyDefault(control.DebugEntryStyle, theme.Text.Secondary);
        control.InfoEntryStyle = ApplyDefault(control.InfoEntryStyle, theme.State.Info);
        control.WarningEntryStyle = ApplyDefault(control.WarningEntryStyle, theme.State.Warning);
        control.ErrorEntryStyle = ApplyDefault(control.ErrorEntryStyle, theme.State.Error);
        control.CriticalEntryStyle = ApplyDefault(control.CriticalEntryStyle, theme.State.Error);
        control.DisabledEntryStyle = ApplyDefault(control.DisabledEntryStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="LogTailPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static LogTailPanel ApplyThemeDefaults(
        this LogTailPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to an <see cref="ActivityFeed" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static ActivityFeed ApplyTheme(this ActivityFeed control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.InfoItemStyle = theme.State.Info;
        control.SuccessItemStyle = theme.State.Success;
        control.WarningItemStyle = theme.State.Warning;
        control.ErrorItemStyle = theme.State.Error;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedItemStyle = theme.Focus.Ring;
        control.UnreadItemStyle = theme.Accent.Primary;
        control.MutedItemStyle = theme.Text.Muted;
        control.DisabledItemStyle = theme.Text.Muted;
        control.TimestampStyle = theme.Text.Secondary;
        control.EmptyStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to an <see cref="ActivityFeed" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static ActivityFeed ApplyTheme(
        this ActivityFeed control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on an <see cref="ActivityFeed" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static ActivityFeed ApplyThemeDefaults(this ActivityFeed control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.InfoItemStyle = ApplyDefault(control.InfoItemStyle, theme.State.Info);
        control.SuccessItemStyle = ApplyDefault(control.SuccessItemStyle, theme.State.Success);
        control.WarningItemStyle = ApplyDefault(control.WarningItemStyle, theme.State.Warning);
        control.ErrorItemStyle = ApplyDefault(control.ErrorItemStyle, theme.State.Error);
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedItemStyle = ApplyDefault(control.FocusedSelectedItemStyle, theme.Focus.Ring);
        control.UnreadItemStyle = ApplyDefault(control.UnreadItemStyle, theme.Accent.Primary);
        control.MutedItemStyle = ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.DisabledItemStyle = ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.TimestampStyle = ApplyDefault(control.TimestampStyle, theme.Text.Secondary);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to an <see cref="ActivityFeed" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static ActivityFeed ApplyThemeDefaults(
        this ActivityFeed control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to a <see cref="NotificationInbox" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static NotificationInbox ApplyTheme(this NotificationInbox control, TesseraTheme theme)
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
        control.PinnedItemStyle = theme.Accent.Primary;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="NotificationInbox" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static NotificationInbox ApplyTheme(
        this NotificationInbox control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="NotificationInbox" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static NotificationInbox ApplyThemeDefaults(this NotificationInbox control, TesseraTheme theme)
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
        control.PinnedItemStyle = ApplyDefault(control.PinnedItemStyle, theme.Accent.Primary);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="NotificationInbox" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static NotificationInbox ApplyThemeDefaults(
        this NotificationInbox control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to a <see cref="KeyBindingHelpDialog" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static KeyBindingHelpDialog ApplyTheme(this KeyBindingHelpDialog control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.GroupStyle = theme.Text.Secondary;
        control.KeysStyle = theme.Accent.Primary;
        control.DescriptionStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.GlobalBindingStyle = theme.State.Info;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="KeyBindingHelpDialog" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static KeyBindingHelpDialog ApplyTheme(
        this KeyBindingHelpDialog control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="KeyBindingHelpDialog" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static KeyBindingHelpDialog ApplyThemeDefaults(this KeyBindingHelpDialog control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.GroupStyle = ApplyDefault(control.GroupStyle, theme.Text.Secondary);
        control.KeysStyle = ApplyDefault(control.KeysStyle, theme.Accent.Primary);
        control.DescriptionStyle = ApplyDefault(control.DescriptionStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.GlobalBindingStyle = ApplyDefault(control.GlobalBindingStyle, theme.State.Info);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="KeyBindingHelpDialog" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static KeyBindingHelpDialog ApplyThemeDefaults(
        this KeyBindingHelpDialog control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to a <see cref="TraceViewer" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static TraceViewer ApplyTheme(this TraceViewer control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.EntryStyle = theme.Text.Primary;
        control.VerboseRowStyle = theme.Text.Muted;
        control.InfoRowStyle = theme.State.Info;
        control.WarningRowStyle = theme.State.Warning;
        control.ErrorRowStyle = theme.State.Error;
        control.CriticalRowStyle = theme.State.Error;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.MutedRowStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="TraceViewer" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static TraceViewer ApplyTheme(
        this TraceViewer control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="TraceViewer" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static TraceViewer ApplyThemeDefaults(this TraceViewer control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.EntryStyle = ApplyDefault(control.EntryStyle, theme.Text.Primary);
        control.VerboseRowStyle = ApplyDefault(control.VerboseRowStyle, theme.Text.Muted);
        control.InfoRowStyle = ApplyDefault(control.InfoRowStyle, theme.State.Info);
        control.WarningRowStyle = ApplyDefault(control.WarningRowStyle, theme.State.Warning);
        control.ErrorRowStyle = ApplyDefault(control.ErrorRowStyle, theme.State.Error);
        control.CriticalRowStyle = ApplyDefault(control.CriticalRowStyle, theme.State.Error);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.MutedRowStyle = ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="TraceViewer" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static TraceViewer ApplyThemeDefaults(
        this TraceViewer control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies resolved theme tokens to a <see cref="TaskRunnerPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fully resolved theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static TaskRunnerPanel ApplyTheme(this TaskRunnerPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.RowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedRowStyle = theme.Focus.Ring;
        control.DisabledRowStyle = theme.Text.Muted;
        control.StatusMarkerStyle = theme.Text.Secondary;
        control.RunningStatusStyle = theme.State.Info;
        control.SucceededStatusStyle = theme.State.Success;
        control.FailedStatusStyle = theme.State.Error;
        control.EmptyStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies the resulting theme to a <see cref="TaskRunnerPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static TaskRunnerPanel ApplyTheme(
        this TaskRunnerPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme tokens to unset style members on a <see cref="TaskRunnerPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static TaskRunnerPanel ApplyThemeDefaults(this TaskRunnerPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.RowStyle = ApplyDefault(control.RowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle = ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledRowStyle = ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.StatusMarkerStyle = ApplyDefault(control.StatusMarkerStyle, theme.Text.Secondary);
        control.RunningStatusStyle = ApplyDefault(control.RunningStatusStyle, theme.State.Info);
        control.SucceededStatusStyle = ApplyDefault(control.SucceededStatusStyle, theme.State.Success);
        control.FailedStatusStyle = ApplyDefault(control.FailedStatusStyle, theme.State.Error);
        control.EmptyStyle = ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves overrides and applies default-only theme values to a <see cref="TaskRunnerPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="overrides">Theme override rules used to resolve effective tokens.</param>
    /// <param name="baseTheme">The base theme used during override resolution.</param>
    /// <param name="state">The visual state used during override resolution.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static TaskRunnerPanel ApplyThemeDefaults(
        this TaskRunnerPanel control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
