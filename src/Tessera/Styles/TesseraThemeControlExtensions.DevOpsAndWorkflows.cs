using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
///     Represents tessera theme control extensions dev ops and workflows apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsDevOpsAndWorkflowsApplyExtensions
{
    /// <summary>
    ///     Applies resolved theme tokens to a <see cref="JsonTreeView" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="JsonTreeView" />.
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
    ///     Applies resolved theme tokens to a <see cref="CommandOutput" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="CommandOutput" />.
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
    ///     Applies resolved theme tokens to a <see cref="LogTailPanel" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="LogTailPanel" />.
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
    ///     Applies resolved theme tokens to an <see cref="ActivityFeed" />.
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
    ///     Resolves overrides and applies the resulting theme to an <see cref="ActivityFeed" />.
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
    ///     Applies resolved theme tokens to a <see cref="NotificationInbox" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="NotificationInbox" />.
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
    ///     Applies resolved theme tokens to a <see cref="KeyBindingHelpDialog" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="KeyBindingHelpDialog" />.
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
    ///     Applies resolved theme tokens to a <see cref="TraceViewer" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="TraceViewer" />.
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
    ///     Applies resolved theme tokens to a <see cref="TaskRunnerPanel" />.
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
    ///     Resolves overrides and applies the resulting theme to a <see cref="TaskRunnerPanel" />.
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
}

/// <summary>
///     Represents tessera theme control extensions dev ops and workflows default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsDevOpsAndWorkflowsDefaultExtensions
{
    /// <summary>
    ///     Applies theme tokens to unset style members on a <see cref="JsonTreeView" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static JsonTreeView ApplyThemeDefaults(this JsonTreeView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ContainerStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.ContainerStyle, theme.Text.Secondary);
        control.ValueStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.MutedStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="JsonTreeView" />.
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
    ///     Applies theme tokens to unset style members on a <see cref="CommandOutput" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static CommandOutput ApplyThemeDefaults(this CommandOutput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.StdOutStyle = TesseraThemeControlExtensions.ApplyDefault(control.StdOutStyle, theme.Text.Primary);
        control.StdErrStyle = TesseraThemeControlExtensions.ApplyDefault(control.StdErrStyle, theme.State.Error);
        control.SystemStyle = TesseraThemeControlExtensions.ApplyDefault(control.SystemStyle, theme.Accent.Secondary);
        control.HoveredLineStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredLineStyle, theme.Accent.Secondary);
        control.SelectedLineStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedLineStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedLineStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedLineStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.TimestampStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.TimestampStyle, theme.Text.Secondary);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="CommandOutput" />.
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
    ///     Applies theme tokens to unset style members on a <see cref="LogTailPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static LogTailPanel ApplyThemeDefaults(this LogTailPanel control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.EntryStyle = TesseraThemeControlExtensions.ApplyDefault(control.EntryStyle, theme.Text.Primary);
        control.HoveredEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredEntryStyle, theme.Accent.Secondary);
        control.SelectedEntryStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedEntryStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedEntryStyle, theme.Focus.Ring);
        control.MutedEntryStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedEntryStyle, theme.Text.Muted);
        control.TraceEntryStyle = TesseraThemeControlExtensions.ApplyDefault(control.TraceEntryStyle, theme.Text.Muted);
        control.DebugEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DebugEntryStyle, theme.Text.Secondary);
        control.InfoEntryStyle = TesseraThemeControlExtensions.ApplyDefault(control.InfoEntryStyle, theme.State.Info);
        control.WarningEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.WarningEntryStyle, theme.State.Warning);
        control.ErrorEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.ErrorEntryStyle, theme.State.Error);
        control.CriticalEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.CriticalEntryStyle, theme.State.Error);
        control.DisabledEntryStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledEntryStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="LogTailPanel" />.
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
    ///     Applies theme tokens to unset style members on an <see cref="ActivityFeed" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static ActivityFeed ApplyThemeDefaults(this ActivityFeed control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.InfoItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.InfoItemStyle, theme.State.Info);
        control.SuccessItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.SuccessItemStyle, theme.State.Success);
        control.WarningItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.WarningItemStyle, theme.State.Warning);
        control.ErrorItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorItemStyle, theme.State.Error);
        control.HoveredItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedItemStyle, theme.Focus.Ring);
        control.UnreadItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.UnreadItemStyle, theme.Accent.Primary);
        control.MutedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.TimestampStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.TimestampStyle, theme.Text.Secondary);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to an <see cref="ActivityFeed" />.
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
    ///     Applies theme tokens to unset style members on a <see cref="NotificationInbox" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static NotificationInbox ApplyThemeDefaults(this NotificationInbox control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.UnreadItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.UnreadItemStyle, theme.Accent.Primary);
        control.MutedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.InfoItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.InfoItemStyle, theme.State.Info);
        control.SuccessItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.SuccessItemStyle, theme.State.Success);
        control.WarningItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.WarningItemStyle, theme.State.Warning);
        control.ErrorItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorItemStyle, theme.State.Error);
        control.PinnedItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PinnedItemStyle, theme.Accent.Primary);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="NotificationInbox" />.
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
    ///     Applies theme tokens to unset style members on a <see cref="KeyBindingHelpDialog" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static KeyBindingHelpDialog ApplyThemeDefaults(this KeyBindingHelpDialog control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.GroupStyle = TesseraThemeControlExtensions.ApplyDefault(control.GroupStyle, theme.Text.Secondary);
        control.KeysStyle = TesseraThemeControlExtensions.ApplyDefault(control.KeysStyle, theme.Accent.Primary);
        control.DescriptionStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DescriptionStyle, theme.Text.Primary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.GlobalBindingStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.GlobalBindingStyle, theme.State.Info);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="KeyBindingHelpDialog" />.
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
    ///     Applies theme tokens to unset style members on a <see cref="TraceViewer" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static TraceViewer ApplyThemeDefaults(this TraceViewer control, TesseraTheme theme)
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
        control.EntryStyle = TesseraThemeControlExtensions.ApplyDefault(control.EntryStyle, theme.Text.Primary);
        control.VerboseRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.VerboseRowStyle, theme.Text.Muted);
        control.InfoRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.InfoRowStyle, theme.State.Info);
        control.WarningRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.WarningRowStyle, theme.State.Warning);
        control.ErrorRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorRowStyle, theme.State.Error);
        control.CriticalRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.CriticalRowStyle, theme.State.Error);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.MutedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="TraceViewer" />.
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
    ///     Applies theme tokens to unset style members on a <see cref="TaskRunnerPanel" />.
    /// </summary>
    /// <param name="control">The control to mutate.</param>
    /// <param name="theme">The fallback theme tokens.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    /// <remarks>Existing non-empty style values are preserved.</remarks>
    public static TaskRunnerPanel ApplyThemeDefaults(this TaskRunnerPanel control, TesseraTheme theme)
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
        control.RowStyle = TesseraThemeControlExtensions.ApplyDefault(control.RowStyle, theme.Text.Primary);
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedRowStyle, theme.Focus.Ring);
        control.DisabledRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.StatusMarkerStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.StatusMarkerStyle, theme.Text.Secondary);
        control.RunningStatusStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.RunningStatusStyle, theme.State.Info);
        control.SucceededStatusStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.SucceededStatusStyle, theme.State.Success);
        control.FailedStatusStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FailedStatusStyle, theme.State.Error);
        control.EmptyStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves overrides and applies default-only theme values to a <see cref="TaskRunnerPanel" />.
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
