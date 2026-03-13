using System.ComponentModel;
namespace TeaSharp.Controls;

/// <summary>
/// Defines advanced rendering options for a <see cref="LineChart"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct LineChartOptions(
    bool ShowAxes = false,
    string? Legend = null,
    string? XLabel = null,
    string? YLabel = null,
    double Zoom = 1.0,
    int Offset = 0);
