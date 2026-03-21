using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="PivotTable"/>.
    /// </summary>
    public static PivotTable ApplyTheme(this PivotTable control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="PivotTable"/>.
    /// </summary>
    public static PivotTable ApplyTheme(
        this PivotTable control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="PivotTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static PivotTable ApplyThemeDefaults(this PivotTable control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.BodyStyle = ApplyDefault(control.BodyStyle, theme.Text.Primary);
        control.SelectedCellStyle = ApplyDefault(
            control.SelectedCellStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedCellStyle = ApplyDefault(control.FocusedCellStyle, theme.Focus.Ring);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="PivotTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static PivotTable ApplyThemeDefaults(
        this PivotTable control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="QueryBuilder"/>.
    /// </summary>
    public static QueryBuilder ApplyTheme(this QueryBuilder control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="QueryBuilder"/>.
    /// </summary>
    public static QueryBuilder ApplyTheme(
        this QueryBuilder control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="QueryBuilder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static QueryBuilder ApplyThemeDefaults(this QueryBuilder control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.RuleStyle = ApplyDefault(control.RuleStyle, theme.Text.Primary);
        control.SelectedRuleStyle = ApplyDefault(
            control.SelectedRuleStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedRuleStyle = ApplyDefault(control.FocusedRuleStyle, theme.Focus.Ring);
        control.HoveredRuleStyle = ApplyDefault(control.HoveredRuleStyle, theme.Accent.Secondary);
        control.DisabledRuleStyle = ApplyDefault(control.DisabledRuleStyle, theme.Text.Muted);
        control.ErrorRuleStyle = ApplyDefault(control.ErrorRuleStyle, theme.State.Error);
        control.PreviewStyle = ApplyDefault(control.PreviewStyle, theme.Text.Secondary);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="QueryBuilder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static QueryBuilder ApplyThemeDefaults(
        this QueryBuilder control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="RichTextView"/>.
    /// </summary>
    public static RichTextView ApplyTheme(this RichTextView control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="RichTextView"/>.
    /// </summary>
    public static RichTextView ApplyTheme(
        this RichTextView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="RichTextView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static RichTextView ApplyThemeDefaults(this RichTextView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TextStyle = ApplyDefault(control.TextStyle, theme.Text.Primary);
        control.HeadingStyle = ApplyDefault(control.HeadingStyle, theme.Accent.Primary);
        control.ListMarkerStyle = ApplyDefault(control.ListMarkerStyle, theme.Text.Secondary);
        control.QuoteMarkerStyle = ApplyDefault(control.QuoteMarkerStyle, theme.Text.Muted);
        control.EmphasisStyle = ApplyDefault(control.EmphasisStyle, theme.Accent.Secondary);
        control.StrongStyle = ApplyDefault(control.StrongStyle, theme.Accent.Primary);
        control.InlineCodeStyle = ApplyDefault(control.InlineCodeStyle, theme.Surface.Panel.Merge(theme.Text.Primary));
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="RichTextView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static RichTextView ApplyThemeDefaults(
        this RichTextView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
