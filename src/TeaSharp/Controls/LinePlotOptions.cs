using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Defines advanced rendering options for a <see cref="LinePlot"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct LinePlotOptions(
    bool ShowAxes = false,
    bool ShowGrid = false,
    bool ShowLegend = true,
    bool ShowStats = true,
    string? XLabel = null,
    string? YLabel = null,
    string? SharedAxisLabel = null,
    string? NormalizedAxisLabel = null,
    double Zoom = 1.0,
    int Offset = 0);
