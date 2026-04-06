namespace Tessera.Styles;

/// <summary>
/// Defines semantic state styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeStateTokens
{
    public TesseraStyle Success { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Warning { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Error { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Info { get; init; } = TesseraStyle.Empty;
}
