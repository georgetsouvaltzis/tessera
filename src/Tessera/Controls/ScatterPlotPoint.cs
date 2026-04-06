namespace Tessera.Controls;

/// <summary>
/// Represents one point in a <see cref="ScatterPlot"/>.
/// </summary>
public readonly record struct ScatterPlotPoint
{
    /// <summary>
    /// Initializes a new point.
    /// </summary>
    /// <param name="x">The X-axis value.</param>
    /// <param name="y">The Y-axis value.</param>
    /// <param name="label">Optional point label.</param>
    public ScatterPlotPoint(double x, double y, string? label = null)
    {
        X = x;
        Y = y;
        Label = label ?? string.Empty;
    }

    /// <summary>
    /// Gets the X-axis value.
    /// </summary>
    public double X { get; init; }

    /// <summary>
    /// Gets the Y-axis value.
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    /// Gets the optional label shown near the point when enabled.
    /// </summary>
    public string Label { get; init; }
}
