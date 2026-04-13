using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
///     Defines advanced rendering options for a <see cref="LinePlot" />.
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
///     Selects how a <see cref="LinePlot" /> rasterizes series inside the plot area.
/// </summary>
public enum LinePlotRenderMode
{
    /// <summary>
    ///     Uses the existing cell-by-cell box-drawing renderer.
    /// </summary>
    Coarse = 0,

    /// <summary>
    ///     Uses compact terminal-native line rendering for coarse single-series plots in tight spaces.
    ///     Prefer <see cref="TelemetryChart" /> for tiny dashboard telemetry cards.
    /// </summary>
    Compact = 1,

    /// <summary>
    ///     Uses compact braille/subcell plotting for dense single-series plots where braille coverage is preferred.
    ///     Prefer <see cref="TelemetryChart" /> for tiny dashboard telemetry cards.
    /// </summary>
    CompactBraille = 2
}
