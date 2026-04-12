namespace Tessera.Styles;

/// <summary>
/// Defines semantic border styles for a <see cref="TesseraTheme"/>.
/// </summary>
public sealed class TesseraThemeBorderTokens
{
    /// <summary>
    /// Gets or sets the default.
    /// </summary>
    public TesseraStyle Default { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the strong.
    /// </summary>
    public TesseraStyle Strong { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the focused.
    /// </summary>
    public TesseraStyle Focused { get; init; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    public TesseraStyle Error { get; init; } = TesseraStyle.Empty;
}
