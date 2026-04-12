namespace Tessera.Styles;

/// <summary>
/// Represents a semantic theme for Tessera controls.
/// </summary>
public sealed class TesseraTheme
{
    /// <summary>
    /// Executes tessera theme.
    /// </summary>
    /// <returns>The result of tessera theme.</returns>
    public TesseraTheme()
    {
    }

    /// <summary>
    /// Executes tessera theme.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <param name="surface">The surface value.</param>
    /// <param name="border">The border value.</param>
    /// <param name="state">The state value.</param>
    /// <param name="accent">The accent value.</param>
    /// <param name="selection">The selection value.</param>
    /// <param name="focus">The focus value.</param>
    /// <returns>The result of tessera theme.</returns>
    public TesseraTheme(
        TesseraThemeTextTokens text,
        TesseraThemeSurfaceTokens surface,
        TesseraThemeBorderTokens border,
        TesseraThemeStateTokens state,
        TesseraThemeAccentTokens accent,
        TesseraThemeSelectionTokens selection,
        TesseraThemeFocusTokens focus)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Surface = surface ?? throw new ArgumentNullException(nameof(surface));
        Border = border ?? throw new ArgumentNullException(nameof(border));
        State = state ?? throw new ArgumentNullException(nameof(state));
        Accent = accent ?? throw new ArgumentNullException(nameof(accent));
        Selection = selection ?? throw new ArgumentNullException(nameof(selection));
        Focus = focus ?? throw new ArgumentNullException(nameof(focus));
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public TesseraThemeTextTokens Text { get; init; } = new();

    /// <summary>
    /// Gets or sets the surface.
    /// </summary>
    public TesseraThemeSurfaceTokens Surface { get; init; } = new();

    /// <summary>
    /// Gets or sets the border.
    /// </summary>
    public TesseraThemeBorderTokens Border { get; init; } = new();

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    public TesseraThemeStateTokens State { get; init; } = new();

    /// <summary>
    /// Gets or sets the accent.
    /// </summary>
    public TesseraThemeAccentTokens Accent { get; init; } = new();

    /// <summary>
    /// Gets or sets the selection.
    /// </summary>
    public TesseraThemeSelectionTokens Selection { get; init; } = new();

    /// <summary>
    /// Gets or sets the focus.
    /// </summary>
    public TesseraThemeFocusTokens Focus { get; init; } = new();
}
