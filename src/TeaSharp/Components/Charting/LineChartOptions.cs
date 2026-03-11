using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
namespace TeaSharp.Components.Charting;

public readonly record struct LineChartOptions(
    bool ShowAxes = false,
    string? Legend = null,
    string? XLabel = null,
    string? YLabel = null,
    double Zoom = 1.0,
    int Offset = 0);
