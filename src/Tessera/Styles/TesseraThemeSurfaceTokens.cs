namespace Tessera.Styles;

/// <summary>
/// Defines semantic surface styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeSurfaceTokens
{
    /// <summary>
    /// Gets or sets the base.
    /// </summary>
    public TesseraStyle Base { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the panel.
    /// </summary>
    public TesseraStyle Panel { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the overlay.
    /// </summary>
    public TesseraStyle Overlay { get; init; } = TesseraStyle.Empty;
}
