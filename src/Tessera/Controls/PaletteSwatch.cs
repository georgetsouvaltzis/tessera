using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents one named color swatch row in <see cref="PaletteEditor" />.
/// </summary>
public sealed class PaletteSwatch
{
    /// <summary>
    /// Initializes a new swatch.
    /// </summary>
    /// <param name="name">Display name shown in the palette grid.</param>
    /// <param name="hex">Optional hex text shown beside the name (for example <c>#89B4FA</c>).</param>
    /// <param name="description">Optional helper text.</param>
    public PaletteSwatch(string name, string? hex = null, string? description = null)
    {
        Name = name ?? string.Empty;
        Hex = hex ?? string.Empty;
        Description = description ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets display name shown in the grid.
    /// </summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets optional hex text shown with the swatch.
    /// </summary>
    public string Hex
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets optional description text.
    /// </summary>
    public string Description
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets style merged into row text for this swatch.
    /// </summary>
    public TesseraStyle Style { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into preview glyph text.
    /// </summary>
    public TesseraStyle PreviewStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets whether this swatch should render with muted emphasis.
    /// </summary>
    public bool IsMuted { get; set; }
}
