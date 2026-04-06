namespace Tessera.Styles;

/// <summary>
/// Defines semantic surface styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeSurfaceTokens
{
    public TesseraStyle Base { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Panel { get; init; } = TesseraStyle.Empty;

    public TesseraStyle Overlay { get; init; } = TesseraStyle.Empty;
}
