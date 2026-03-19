using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="DataGrid"/>.
    /// </summary>
    public static DataGrid ApplyTheme(this DataGrid control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="DataGrid"/>.
    /// </summary>
    public static DataGrid ApplyTheme(
        this DataGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="DataGrid"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static DataGrid ApplyThemeDefaults(this DataGrid control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="DataGrid"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static DataGrid ApplyThemeDefaults(
        this DataGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="TreeTable"/>.
    /// </summary>
    public static TreeTable ApplyTheme(this TreeTable control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.HeaderStyle = theme.Text.Secondary;
        control.BranchRowStyle = theme.Accent.Primary;
        control.LeafRowStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MutedRowStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="TreeTable"/>.
    /// </summary>
    public static TreeTable ApplyTheme(
        this TreeTable control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="TreeTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TreeTable ApplyThemeDefaults(this TreeTable control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.BranchRowStyle = ApplyDefault(control.BranchRowStyle, theme.Accent.Primary);
        control.LeafRowStyle = ApplyDefault(control.LeafRowStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedRowStyle = ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="TreeTable"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TreeTable ApplyThemeDefaults(
        this TreeTable control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="KeyValueList"/>.
    /// </summary>
    public static KeyValueList ApplyTheme(this KeyValueList control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.KeyStyle = theme.Text.Secondary;
        control.ValueStyle = theme.Text.Primary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.SeparatorStyle = theme.Text.Muted;
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="KeyValueList"/>.
    /// </summary>
    public static KeyValueList ApplyTheme(
        this KeyValueList control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="KeyValueList"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static KeyValueList ApplyThemeDefaults(this KeyValueList control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="KeyValueList"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static KeyValueList ApplyThemeDefaults(
        this KeyValueList control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Timeline"/>.
    /// </summary>
    public static Timeline ApplyTheme(this Timeline control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Timeline"/>.
    /// </summary>
    public static Timeline ApplyTheme(
        this Timeline control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Timeline"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Timeline ApplyThemeDefaults(this Timeline control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Timeline"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Timeline ApplyThemeDefaults(
        this Timeline control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="Stepper"/>.
    /// </summary>
    public static Stepper ApplyTheme(this Stepper control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="Stepper"/>.
    /// </summary>
    public static Stepper ApplyTheme(
        this Stepper control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="Stepper"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Stepper ApplyThemeDefaults(this Stepper control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="Stepper"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static Stepper ApplyThemeDefaults(
        this Stepper control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
