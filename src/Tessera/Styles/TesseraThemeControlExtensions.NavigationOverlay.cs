using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
///     Represents tessera theme control extensions navigation overlay apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsNavigationOverlayApplyExtensions
{
    /// <summary>
    ///     Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static Choice ApplyTheme(this Choice control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueStyle = theme.Text.Primary;
        control.HoveredValueStyle = theme.Accent.Secondary;
        control.OptionStyle = theme.Text.Primary;
        control.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredOptionStyle = theme.Accent.Secondary;
        control.FocusMarker = theme.Focus.Marker;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static Choice ApplyTheme(
        this Choice control,
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
    public static ComboBox ApplyTheme(this ComboBox control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.HoveredValueStyle = theme.Accent.Secondary;
        control.OptionStyle = theme.Text.Primary;
        control.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredOptionStyle = theme.Accent.Secondary;
        control.FocusMarker = theme.Focus.Marker;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static ComboBox ApplyTheme(
        this ComboBox control,
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
    public static TreeView ApplyTheme(this TreeView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.BranchStyle = theme.Accent.Primary;
        control.LeafStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.FocusMarker = theme.Focus.Marker;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static TreeView ApplyTheme(
        this TreeView control,
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
    public static AutocompleteInput ApplyTheme(this AutocompleteInput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.InputTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.PopupStyle = theme.Text.Secondary;
        control.SuggestionStyle = theme.Text.Primary;
        control.HoveredSuggestionStyle = theme.Accent.Secondary;
        control.SelectedSuggestionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedSuggestionStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.CommitMarkerStyle = theme.Accent.Primary;
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
    public static AutocompleteInput ApplyTheme(
        this AutocompleteInput control,
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
    public static MenuBar ApplyTheme(this MenuBar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.FocusedItemStyle = theme.Focus.Ring;
        control.DisabledItemStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static MenuBar ApplyTheme(
        this MenuBar control,
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
    public static ContextMenu ApplyTheme(this ContextMenu control, TesseraTheme theme)
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
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static ContextMenu ApplyTheme(
        this ContextMenu control,
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
    public static CommandPalette ApplyTheme(this CommandPalette control, TesseraTheme theme)
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
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static CommandPalette ApplyTheme(
        this CommandPalette control,
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
    public static Notifications ApplyTheme(this Notifications control, TesseraTheme theme)
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
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static Notifications ApplyTheme(
        this Notifications control,
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
    public static QuickOpenOverlay ApplyTheme(this QuickOpenOverlay control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.QueryTextStyle = theme.Text.Primary;
        control.PlaceholderStyle = theme.Text.Muted;
        control.ItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.MatchMarkerStyle = theme.Accent.Primary;
        control.DisabledStyle = theme.Text.Muted;
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
    public static QuickOpenOverlay ApplyTheme(
        this QuickOpenOverlay control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
///     Represents tessera theme control extensions navigation overlay default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsNavigationOverlayDefaultExtensions
{
    /// <summary>
    ///     Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static Choice ApplyThemeDefaults(this Choice control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.HoveredValueStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredValueStyle, theme.Accent.Secondary);
        control.OptionStyle = TesseraThemeControlExtensions.ApplyDefault(control.OptionStyle, theme.Text.Primary);
        control.SelectedOptionStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedOptionStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredOptionStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredOptionStyle, theme.Accent.Secondary);
        if (control.FocusMarker.Length == 0)
        {
            control.FocusMarker = theme.Focus.Marker;
        }

        control.MutedStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static Choice ApplyThemeDefaults(
        this Choice control,
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
    public static ComboBox ApplyThemeDefaults(this ComboBox control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.HoveredValueStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredValueStyle, theme.Accent.Secondary);
        control.OptionStyle = TesseraThemeControlExtensions.ApplyDefault(control.OptionStyle, theme.Text.Primary);
        control.SelectedOptionStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedOptionStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredOptionStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredOptionStyle, theme.Accent.Secondary);
        if (control.FocusMarker.Length == 0)
        {
            control.FocusMarker = theme.Focus.Marker;
        }

        control.MutedStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static ComboBox ApplyThemeDefaults(
        this ComboBox control,
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
    public static TreeView ApplyThemeDefaults(this TreeView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.BranchStyle = TesseraThemeControlExtensions.ApplyDefault(control.BranchStyle, theme.Accent.Primary);
        control.LeafStyle = TesseraThemeControlExtensions.ApplyDefault(control.LeafStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        if (control.FocusMarker.Length == 0)
        {
            control.FocusMarker = theme.Focus.Marker;
        }

        control.MutedStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static TreeView ApplyThemeDefaults(
        this TreeView control,
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
    public static AutocompleteInput ApplyThemeDefaults(this AutocompleteInput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.InputTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.InputTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.PopupStyle = TesseraThemeControlExtensions.ApplyDefault(control.PopupStyle, theme.Text.Secondary);
        control.SuggestionStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.SuggestionStyle, theme.Text.Primary);
        control.HoveredSuggestionStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredSuggestionStyle, theme.Accent.Secondary);
        control.SelectedSuggestionStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedSuggestionStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedSuggestionStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedSuggestionStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.CommitMarkerStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.CommitMarkerStyle, theme.Accent.Primary);
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
    public static AutocompleteInput ApplyThemeDefaults(
        this AutocompleteInput control,
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
    public static MenuBar ApplyThemeDefaults(this MenuBar control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.FocusedItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedItemStyle, theme.Focus.Ring);
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static MenuBar ApplyThemeDefaults(
        this MenuBar control,
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
    public static ContextMenu ApplyThemeDefaults(this ContextMenu control, TesseraTheme theme)
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
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.MutedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static ContextMenu ApplyThemeDefaults(
        this ContextMenu control,
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
    public static CommandPalette ApplyThemeDefaults(this CommandPalette control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.QueryTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.QueryTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.MutedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static CommandPalette ApplyThemeDefaults(
        this CommandPalette control,
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
    public static Notifications ApplyThemeDefaults(this Notifications control, TesseraTheme theme)
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
        control.DisabledItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledItemStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static Notifications ApplyThemeDefaults(
        this Notifications control,
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
    public static QuickOpenOverlay ApplyThemeDefaults(this QuickOpenOverlay control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.QueryTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.QueryTextStyle, theme.Text.Primary);
        control.PlaceholderStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderStyle, theme.Text.Muted);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredItemStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.MatchMarkerStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.MatchMarkerStyle, theme.Accent.Primary);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
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
    public static QuickOpenOverlay ApplyThemeDefaults(
        this QuickOpenOverlay control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
