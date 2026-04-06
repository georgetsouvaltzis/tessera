using Tessera.Controls;

namespace Tessera.Styles;

public static partial class TesseraThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="DataGrid"/>.
    /// </summary>
    public static DataGrid ApplyTheme(this DataGrid control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.RowStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.SelectedCellStyle = theme.Accent.Primary;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="DataGrid"/>.
    /// </summary>
    public static DataGrid ApplyTheme(
        this DataGrid control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="DataGrid"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static DataGrid ApplyThemeDefaults(this DataGrid control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.RowStyle = ApplyDefault(control.RowStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.SelectedCellStyle = ApplyDefault(control.SelectedCellStyle, theme.Accent.Primary);
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="DataGrid"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static DataGrid ApplyThemeDefaults(
        this DataGrid control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="TreeTable"/>.
    /// </summary>
    public static TreeTable ApplyTheme(this TreeTable control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.BranchRowStyle = theme.Accent.Primary;
        control.LeafRowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MutedRowStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="TreeTable"/>.
    /// </summary>
    public static TreeTable ApplyTheme(
        this TreeTable control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="TreeTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TreeTable ApplyThemeDefaults(this TreeTable control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.BranchRowStyle = ApplyDefault(control.BranchRowStyle, theme.Accent.Primary);
        control.LeafRowStyle = ApplyDefault(control.LeafRowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedRowStyle = ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="TreeTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TreeTable ApplyThemeDefaults(
        this TreeTable control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="KeyValueList"/>.
    /// </summary>
    public static KeyValueList ApplyTheme(this KeyValueList control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.KeyStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.SeparatorStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="KeyValueList"/>.
    /// </summary>
    public static KeyValueList ApplyTheme(
        this KeyValueList control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="KeyValueList"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static KeyValueList ApplyThemeDefaults(this KeyValueList control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.KeyStyle = ApplyDefault(control.KeyStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="KeyValueList"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static KeyValueList ApplyThemeDefaults(
        this KeyValueList control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to an <see cref="EmptyState"/>.
    /// </summary>
    public static EmptyState ApplyTheme(this EmptyState control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DescriptionStyle = theme.Text.Primary;
        control.HintStyle = theme.Text.Muted;
        control.ActionStyle = theme.Accent.Primary;
        control.FocusedActionStyle = theme.Focus.Ring.Merge(theme.Accent.Primary);
        control.HoveredActionStyle = theme.Accent.Secondary;
        control.DisabledStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to an <see cref="EmptyState"/>.
    /// </summary>
    public static EmptyState ApplyTheme(
        this EmptyState control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to an <see cref="EmptyState"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static EmptyState ApplyThemeDefaults(this EmptyState control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DescriptionStyle = ApplyDefault(control.DescriptionStyle, theme.Text.Primary);
        control.HintStyle = ApplyDefault(control.HintStyle, theme.Text.Muted);
        control.ActionStyle = ApplyDefault(control.ActionStyle, theme.Accent.Primary);
        control.FocusedActionStyle = ApplyDefault(control.FocusedActionStyle, theme.Focus.Ring.Merge(theme.Accent.Primary));
        control.HoveredActionStyle = ApplyDefault(control.HoveredActionStyle, theme.Accent.Secondary);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to an <see cref="EmptyState"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static EmptyState ApplyThemeDefaults(
        this EmptyState control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="ValidationSummary"/>.
    /// </summary>
    public static ValidationSummary ApplyTheme(this ValidationSummary control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DefaultIssueStyle = theme.Text.Primary;
        control.InfoSeverityStyle = theme.State.Info;
        control.WarningSeverityStyle = theme.State.Warning;
        control.ErrorSeverityStyle = theme.State.Error;
        control.SelectedIssueStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedIssueStyle = theme.Focus.Ring;
        control.HoveredIssueStyle = theme.Accent.Secondary;
        control.DisabledIssueStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="ValidationSummary"/>.
    /// </summary>
    public static ValidationSummary ApplyTheme(
        this ValidationSummary control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="ValidationSummary"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ValidationSummary ApplyThemeDefaults(this ValidationSummary control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DefaultIssueStyle = ApplyDefault(control.DefaultIssueStyle, theme.Text.Primary);
        control.InfoSeverityStyle = ApplyDefault(control.InfoSeverityStyle, theme.State.Info);
        control.WarningSeverityStyle = ApplyDefault(control.WarningSeverityStyle, theme.State.Warning);
        control.ErrorSeverityStyle = ApplyDefault(control.ErrorSeverityStyle, theme.State.Error);
        control.SelectedIssueStyle = ApplyDefault(
            control.SelectedIssueStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedIssueStyle = ApplyDefault(control.FocusedIssueStyle, theme.Focus.Ring);
        control.HoveredIssueStyle = ApplyDefault(control.HoveredIssueStyle, theme.Accent.Secondary);
        control.DisabledIssueStyle = ApplyDefault(control.DisabledIssueStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="ValidationSummary"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ValidationSummary ApplyThemeDefaults(
        this ValidationSummary control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Timeline"/>.
    /// </summary>
    public static Timeline ApplyTheme(this Timeline control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TimestampStyle = theme.Text.Secondary;
        control.LabelStyle = theme.Text.Primary;
        control.ContentStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.SeparatorStyle = theme.Text.Muted;
        control.MutedStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Timeline"/>.
    /// </summary>
    public static Timeline ApplyTheme(
        this Timeline control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Timeline"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Timeline ApplyThemeDefaults(this Timeline control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TimestampStyle = ApplyDefault(control.TimestampStyle, theme.Text.Secondary);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.ContentStyle = ApplyDefault(control.ContentStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Muted);
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Timeline"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Timeline ApplyThemeDefaults(
        this Timeline control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Stepper"/>.
    /// </summary>
    public static Stepper ApplyTheme(this Stepper control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.StepTextStyle = theme.Text.Primary;
        control.ActiveStepStyle = theme.Accent.Primary;
        control.CompletedStepStyle = theme.State.Success;
        control.PendingStepStyle = theme.Text.Secondary;
        control.ConnectorStyle = theme.Text.Muted;
        control.DisabledStepStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Stepper"/>.
    /// </summary>
    public static Stepper ApplyTheme(
        this Stepper control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Stepper"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Stepper ApplyThemeDefaults(this Stepper control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.StepTextStyle = ApplyDefault(control.StepTextStyle, theme.Text.Primary);
        control.ActiveStepStyle = ApplyDefault(control.ActiveStepStyle, theme.Accent.Primary);
        control.CompletedStepStyle = ApplyDefault(control.CompletedStepStyle, theme.State.Success);
        control.PendingStepStyle = ApplyDefault(control.PendingStepStyle, theme.Text.Secondary);
        control.ConnectorStyle = ApplyDefault(control.ConnectorStyle, theme.Text.Muted);
        control.DisabledStepStyle = ApplyDefault(control.DisabledStepStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Stepper"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Stepper ApplyThemeDefaults(
        this Stepper control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
