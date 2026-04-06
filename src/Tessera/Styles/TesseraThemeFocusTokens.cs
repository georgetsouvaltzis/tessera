namespace Tessera.Styles;

/// <summary>
/// Defines semantic focus styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeFocusTokens
{
    /// <summary>
    /// Gets focus ring style for focused elements.
    /// </summary>
    public TesseraStyle Ring { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets focus style for focused titles.
    /// </summary>
    public TesseraStyle Title { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets focus style for focused borders.
    /// </summary>
    public TesseraStyle Border { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets marker text appended by focus-capable overlays.
    /// </summary>
    /// <remarks>
    /// Empty means "unspecified" and allows control defaults to remain in effect.
    /// </remarks>
    public string Marker
    {
        get;
        init => field = value ?? string.Empty;
    } = string.Empty;
}
