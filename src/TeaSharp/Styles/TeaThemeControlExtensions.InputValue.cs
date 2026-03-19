using TeaSharp.Controls;

namespace TeaSharp.Styles;

public static partial class TeaThemeControlExtensions
{
    public static Label ApplyTheme(this Label control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.TextStyle = theme.Text.Primary;
        return control;
    }

    public static Label ApplyTheme(
        this Label control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Label ApplyThemeDefaults(this Label control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.TextStyle = ApplyDefault(control.TextStyle, theme.Text.Primary);
        return control;
    }

    public static Label ApplyThemeDefaults(
        this Label control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static TextArea ApplyTheme(this TextArea control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.DisabledValueTextStyle = theme.Text.Muted;
        return control;
    }

    public static TextArea ApplyTheme(
        this TextArea control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static TextArea ApplyThemeDefaults(this TextArea control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.DisabledValueTextStyle = ApplyDefault(control.DisabledValueTextStyle, theme.Text.Muted);
        return control;
    }

    public static TextArea ApplyThemeDefaults(
        this TextArea control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Toggle ApplyTheme(this Toggle control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueStyle = theme.Text.Primary;
        control.OnValueStyle = theme.State.Success;
        control.OffValueStyle = theme.Text.Secondary;
        control.DisabledValueStyle = theme.Text.Muted;
        return control;
    }

    public static Toggle ApplyTheme(
        this Toggle control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Toggle ApplyThemeDefaults(this Toggle control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.OnValueStyle = ApplyDefault(control.OnValueStyle, theme.State.Success);
        control.OffValueStyle = ApplyDefault(control.OffValueStyle, theme.Text.Secondary);
        control.DisabledValueStyle = ApplyDefault(control.DisabledValueStyle, theme.Text.Muted);
        return control;
    }

    public static Toggle ApplyThemeDefaults(
        this Toggle control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Slider ApplyTheme(this Slider control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueLabelStyle = theme.Text.Primary;
        control.FillStyle = theme.Accent.Primary;
        control.TrackStyle = theme.Text.Muted;
        control.DisabledStyle = theme.Text.Muted;
        return control;
    }

    public static Slider ApplyTheme(
        this Slider control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Slider ApplyThemeDefaults(this Slider control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueLabelStyle = ApplyDefault(control.ValueLabelStyle, theme.Text.Primary);
        control.FillStyle = ApplyDefault(control.FillStyle, theme.Accent.Primary);
        control.TrackStyle = ApplyDefault(control.TrackStyle, theme.Text.Muted);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        return control;
    }

    public static Slider ApplyThemeDefaults(
        this Slider control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static Spinner ApplyTheme(this Spinner control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueStyle = theme.Text.Primary;
        control.RunningValueStyle = theme.Accent.Primary;
        control.StoppedValueStyle = theme.Text.Secondary;
        control.DisabledValueStyle = theme.Text.Muted;
        return control;
    }

    public static Spinner ApplyTheme(
        this Spinner control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static Spinner ApplyThemeDefaults(this Spinner control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueStyle = ApplyDefault(control.ValueStyle, theme.Text.Primary);
        control.RunningValueStyle = ApplyDefault(control.RunningValueStyle, theme.Accent.Primary);
        control.StoppedValueStyle = ApplyDefault(control.StoppedValueStyle, theme.Text.Secondary);
        control.DisabledValueStyle = ApplyDefault(control.DisabledValueStyle, theme.Text.Muted);
        return control;
    }

    public static Spinner ApplyThemeDefaults(
        this Spinner control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static ProgressBar ApplyTheme(this ProgressBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.FillStyle = theme.Accent.Primary;
        control.TrackStyle = theme.Text.Muted;
        control.LabelStyle = theme.Text.Primary;
        control.DisabledStyle = theme.Text.Muted;
        return control;
    }

    public static ProgressBar ApplyTheme(
        this ProgressBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static ProgressBar ApplyThemeDefaults(this ProgressBar control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.FillStyle = ApplyDefault(control.FillStyle, theme.Accent.Primary);
        control.TrackStyle = ApplyDefault(control.TrackStyle, theme.Text.Muted);
        control.LabelStyle = ApplyDefault(control.LabelStyle, theme.Text.Primary);
        control.DisabledStyle = ApplyDefault(control.DisabledStyle, theme.Text.Muted);
        return control;
    }

    public static ProgressBar ApplyThemeDefaults(
        this ProgressBar control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static NumberInput ApplyTheme(this NumberInput control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.SummaryTextStyle = theme.Text.Secondary;
        control.DisabledTextStyle = theme.Text.Muted;
        return control;
    }

    public static NumberInput ApplyTheme(
        this NumberInput control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static NumberInput ApplyThemeDefaults(this NumberInput control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.SummaryTextStyle = ApplyDefault(control.SummaryTextStyle, theme.Text.Secondary);
        control.DisabledTextStyle = ApplyDefault(control.DisabledTextStyle, theme.Text.Muted);
        return control;
    }

    public static NumberInput ApplyThemeDefaults(
        this NumberInput control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static DatePicker ApplyTheme(this DatePicker control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.MonthHeaderStyle = theme.Text.Secondary;
        control.WeekdayHeaderStyle = theme.Text.Secondary;
        control.DayStyle = theme.Text.Primary;
        control.SelectedDayStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        control.HoveredDayStyle = theme.Accent.Secondary;
        control.DisabledDayStyle = theme.Text.Muted;
        return control;
    }

    public static DatePicker ApplyTheme(
        this DatePicker control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static DatePicker ApplyThemeDefaults(this DatePicker control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.MonthHeaderStyle = ApplyDefault(control.MonthHeaderStyle, theme.Text.Secondary);
        control.WeekdayHeaderStyle = ApplyDefault(control.WeekdayHeaderStyle, theme.Text.Secondary);
        control.DayStyle = ApplyDefault(control.DayStyle, theme.Text.Primary);
        control.SelectedDayStyle = ApplyDefault(control.SelectedDayStyle, theme.Selection.Foreground.Merge(theme.Selection.Background));
        control.HoveredDayStyle = ApplyDefault(control.HoveredDayStyle, theme.Accent.Secondary);
        control.DisabledDayStyle = ApplyDefault(control.DisabledDayStyle, theme.Text.Muted);
        return control;
    }

    public static DatePicker ApplyThemeDefaults(
        this DatePicker control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }

    public static TimePicker ApplyTheme(this TimePicker control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = theme.Text.Secondary;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.ValueTextStyle = theme.Text.Primary;
        control.ActiveFieldStyle = theme.Accent.Primary;
        control.HoveredFieldStyle = theme.Accent.Secondary;
        control.SeparatorStyle = theme.Text.Secondary;
        control.DisabledValueStyle = theme.Text.Muted;
        return control;
    }

    public static TimePicker ApplyTheme(
        this TimePicker control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyTheme(overrides.Resolve(control, baseTheme, state));
    }

    public static TimePicker ApplyThemeDefaults(this TimePicker control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(theme);

        control.TitleStyle = ApplyDefault(control.TitleStyle, theme.Text.Secondary);
        control.FocusedTitleStyle = ApplyDefault(control.FocusedTitleStyle, theme.Focus.Title);
        control.ValueTextStyle = ApplyDefault(control.ValueTextStyle, theme.Text.Primary);
        control.ActiveFieldStyle = ApplyDefault(control.ActiveFieldStyle, theme.Accent.Primary);
        control.HoveredFieldStyle = ApplyDefault(control.HoveredFieldStyle, theme.Accent.Secondary);
        control.SeparatorStyle = ApplyDefault(control.SeparatorStyle, theme.Text.Secondary);
        control.DisabledValueStyle = ApplyDefault(control.DisabledValueStyle, theme.Text.Muted);
        return control;
    }

    public static TimePicker ApplyThemeDefaults(
        this TimePicker control,
        TeaThemeOverrides overrides,
        TeaTheme baseTheme,
        TeaThemeVisualState state = TeaThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(overrides);
        return control.ApplyThemeDefaults(overrides.Resolve(control, baseTheme, state));
    }
}
