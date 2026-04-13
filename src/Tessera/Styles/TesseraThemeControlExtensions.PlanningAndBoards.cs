using Tessera.Controls;

namespace Tessera.Styles;

/// <summary>
///     Represents tessera theme control extensions planning and boards apply extensions.
/// </summary>
public static class TesseraThemeControlExtensionsPlanningAndBoardsApplyExtensions
{
    /// <summary>
    ///     Applies a resolved theme to a <see cref="VirtualizedListView{T}" />.
    /// </summary>
    public static VirtualizedListView<T> ApplyTheme<T>(this VirtualizedListView<T> control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DefaultRowStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledRowStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="VirtualizedListView{T}" />.
    /// </summary>
    public static VirtualizedListView<T> ApplyTheme<T>(
        this VirtualizedListView<T> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="GroupedListView{TGroup, TItem}" />.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyTheme<TGroup, TItem>(this GroupedListView<TGroup, TItem> control,
        TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.GroupHeaderStyle = theme.Text.Secondary;
        control.ItemStyle = theme.Text.Primary;
        control.HoveredRowStyle = theme.Accent.Secondary;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.DisabledRowStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="GroupedListView{TGroup, TItem}" />.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyTheme<TGroup, TItem>(
        this GroupedListView<TGroup, TItem> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="KanbanBoard" />.
    /// </summary>
    public static KanbanBoard ApplyTheme(this KanbanBoard control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.LaneHeaderStyle = theme.Text.Secondary;
        control.SelectedLaneHeaderStyle = theme.Accent.Secondary;
        control.CardStyle = theme.Text.Primary;
        control.SelectedCardStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedCardStyle = theme.Focus.Ring;
        control.HoveredCardStyle = theme.Accent.Secondary;
        control.DisabledCardStyle = theme.Text.Muted;
        control.ErrorCardStyle = theme.State.Error;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="KanbanBoard" />.
    /// </summary>
    public static KanbanBoard ApplyTheme(
        this KanbanBoard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="TagInput" />.
    /// </summary>
    public static TagInput ApplyTheme(this TagInput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TagStyle = theme.Text.Secondary;
        control.SelectedTagStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedTagStyle = theme.Focus.Ring;
        control.HoveredTagStyle = theme.Accent.Secondary;
        control.DisabledTagStyle = theme.Text.Muted;
        control.ErrorTagStyle = theme.State.Error;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.CaretStyle = theme.Focus.Ring;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="TagInput" />.
    /// </summary>
    public static TagInput ApplyTheme(
        this TagInput control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="TokenEditor" />.
    /// </summary>
    public static TokenEditor ApplyTheme(this TokenEditor control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TokenStyle = theme.Text.Secondary;
        control.SelectedTokenStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.FocusedSelectedTokenStyle = theme.Focus.Ring;
        control.HoveredTokenStyle = theme.Accent.Secondary;
        control.DisabledTokenStyle = theme.Text.Muted;
        control.ValueTextStyle = theme.Text.Primary;
        control.PlaceholderTextStyle = theme.Text.Muted;
        control.BorderStyleText = theme.Border.Default;
        control.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="TokenEditor" />.
    /// </summary>
    public static TokenEditor ApplyTheme(
        this TokenEditor control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="CalendarMonthView" />.
    /// </summary>
    public static CalendarMonthView ApplyTheme(this CalendarMonthView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.MonthHeaderStyle = theme.Text.Secondary;
        control.WeekdayHeaderStyle = theme.Text.Secondary;
        control.DayStyle = theme.Text.Primary;
        control.OutsideMonthDayStyle = theme.Text.Muted;
        control.TodayDayStyle = theme.Accent.Primary;
        control.SelectedDayStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredDayStyle = theme.Accent.Secondary;
        control.DisabledDayStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="CalendarMonthView" />.
    /// </summary>
    public static CalendarMonthView ApplyTheme(
        this CalendarMonthView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies a resolved theme to a <see cref="SchedulerTimeline" />.
    /// </summary>
    public static SchedulerTimeline ApplyTheme(this SchedulerTimeline control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TimeTextStyle = theme.Text.Secondary;
        control.EntryTextStyle = theme.Text.Primary;
        control.MetaTextStyle = theme.Text.Muted;
        control.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.MutedRowStyle = theme.Text.Muted;
        control.ConflictRowStyle = theme.State.Warning;
        control.DisabledStyle = theme.Text.Muted;
        control.EmptyTextStyle = theme.Text.Muted;
        control.FocusMarker = theme.Focus.Marker;
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical overrides to a <see cref="SchedulerTimeline" />.
    /// </summary>
    public static SchedulerTimeline ApplyTheme(
        this SchedulerTimeline control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }
}

/// <summary>
///     Represents tessera theme control extensions planning and boards default extensions.
/// </summary>
public static class TesseraThemeControlExtensionsPlanningAndBoardsDefaultExtensions
{
    /// <summary>
    ///     Applies theme defaults to a <see cref="VirtualizedListView{T}" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static VirtualizedListView<T> ApplyThemeDefaults<T>(this VirtualizedListView<T> control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DefaultRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DefaultRowStyle, theme.Text.Primary);
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="VirtualizedListView{T}" /> without overwriting explicit
    ///     non-empty styles.
    /// </summary>
    public static VirtualizedListView<T> ApplyThemeDefaults<T>(
        this VirtualizedListView<T> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="GroupedListView{TGroup, TItem}" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyThemeDefaults<TGroup, TItem>(
        this GroupedListView<TGroup, TItem> control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.GroupHeaderStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.GroupHeaderStyle, theme.Text.Secondary);
        control.ItemStyle = TesseraThemeControlExtensions.ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="GroupedListView{TGroup, TItem}" /> without overwriting
    ///     explicit non-empty styles.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyThemeDefaults<TGroup, TItem>(
        this GroupedListView<TGroup, TItem> control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="KanbanBoard" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static KanbanBoard ApplyThemeDefaults(this KanbanBoard control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.LaneHeaderStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.LaneHeaderStyle, theme.Text.Secondary);
        control.SelectedLaneHeaderStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.SelectedLaneHeaderStyle, theme.Accent.Secondary);
        control.CardStyle = TesseraThemeControlExtensions.ApplyDefault(control.CardStyle, theme.Text.Primary);
        control.SelectedCardStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedCardStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedCardStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedCardStyle, theme.Focus.Ring);
        control.HoveredCardStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredCardStyle, theme.Accent.Secondary);
        control.DisabledCardStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledCardStyle, theme.Text.Muted);
        control.ErrorCardStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorCardStyle, theme.State.Error);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="KanbanBoard" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static KanbanBoard ApplyThemeDefaults(
        this KanbanBoard control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="TagInput" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static TagInput ApplyThemeDefaults(this TagInput control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TagStyle = TesseraThemeControlExtensions.ApplyDefault(control.TagStyle, theme.Text.Secondary);
        control.SelectedTagStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedTagStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedTagStyle = TesseraThemeControlExtensions.ApplyDefault(control.FocusedTagStyle, theme.Focus.Ring);
        control.HoveredTagStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredTagStyle, theme.Accent.Secondary);
        control.DisabledTagStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledTagStyle, theme.Text.Muted);
        control.ErrorTagStyle = TesseraThemeControlExtensions.ApplyDefault(control.ErrorTagStyle, theme.State.Error);
        control.ValueTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.CaretStyle = TesseraThemeControlExtensions.ApplyDefault(control.CaretStyle, theme.Focus.Ring);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="TagInput" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static TagInput ApplyThemeDefaults(
        this TagInput control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="TokenEditor" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static TokenEditor ApplyThemeDefaults(this TokenEditor control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TokenStyle = TesseraThemeControlExtensions.ApplyDefault(control.TokenStyle, theme.Text.Secondary);
        control.SelectedTokenStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedTokenStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedTokenStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedSelectedTokenStyle, theme.Focus.Ring);
        control.HoveredTokenStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredTokenStyle, theme.Accent.Secondary);
        control.DisabledTokenStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledTokenStyle, theme.Text.Muted);
        control.ValueTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.BorderStyleText =
            TesseraThemeControlExtensions.ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = TesseraThemeControlExtensions.ApplyDefault(control.FocusedBorderStyleText,
            theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="TokenEditor" /> without overwriting explicit non-empty
    ///     styles.
    /// </summary>
    public static TokenEditor ApplyThemeDefaults(
        this TokenEditor control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="CalendarMonthView" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static CalendarMonthView ApplyThemeDefaults(this CalendarMonthView control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.MonthHeaderStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.MonthHeaderStyle, theme.Text.Secondary);
        control.WeekdayHeaderStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.WeekdayHeaderStyle, theme.Text.Secondary);
        control.DayStyle = TesseraThemeControlExtensions.ApplyDefault(control.DayStyle, theme.Text.Primary);
        control.OutsideMonthDayStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.OutsideMonthDayStyle, theme.Text.Muted);
        control.TodayDayStyle = TesseraThemeControlExtensions.ApplyDefault(control.TodayDayStyle, theme.Accent.Primary);
        control.SelectedDayStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedDayStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredDayStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.HoveredDayStyle, theme.Accent.Secondary);
        control.DisabledDayStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.DisabledDayStyle, theme.Text.Muted);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="CalendarMonthView" /> without overwriting explicit
    ///     non-empty styles.
    /// </summary>
    public static CalendarMonthView ApplyThemeDefaults(
        this CalendarMonthView control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    ///     Applies theme defaults to a <see cref="SchedulerTimeline" /> without overwriting explicit non-empty styles.
    /// </summary>
    public static SchedulerTimeline ApplyThemeDefaults(this SchedulerTimeline control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = TesseraThemeControlExtensions.ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TimeTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.TimeTextStyle, theme.Text.Secondary);
        control.EntryTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EntryTextStyle, theme.Text.Primary);
        control.MetaTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.MetaTextStyle, theme.Text.Muted);
        control.SelectedRowStyle = TesseraThemeControlExtensions.ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedRowStyle = TesseraThemeControlExtensions.ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.ConflictRowStyle =
            TesseraThemeControlExtensions.ApplyDefault(control.ConflictRowStyle, theme.State.Warning);
        control.DisabledStyle = TesseraThemeControlExtensions.ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = TesseraThemeControlExtensions.ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = TesseraThemeControlExtensions.ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    ///     Resolves and applies hierarchical defaults to a <see cref="SchedulerTimeline" /> without overwriting explicit
    ///     non-empty styles.
    /// </summary>
    public static SchedulerTimeline ApplyThemeDefaults(
        this SchedulerTimeline control,
        TesseraThemeOverrides overrides,
        TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
