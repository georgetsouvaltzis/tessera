using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
///     Defines advanced rendering options for a <see cref="ScatterPlot" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct ScatterPlotOptions(
    bool ShowAxes = true,
    bool ShowLabels = false,
    string? Legend = null,
    string? XLabel = null,
    string? YLabel = null,
    char PointGlyph = '●');
