using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
/// Represents tessera theme control extensions query and rich text apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsQueryAndRichTextApplyExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="PivotTable"/>.
    /// </summary>
    public static PivotTable ApplyTheme(this PivotTable control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.BodyStyle = theme.Text.Primary;
        control.SelectedCellStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedCellStyle = theme.Focus.Ring;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="PivotTable"/>.
    /// </summary>
    public static PivotTable ApplyTheme(
        this PivotTable control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="QueryBuilder"/>.
    /// </summary>
    public static QueryBuilder ApplyTheme(this QueryBuilder control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.RuleStyle = theme.Text.Primary;
        control.SelectedRuleStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedRuleStyle = theme.Focus.Ring;
        control.HoveredRuleStyle = theme.Accent.Secondary;
        control.DisabledRuleStyle = theme.Text.Muted;
        control.ErrorRuleStyle = theme.State.Error;
        control.PreviewStyle = theme.Text.Secondary;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="QueryBuilder"/>.
    /// </summary>
    public static QueryBuilder ApplyTheme(
        this QueryBuilder control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="RichTextView"/>.
    /// </summary>
    public static RichTextView ApplyTheme(this RichTextView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TextStyle = theme.Text.Primary;
        control.HeadingStyle = theme.Accent.Primary;
        control.ListMarkerStyle = theme.Text.Secondary;
        control.QuoteMarkerStyle = theme.Text.Muted;
        control.EmphasisStyle = theme.Accent.Secondary;
        control.StrongStyle = theme.Accent.Primary;
        control.InlineCodeStyle = theme.Surface.Panel.Merge(theme.Text.Primary);
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="RichTextView"/>.
    /// </summary>
    public static RichTextView ApplyTheme(
        this RichTextView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
/// Represents tessera theme control extensions query and rich text default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsQueryAndRichTextDefaultExtensions
{
    /// <summary>
    /// Applies theme defaults to a <see cref="PivotTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static PivotTable ApplyThemeDefaults(this PivotTable control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = TesseraThemeControlExtensions.ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.BodyStyle = TesseraThemeControlExtensions.ApplyDefault(control.BodyStyle, theme.Text.Primary);
        control.SelectedCellStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedCellStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedCellStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedCellStyle, theme.Focus.Ring);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="PivotTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static PivotTable ApplyThemeDefaults(
        this PivotTable control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="QueryBuilder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static QueryBuilder ApplyThemeDefaults(this QueryBuilder control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.RuleStyle = TesseraThemeControlExtensions.ApplyDefault(control.RuleStyle, theme.Text.Primary);
        control.SelectedRuleStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRuleStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedRuleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedRuleStyle, theme.Focus.Ring);
        control.HoveredRuleStyle = TesseraThemeControlExtensions.ApplyDefault(control.HoveredRuleStyle, theme.Accent.Secondary);
        control.DisabledRuleStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledRuleStyle, theme.Text.Muted);
        control.ErrorRuleStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorRuleStyle, theme.State.Error);
        control.PreviewStyle = TesseraThemeControlExtensions.ApplyDefault(control.PreviewStyle, theme.Text.Secondary);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="QueryBuilder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static QueryBuilder ApplyThemeDefaults(
        this QueryBuilder control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="RichTextView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static RichTextView ApplyThemeDefaults(this RichTextView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TextStyle = TesseraThemeControlExtensions.ApplyDefault(control.TextStyle, theme.Text.Primary);
        control.HeadingStyle = TesseraThemeControlExtensions.ApplyDefault(control.HeadingStyle, theme.Accent.Primary);
        control.ListMarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.ListMarkerStyle, theme.Text.Secondary);
        control.QuoteMarkerStyle = TesseraThemeControlExtensions.ApplyDefault(control.QuoteMarkerStyle, theme.Text.Muted);
        control.EmphasisStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmphasisStyle, theme.Accent.Secondary);
        control.StrongStyle = TesseraThemeControlExtensions.ApplyDefault(control.StrongStyle, theme.Accent.Primary);
        control.InlineCodeStyle = TesseraThemeControlExtensions.ApplyDefault(control.InlineCodeStyle, theme.Surface.Panel.Merge(theme.Text.Primary));
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="RichTextView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static RichTextView ApplyThemeDefaults(
        this RichTextView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
