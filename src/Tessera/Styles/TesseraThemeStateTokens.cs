namespace Tessera.Styles;

/// <summary>
/// Defines semantic state styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeStateTokens
{
    /// <summary>
    /// Gets or sets the success.
    /// </summary>
    public TesseraStyle Success { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the warning.
    /// </summary>
    public TesseraStyle Warning { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    public TesseraStyle Error { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the info.
    /// </summary>
    public TesseraStyle Info { get; init; } = TesseraStyle.Empty;
}
