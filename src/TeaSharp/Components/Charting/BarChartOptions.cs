using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
namespace TeaSharp.Components.Charting;

public readonly record struct BarChartOptions(bool ShowScale = false, string? Legend = null);
