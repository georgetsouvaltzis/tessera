using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Defines advanced rendering options for a <see cref="Sparkline" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct SparklineOptions(
    string Steps = "▁▂▃▄▅▆▇█",
    bool ShowStats = false,
    string? Legend = null);
