using System.ComponentModel;
namespace TeaSharp.Controls;

/// <summary>
/// Defines advanced rendering options for a <see cref="BarChart"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct BarChartOptions(bool ShowScale = false, string? Legend = null);
