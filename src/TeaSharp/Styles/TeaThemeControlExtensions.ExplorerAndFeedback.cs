using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static DiffView ApplyTheme(this DiffView control, TeaTheme theme)
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
        return control;
    }

    public static DiffView ApplyTheme(
        this DiffView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DiffView ApplyThemeDefaults(this DiffView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.AddedLineStyle = ApplyDefault(control.AddedLineStyle, theme.State.Success);
        control.RemovedLineStyle = ApplyDefault(control.RemovedLineStyle, theme.State.Error);
        control.UnchangedLineStyle = ApplyDefault(control.UnchangedLineStyle, theme.Text.Primary);
        control.SelectedLineStyle = ApplyDefault(
            control.SelectedLineStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static DiffView ApplyThemeDefaults(
        this DiffView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static PropertyGrid ApplyTheme(this PropertyGrid control, TeaTheme theme)
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
        return control;
    }

    public static PropertyGrid ApplyTheme(
        this PropertyGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static PropertyGrid ApplyThemeDefaults(this PropertyGrid control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.HeaderStyle = ApplyDefault(control.HeaderStyle, theme.Text.Secondary);
        control.KeyStyle = ApplyDefault(control.KeyStyle, theme.Text.Secondary);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    public static PropertyGrid ApplyThemeDefaults(
        this PropertyGrid control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="FileExplorer"/>.
    /// </summary>
    public static FileExplorer ApplyTheme(this FileExplorer control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DirectoryStyle = theme.Accent.Primary;
        control.FileStyle = theme.Text.Primary;
        control.SelectedStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MutedStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="FileExplorer"/>.
    /// </summary>
    public static FileExplorer ApplyTheme(
        this FileExplorer control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="FileExplorer"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FileExplorer ApplyThemeDefaults(this FileExplorer control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DirectoryStyle = ApplyDefault(control.DirectoryStyle, theme.Accent.Primary);
        control.FileStyle = ApplyDefault(control.FileStyle, theme.Text.Primary);
        control.SelectedStyle = ApplyDefault(
            control.SelectedStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedStyle = ApplyDefault(control.MutedStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="FileExplorer"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FileExplorer ApplyThemeDefaults(
        this FileExplorer control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="FuzzyFinder"/>.
    /// </summary>
    public static FuzzyFinder ApplyTheme(this FuzzyFinder control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.ListItemStyle = theme.Text.Primary;
        control.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MatchHighlightStyle = theme.Accent.Primary;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="FuzzyFinder"/>.
    /// </summary>
    public static FuzzyFinder ApplyTheme(
        this FuzzyFinder control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="FuzzyFinder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FuzzyFinder ApplyThemeDefaults(this FuzzyFinder control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.ListItemStyle = ApplyDefault(control.ListItemStyle, theme.Text.Primary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MatchHighlightStyle = ApplyDefault(control.MatchHighlightStyle, theme.Accent.Primary);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="FuzzyFinder"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static FuzzyFinder ApplyThemeDefaults(
        this FuzzyFinder control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="ToastCenter"/>.
    /// </summary>
    public static ToastCenter ApplyTheme(this ToastCenter control, TeaTheme theme)
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
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical overrides to a <see cref="ToastCenter"/>.
    /// </summary>
    public static ToastCenter ApplyTheme(
        this ToastCenter control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="ToastCenter"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ToastCenter ApplyThemeDefaults(this ToastCenter control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredItemStyle = ApplyDefault(control.HoveredItemStyle, theme.Accent.Secondary);
        control.SelectedItemStyle = ApplyDefault(
            control.SelectedItemStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedItemStyle = ApplyDefault(control.MutedItemStyle, theme.Text.Muted);
        control.InfoItemStyle = ApplyDefault(control.InfoItemStyle, theme.State.Info);
        control.SuccessItemStyle = ApplyDefault(control.SuccessItemStyle, theme.State.Success);
        control.WarningItemStyle = ApplyDefault(control.WarningItemStyle, theme.State.Warning);
        control.ErrorItemStyle = ApplyDefault(control.ErrorItemStyle, theme.State.Error);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="ToastCenter"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static ToastCenter ApplyThemeDefaults(
        this ToastCenter control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
