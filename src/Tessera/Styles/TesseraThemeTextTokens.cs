namespace Tessera.Styles;

/// <summary>
///     Defines semantic text styles for a <see cref="TesseraTheme" />.
/// </summary>
public sealed class TesseraThemeTextTokens
{
    /// <summary>
    ///     Gets or sets the primary.
    /// </summary>
    public TesseraStyle Primary { get; init; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the secondary.
    /// </summary>
    public TesseraStyle Secondary { get; init; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the muted.
    /// </summary>
    public TesseraStyle Muted { get; init; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the inverse.
    /// </summary>
    public TesseraStyle Inverse { get; init; } = TesseraStyle.Empty;
}
