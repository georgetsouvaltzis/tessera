namespace Tessera.Styles;

/// <summary>
/// Precomputed control-level style overrides derived from a semantic <see cref="TesseraTheme" />.
/// </summary>
/// <remarks>
/// Use this bundle to apply consistent instance overrides across multiple controls without repeating
/// per-control style composition logic.
/// </remarks>
public sealed class TesseraThemeOverrideBundle
{
    private TesseraThemeOverrideBundle(TesseraTheme theme, string focusMarker)
    {
        Theme = theme;
        FocusMarker = string.IsNullOrWhiteSpace(focusMarker) ? "*" : focusMarker;

        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background);

        BorderStyleText = theme.Border.Strong;
        FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        TitleStyle = theme.Accent.Primary.WithBold();
        FocusedTitleStyle = theme.Focus.Title.WithBold();
        DefaultItemStyle = theme.Text.Secondary;
        HeaderStyle = theme.Text.Primary.WithBold();
        SelectedItemStyle = selected.WithBold();
        HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        UnreadItemStyle = theme.Text.Primary.WithBold();
        ActionLabelStyle = theme.Text.Primary.WithBold();
        FocusedActionLabelStyle = theme.Text.Primary.WithBold();
        PressedActionLabelStyle = theme.Selection.Foreground.WithBold();
        ActionSurfaceStyle = theme.Surface.Overlay.IsEmpty ? theme.Surface.Panel : theme.Surface.Overlay;
        FocusedActionSurfaceStyle = ActionSurfaceStyle;
        PressedActionSurfaceStyle = theme.Selection.Background.IsEmpty ? ActionSurfaceStyle : theme.Selection.Background;
        BodyTextStyle = theme.Text.Primary;
        EntryTextStyle = theme.Text.Secondary;
    }

    /// <summary>
    /// Gets the theme used to derive all bundle styles.
    /// </summary>
    public TesseraTheme Theme { get; }

    /// <summary>
    /// Gets the focus marker text applied to controls that expose focus-marker APIs.
    /// </summary>
    public string FocusMarker { get; }

    /// <summary>
    /// Gets the unfocused border text style override.
    /// </summary>
    public TesseraStyle BorderStyleText { get; }

    /// <summary>
    /// Gets the focused border text style override.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; }

    /// <summary>
    /// Gets the unfocused title style override.
    /// </summary>
    public TesseraStyle TitleStyle { get; }

    /// <summary>
    /// Gets the focused title style override.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; }

    /// <summary>
    /// Gets the default list-like item style override.
    /// </summary>
    public TesseraStyle DefaultItemStyle { get; }

    /// <summary>
    /// Gets the list/table header style override.
    /// </summary>
    public TesseraStyle HeaderStyle { get; }

    /// <summary>
    /// Gets the selected item/row style override.
    /// </summary>
    public TesseraStyle SelectedItemStyle { get; }

    /// <summary>
    /// Gets the hovered item/row style override.
    /// </summary>
    public TesseraStyle HoveredItemStyle { get; }

    /// <summary>
    /// Gets the unread item style override used by notification-style controls.
    /// </summary>
    public TesseraStyle UnreadItemStyle { get; }

    /// <summary>
    /// Gets the default action label style override for action controls.
    /// </summary>
    public TesseraStyle ActionLabelStyle { get; }

    /// <summary>
    /// Gets the focused action label style override for action controls.
    /// </summary>
    public TesseraStyle FocusedActionLabelStyle { get; }

    /// <summary>
    /// Gets the pressed action label style override for action controls.
    /// </summary>
    public TesseraStyle PressedActionLabelStyle { get; }

    /// <summary>
    /// Gets the default action body surface override for action controls.
    /// </summary>
    public TesseraStyle ActionSurfaceStyle { get; }

    /// <summary>
    /// Gets the focused action body surface override for action controls.
    /// </summary>
    public TesseraStyle FocusedActionSurfaceStyle { get; }

    /// <summary>
    /// Gets the pressed action body surface override for action controls.
    /// </summary>
    public TesseraStyle PressedActionSurfaceStyle { get; }

    /// <summary>
    /// Gets the default body text style override.
    /// </summary>
    public TesseraStyle BodyTextStyle { get; }

    /// <summary>
    /// Gets the default log/entry text style override.
    /// </summary>
    public TesseraStyle EntryTextStyle { get; }

    /// <summary>
    /// Creates a dashboard-oriented override bundle from a semantic theme.
    /// </summary>
    /// <param name="theme">The semantic theme source.</param>
    /// <param name="focusMarker">The focus marker to apply to controls that expose marker hooks.</param>
    /// <returns>A reusable bundle with precomputed style overrides.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="theme" /> is <see langword="null" />.</exception>
    public static TesseraThemeOverrideBundle CreateDashboardBundle(TesseraTheme theme, string focusMarker = "*")
    {
        ArgumentNullException.ThrowIfNull(theme);
        return new TesseraThemeOverrideBundle(theme, focusMarker);
    }
}
