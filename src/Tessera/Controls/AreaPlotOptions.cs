using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
///     Defines advanced rendering options for an <see cref="AreaPlot" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct AreaPlotOptions(
    char FillGlyph = '█',
    char LineGlyph = '▀',
    bool ShowBaseline = true,
    char BaselineGlyph = '─',
    bool ShowStats = false,
    string? Legend = null);
