namespace Tessera.Styles;

/// <summary>
/// Defines semantic selection styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeSelectionTokens
{
    /// <summary>
    /// Gets or sets the foreground.
    /// </summary>
    public TesseraStyle Foreground { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    public TesseraStyle Background { get; init; } = TesseraStyle.Empty;
}
