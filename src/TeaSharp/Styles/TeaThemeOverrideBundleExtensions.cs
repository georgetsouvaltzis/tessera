using TeaSharp.Controls;

namespace TeaSharp.Styles;

/// <summary>
/// Applies <see cref="TeaThemeOverrideBundle" /> values to common dashboard-oriented controls.
/// </summary>
public static class TeaThemeOverrideBundleExtensions
{
    /// <summary>
    /// Applies dashboard overrides to a <see cref="ListView{T}" />.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="control">The list view instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static ListView<T> ApplyDashboardOverrides<T>(this ListView<T> control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.FocusMarker = bundle.FocusMarker;
        control.TitleStyle = bundle.TitleStyle;
        control.FocusedTitleStyle = bundle.FocusedTitleStyle;
        control.BorderStyleText = bundle.BorderStyleText;
        control.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        control.DefaultRowStyle = bundle.DefaultItemStyle;
        control.HoveredRowStyle = bundle.HoveredItemStyle;
        control.SelectedRowStyle = bundle.SelectedItemStyle;
        return control;
    }

    /// <summary>
    /// Applies the bundle theme, then dashboard overrides to a <see cref="ListView{T}" />.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="control">The list view instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static ListView<T> ApplyThemeAndDashboardOverrides<T>(this ListView<T> control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.ApplyTheme(bundle.Theme);
        return control.ApplyDashboardOverrides(bundle);
    }

    /// <summary>
    /// Applies dashboard overrides to a <see cref="Table" />.
    /// </summary>
    /// <param name="control">The table instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Table ApplyDashboardOverrides(this Table control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.FocusMarker = bundle.FocusMarker;
        control.TitleStyle = bundle.TitleStyle;
        control.FocusedTitleStyle = bundle.FocusedTitleStyle;
        control.BorderStyleText = bundle.BorderStyleText;
        control.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        control.HeaderStyle = bundle.HeaderStyle;
        control.RowStyle = bundle.DefaultItemStyle;
        control.HoveredRowStyle = bundle.HoveredItemStyle;
        control.SelectedRowStyle = bundle.SelectedItemStyle;
        return control;
    }

    /// <summary>
    /// Applies the bundle theme, then dashboard overrides to a <see cref="Table" />.
    /// </summary>
    /// <param name="control">The table instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Table ApplyThemeAndDashboardOverrides(this Table control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.ApplyTheme(bundle.Theme);
        return control.ApplyDashboardOverrides(bundle);
    }

    /// <summary>
    /// Applies dashboard overrides to a <see cref="Notifications" /> control.
    /// </summary>
    /// <param name="control">The notifications instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Notifications ApplyDashboardOverrides(this Notifications control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.FocusMarker = bundle.FocusMarker;
        control.BorderStyleText = bundle.BorderStyleText;
        control.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        control.SelectedItemStyle = bundle.SelectedItemStyle;
        control.HoveredItemStyle = bundle.HoveredItemStyle;
        control.UnreadItemStyle = bundle.UnreadItemStyle;
        return control;
    }

    /// <summary>
    /// Applies the bundle theme, then dashboard overrides to a <see cref="Notifications" /> control.
    /// </summary>
    /// <param name="control">The notifications instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Notifications ApplyThemeAndDashboardOverrides(this Notifications control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.ApplyTheme(bundle.Theme);
        return control.ApplyDashboardOverrides(bundle);
    }

    /// <summary>
    /// Applies dashboard overrides to a <see cref="LogView" />.
    /// </summary>
    /// <param name="control">The log view instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static LogView ApplyDashboardOverrides(this LogView control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.FocusMarker = bundle.FocusMarker;
        control.BorderStyleText = bundle.BorderStyleText;
        control.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        control.EntryStyle = bundle.EntryTextStyle;
        return control;
    }

    /// <summary>
    /// Applies the bundle theme, then dashboard overrides to a <see cref="LogView" />.
    /// </summary>
    /// <param name="control">The log view instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static LogView ApplyThemeAndDashboardOverrides(this LogView control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.ApplyTheme(bundle.Theme);
        return control.ApplyDashboardOverrides(bundle);
    }

    /// <summary>
    /// Applies dashboard overrides to a <see cref="Button" />.
    /// </summary>
    /// <param name="control">The button instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Button ApplyDashboardOverrides(this Button control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.BorderStyleText = bundle.BorderStyleText;
        control.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        control.LabelStyle = bundle.ActionLabelStyle;
        control.FocusedLabelStyle = bundle.FocusedActionLabelStyle;
        control.PressedLabelStyle = bundle.PressedActionLabelStyle;
        return control;
    }

    /// <summary>
    /// Applies the bundle theme, then dashboard overrides to a <see cref="Button" />.
    /// </summary>
    /// <param name="control">The button instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Button ApplyThemeAndDashboardOverrides(this Button control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.ApplyTheme(bundle.Theme);
        return control.ApplyDashboardOverrides(bundle);
    }

    /// <summary>
    /// Applies dashboard overrides to a <see cref="Dialog" />.
    /// </summary>
    /// <param name="control">The dialog instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Dialog ApplyDashboardOverrides(this Dialog control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.FocusMarker = bundle.FocusMarker;
        control.BorderStyleText = bundle.BorderStyleText;
        control.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        control.BodyTextStyle = bundle.BodyTextStyle;
        return control;
    }

    /// <summary>
    /// Applies the bundle theme, then dashboard overrides to a <see cref="Dialog" />.
    /// </summary>
    /// <param name="control">The dialog instance.</param>
    /// <param name="bundle">The override bundle to apply.</param>
    /// <returns>The same <paramref name="control" /> instance.</returns>
    public static Dialog ApplyThemeAndDashboardOverrides(this Dialog control, TeaThemeOverrideBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(bundle);

        control.ApplyTheme(bundle.Theme);
        return control.ApplyDashboardOverrides(bundle);
    }
}
