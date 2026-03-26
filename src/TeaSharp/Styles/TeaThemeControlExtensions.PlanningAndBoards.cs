using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    /// <summary>
    /// Applies a resolved theme to a <see cref="VirtualizedListView{T}"/>.
    /// </summary>
    public static VirtualizedListView<T> ApplyTheme<T>(this VirtualizedListView<T> control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="VirtualizedListView{T}"/>.
    /// </summary>
    public static VirtualizedListView<T> ApplyTheme<T>(
        this VirtualizedListView<T> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="VirtualizedListView{T}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static VirtualizedListView<T> ApplyThemeDefaults<T>(this VirtualizedListView<T> control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.DefaultRowStyle = ApplyDefault(control.DefaultRowStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledRowStyle = ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="VirtualizedListView{T}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static VirtualizedListView<T> ApplyThemeDefaults<T>(
        this VirtualizedListView<T> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="GroupedListView{TGroup, TItem}"/>.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyTheme<TGroup, TItem>(this GroupedListView<TGroup, TItem> control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="GroupedListView{TGroup, TItem}"/>.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyTheme<TGroup, TItem>(
        this GroupedListView<TGroup, TItem> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="GroupedListView{TGroup, TItem}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyThemeDefaults<TGroup, TItem>(this GroupedListView<TGroup, TItem> control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.GroupHeaderStyle = ApplyDefault(control.GroupHeaderStyle, theme.Text.Secondary);
        control.ItemStyle = ApplyDefault(control.ItemStyle, theme.Text.Primary);
        control.HoveredRowStyle = ApplyDefault(control.HoveredRowStyle, theme.Accent.Secondary);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.DisabledRowStyle = ApplyDefault(control.DisabledRowStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="GroupedListView{TGroup, TItem}"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static GroupedListView<TGroup, TItem> ApplyThemeDefaults<TGroup, TItem>(
        this GroupedListView<TGroup, TItem> control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="KanbanBoard"/>.
    /// </summary>
    public static KanbanBoard ApplyTheme(this KanbanBoard control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="KanbanBoard"/>.
    /// </summary>
    public static KanbanBoard ApplyTheme(
        this KanbanBoard control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="KanbanBoard"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static KanbanBoard ApplyThemeDefaults(this KanbanBoard control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.LaneHeaderStyle = ApplyDefault(control.LaneHeaderStyle, theme.Text.Secondary);
        control.SelectedLaneHeaderStyle = ApplyDefault(control.SelectedLaneHeaderStyle, theme.Accent.Secondary);
        control.CardStyle = ApplyDefault(control.CardStyle, theme.Text.Primary);
        control.SelectedCardStyle = ApplyDefault(
            control.SelectedCardStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedCardStyle = ApplyDefault(control.FocusedCardStyle, theme.Focus.Ring);
        control.HoveredCardStyle = ApplyDefault(control.HoveredCardStyle, theme.Accent.Secondary);
        control.DisabledCardStyle = ApplyDefault(control.DisabledCardStyle, theme.Text.Muted);
        control.ErrorCardStyle = ApplyDefault(control.ErrorCardStyle, theme.State.Error);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="KanbanBoard"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static KanbanBoard ApplyThemeDefaults(
        this KanbanBoard control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="TagInput"/>.
    /// </summary>
    public static TagInput ApplyTheme(this TagInput control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="TagInput"/>.
    /// </summary>
    public static TagInput ApplyTheme(
        this TagInput control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="TagInput"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TagInput ApplyThemeDefaults(this TagInput control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TagStyle = ApplyDefault(control.TagStyle, theme.Text.Secondary);
        control.SelectedTagStyle = ApplyDefault(
            control.SelectedTagStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedTagStyle = ApplyDefault(control.FocusedTagStyle, theme.Focus.Ring);
        control.HoveredTagStyle = ApplyDefault(control.HoveredTagStyle, theme.Accent.Secondary);
        control.DisabledTagStyle = ApplyDefault(control.DisabledTagStyle, theme.Text.Muted);
        control.ErrorTagStyle = ApplyDefault(control.ErrorTagStyle, theme.State.Error);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.CaretStyle = ApplyDefault(control.CaretStyle, theme.Focus.Ring);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="TagInput"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TagInput ApplyThemeDefaults(
        this TagInput control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="TokenEditor"/>.
    /// </summary>
    public static TokenEditor ApplyTheme(this TokenEditor control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="TokenEditor"/>.
    /// </summary>
    public static TokenEditor ApplyTheme(
        this TokenEditor control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="TokenEditor"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TokenEditor ApplyThemeDefaults(this TokenEditor control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TokenStyle = ApplyDefault(control.TokenStyle, theme.Text.Secondary);
        control.SelectedTokenStyle = ApplyDefault(
            control.SelectedTokenStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.FocusedSelectedTokenStyle = ApplyDefault(control.FocusedSelectedTokenStyle, theme.Focus.Ring);
        control.HoveredTokenStyle = ApplyDefault(control.HoveredTokenStyle, theme.Accent.Secondary);
        control.DisabledTokenStyle = ApplyDefault(control.DisabledTokenStyle, theme.Text.Muted);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.PlaceholderTextStyle = ApplyDefault(control.PlaceholderTextStyle, theme.Text.Muted);
        control.BorderStyleText = ApplyDefault(control.BorderStyleText, theme.Border.Default);
        control.FocusedBorderStyleText = ApplyDefault(control.FocusedBorderStyleText, theme.Border.Focused.Merge(theme.Focus.Border));
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="TokenEditor"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static TokenEditor ApplyThemeDefaults(
        this TokenEditor control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="CalendarMonthView"/>.
    /// </summary>
    public static CalendarMonthView ApplyTheme(this CalendarMonthView control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="CalendarMonthView"/>.
    /// </summary>
    public static CalendarMonthView ApplyTheme(
        this CalendarMonthView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="CalendarMonthView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static CalendarMonthView ApplyThemeDefaults(this CalendarMonthView control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.MonthHeaderStyle = ApplyDefault(control.MonthHeaderStyle, theme.Text.Secondary);
        control.WeekdayHeaderStyle = ApplyDefault(control.WeekdayHeaderStyle, theme.Text.Secondary);
        control.DayStyle = ApplyDefault(control.DayStyle, theme.Text.Primary);
        control.OutsideMonthDayStyle = ApplyDefault(control.OutsideMonthDayStyle, theme.Text.Muted);
        control.TodayDayStyle = ApplyDefault(control.TodayDayStyle, theme.Accent.Primary);
        control.SelectedDayStyle = ApplyDefault(
            control.SelectedDayStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredDayStyle = ApplyDefault(control.HoveredDayStyle, theme.Accent.Secondary);
        control.DisabledDayStyle = ApplyDefault(control.DisabledDayStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="CalendarMonthView"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static CalendarMonthView ApplyThemeDefaults(
        this CalendarMonthView control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies a resolved theme to a <see cref="SchedulerTimeline"/>.
    /// </summary>
    public static SchedulerTimeline ApplyTheme(this SchedulerTimeline control, TeaTheme theme)
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
    /// Resolves and applies hierarchical overrides to a <see cref="SchedulerTimeline"/>.
    /// </summary>
    public static SchedulerTimeline ApplyTheme(
        this SchedulerTimeline control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    /// <summary>
    /// Applies theme defaults to a <see cref="SchedulerTimeline"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static SchedulerTimeline ApplyThemeDefaults(this SchedulerTimeline control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TimeTextStyle = ApplyDefault(control.TimeTextStyle, theme.Text.Secondary);
        control.EntryTextStyle = ApplyDefault(control.EntryTextStyle, theme.Text.Primary);
        control.MetaTextStyle = ApplyDefault(control.MetaTextStyle, theme.Text.Muted);
        control.SelectedRowStyle = ApplyDefault(
            control.SelectedRowStyle,
            theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.MutedRowStyle = ApplyDefault(control.MutedRowStyle, theme.Text.Muted);
        control.ConflictRowStyle = ApplyDefault(control.ConflictRowStyle, theme.State.Warning);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        control.EmptyTextStyle = ApplyDefault(control.EmptyTextStyle, theme.Text.Muted);
        control.FocusMarker = ApplyDefault(control.FocusMarker, theme.Focus.Marker);
        return control;
    }

    /// <summary>
    /// Resolves and applies hierarchical defaults to a <see cref="SchedulerTimeline"/> without overwriting explicit non-empty styles.
    /// </summary>
    public static SchedulerTimeline ApplyThemeDefaults(
        this SchedulerTimeline control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
