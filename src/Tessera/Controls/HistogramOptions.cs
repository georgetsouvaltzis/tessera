using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
///     Defines advanced rendering options for a <see cref="Histogram" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct HistogramOptions(
    bool ShowAxes = true,
    bool ShowBucketLabels = true,
    bool ShowScale = false,
    string? Legend = null,
    string? XLabel = null,
    string? YLabel = null,
    char BarGlyph = '█');
