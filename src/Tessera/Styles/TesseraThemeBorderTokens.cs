namespace Tessera.Styles;

/// <summary>
/// Defines semantic border styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeBorderTokens
{
    public TesseraStyle Default { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Strong { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Focused { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Error { get; init; } = TesseraStyle.Empty;
}
