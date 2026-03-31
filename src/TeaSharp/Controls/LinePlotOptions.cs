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
    int Offset = 0,
    LinePlotRenderMode RenderMode = LinePlotRenderMode.Coarse);

/// <summary>
/// Selects how a <see cref="LinePlot"/> rasterizes series inside the plot area.
/// </summary>
public enum LinePlotRenderMode
{
    /// <summary>
    /// Uses the existing cell-by-cell box-drawing renderer.
    /// </summary>
    Coarse = 0,

    /// <summary>
    /// Uses compact subcell plotting optimized for dense single-series telemetry cards.
    /// Falls back to a block micro-chart when the plot area is too small for braille rasterization.
    /// </summary>
    Compact = 1,
}
