using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
///     Defines advanced rendering options for a <see cref="TelemetryChart" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct TelemetryChartOptions(
    bool ShowStats = false,
    string? Legend = null,
    TelemetryChartRenderMode RenderMode = TelemetryChartRenderMode.Auto);

/// <summary>
///     Selects how a <see cref="TelemetryChart" /> rasterizes a compact telemetry trend.
/// </summary>
public enum TelemetryChartRenderMode
{
    /// <summary>
    ///     Selects a braille-first compact renderer, with block fallback when the card is too short.
    /// </summary>
    Auto = 0,

    /// <summary>
    ///     Uses a filled block-area microchart optimized for tiny dashboard cards.
    /// </summary>
    Area = 1,

    /// <summary>
    ///     Uses a narrow filled ribbon microchart instead of a full area fill.
    /// </summary>
    Block = 2,

    /// <summary>
    ///     Uses braille/subcell area coverage for terminals that render braille crisply.
    /// </summary>
    Braille = 3
}
