namespace Tessera.Styles;

/// <summary>
/// Defines semantic text styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeTextTokens
{
    public TesseraStyle Primary { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Secondary { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Muted { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Inverse { get; init; } = TesseraStyle.Empty;
}
