using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
/// Represents tessera theme control extensions explorer and feedback apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsExplorerAndFeedbackApplyExtensions
{
    /// <summary>
    /// Executes apply theme.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme.</returns>
    public static DiffView ApplyTheme(this DiffView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.AddedLineStyle = theme.State.Success;
        control.RemovedLineStyle = theme.State.Error;
        control.UnchangedLineStyle = theme.Text.Primary;
        control.SelectedLineStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static DiffView ApplyTheme(
        this DiffView control,
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
    public static PropertyGrid ApplyTheme(this PropertyGrid control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.KeyStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
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
    public static PropertyGrid ApplyTheme(
        this PropertyGrid control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="FileExplorer"/>.
    /// </summary>
    public static FileExplorer ApplyTheme(this FileExplorer control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DirectoryStyle = theme.Accent.Primary;
        control.FileStyle = theme.Text.Primary;
        control.HoveredStyle = theme.Accent.Secondary;
        control.SelectedStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MutedStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="FileExplorer"/>.
    /// </summary>
    public static FileExplorer ApplyTheme(
        this FileExplorer control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="FuzzyFinder"/>.
    /// </summary>
    public static FuzzyFinder ApplyTheme(this FuzzyFinder control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.ListItemStyle = theme.Text.Primary;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MatchHighlightStyle = theme.Accent.Primary;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="FuzzyFinder"/>.
    /// </summary>
    public static FuzzyFinder ApplyTheme(
        this FuzzyFinder control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="ToastCenter"/>.
    /// </summary>
    public static ToastCenter ApplyTheme(this ToastCenter control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ItemStyle = theme.Text.Primary;
        control.HoveredItemStyle = theme.Accent.Secondary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MutedItemStyle = theme.Text.Muted;
        control.InfoItemStyle = theme.State.Info;
        control.SuccessItemStyle = theme.State.Success;
        control.WarningItemStyle = theme.State.Warning;
        control.ErrorItemStyle = theme.State.Error;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="ToastCenter"/>.
    /// </summary>
    public static ToastCenter ApplyTheme(
        this ToastCenter control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
/// Represents tessera theme control extensions explorer and feedback default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsExplorerAndFeedbackDefaultExtensions
{
    /// <summary>
    /// Executes apply theme defaults.
    /// </summary>
    /// <param name="control">The control value.</param>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result of apply theme defaults.</returns>
    public static DiffView ApplyThemeDefaults(this DiffView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = TesseraThemeControlExtensions.ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.AddedLineStyle = TesseraThemeControlExtensions.ApplyDefault(control.AddedLineStyle, theme.State.Success);
        control.RemovedLineStyle = TesseraThemeControlExtensions.ApplyDefault(control.RemovedLineStyle, theme.State.Error);
        control.UnchangedLineStyle = TesseraThemeControlExtensions.ApplyDefault(control.UnchangedLineStyle, theme.Text.Primary);
        control.SelectedLineStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedLineStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static DiffView ApplyThemeDefaults(
        this DiffView control,
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
    public static PropertyGrid ApplyThemeDefaults(this PropertyGrid control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = TesseraThemeControlExtensions.ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.KeyStyle = TesseraThemeControlExtensions.ApplyDefault(control.KeyStyle, theme.Text.Secondary);
        control.ValueStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
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
    public static PropertyGrid ApplyThemeDefaults(
        this PropertyGrid control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="FileExplorer"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FileExplorer ApplyThemeDefaults(this FileExplorer control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DirectoryStyle = TesseraThemeControlExtensions.ApplyDefault(control.DirectoryStyle, theme.Accent.Primary);
        control.FileStyle = TesseraThemeControlExtensions.ApplyDefault(control.FileStyle, theme.Text.Primary);
        control.HoveredStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredStyle, theme.Accent.Secondary);
        control.SelectedStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="FileExplorer"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FileExplorer ApplyThemeDefaults(
        this FileExplorer control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="FuzzyFinder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FuzzyFinder ApplyThemeDefaults(this FuzzyFinder control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.ListItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ListItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MatchHighlightStyle = TesseraThemeControlExtensions.ApplyDefault(control.MatchHighlightStyle, theme.Accent.Primary);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="FuzzyFinder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FuzzyFinder ApplyThemeDefaults(
        this FuzzyFinder control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="ToastCenter"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ToastCenter ApplyThemeDefaults(this ToastCenter control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.InfoItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.InfoItemStyle, theme.State.Info);
        control.SuccessItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.SuccessItemStyle, theme.State.Success);
        control.WarningItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.WarningItemStyle, theme.State.Warning);
        control.ErrorItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorItemStyle, theme.State.Error);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="ToastCenter"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ToastCenter ApplyThemeDefaults(
        this ToastCenter control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
