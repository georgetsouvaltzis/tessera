namespace Tessera.Styles;

/// <summary>
/// Defines semantic accent styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeAccentTokens
{
    /// <summary>
    /// Gets or sets the primary.
    /// </summary>
    public TesseraStyle Primary { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the secondary.
    /// </summary>
    public TesseraStyle Secondary { get; init; } = TesseraStyle.Empty;
}
