namespace Tessera.Controls;

/// <summary>
///     Represents service health severity for <see cref="HealthService" /> rows.
/// </summary>
public enum HealthServiceSeverity
{
    /// <summary>
    ///     Service is operating normally.
    /// </summary>
    Healthy = 0,

    /// <summary>
    ///     Service is operating with reduced reliability.
    /// </summary>
    Degraded = 1,

    /// <summary>
    ///     Service is unavailable or in outage state.
    /// </summary>
    Outage = 2
}

/// <summary>
///     Represents one service row rendered by <see cref="HealthBoard" />.
/// </summary>
public sealed class HealthService
{
    /// <summary>
    ///     Initializes a service health row.
    /// </summary>
    /// <param name="id">Stable service identifier.</param>
    /// <param name="name">Display name.</param>
    /// <param name="severity">Current health severity.</param>
    /// <param name="summary">Optional summary text.</param>
    /// <param name="observedAt">Optional observed timestamp. Defaults to <see cref="DateTimeOffset.UtcNow" />.</param>
    public HealthService(
        string id,
        string name,
        HealthServiceSeverity severity = HealthServiceSeverity.Healthy,
        string? summary = null,
        DateTimeOffset? observedAt = null)
    {
        Id = id;
        Name = name;
        Severity = severity;
        Summary = summary ?? string.Empty;
        ObservedAt = observedAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Gets or sets service identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets service display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets health severity.
    /// </summary>
    public HealthServiceSeverity Severity { get; set; }

    /// <summary>
    ///     Gets or sets optional status summary text.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    ///     Gets or sets observed timestamp.
    /// </summary>
    public DateTimeOffset ObservedAt { get; set; }

    /// <summary>
    ///     Gets or sets whether the service alert has been acknowledged.
    /// </summary>
    public bool IsAcknowledged { get; set; }

    /// <summary>
    ///     Gets or sets whether the row should render muted.
    /// </summary>
    public bool IsMuted { get; set; }
}

/// <summary>
///     Defines glyphs used by <see cref="HealthBoard" />.
/// </summary>
public readonly record struct HealthBoardGlyphSet
{
    /// <summary>
    ///     Initializes a glyph set with built-in defaults.
    /// </summary>
    public HealthBoardGlyphSet()
    {
        NormalRowMarker = " ";
        SelectedRowMarker = ">";
        HoveredRowMarker = "+";
        HealthyGlyph = "OK";
        DegradedGlyph = "~";
        OutageGlyph = "!";
        AcknowledgedGlyph = "ACK";
        MarkerSeparator = " ";
    }

    /// <summary>
    ///     Initializes a glyph set.
    /// </summary>
    /// <param name="normalRowMarker">Marker for non-selected and non-hovered rows.</param>
    /// <param name="selectedRowMarker">Marker for selected rows.</param>
    /// <param name="hoveredRowMarker">Marker for hovered rows.</param>
    /// <param name="healthyGlyph">Glyph for healthy services.</param>
    /// <param name="degradedGlyph">Glyph for degraded services.</param>
    /// <param name="outageGlyph">Glyph for outage services.</param>
    /// <param name="acknowledgedGlyph">Glyph rendered when a row is acknowledged.</param>
    /// <param name="markerSeparator">Separator between marker, status, and text segments.</param>
    public HealthBoardGlyphSet(
        string normalRowMarker,
        string selectedRowMarker,
        string hoveredRowMarker,
        string healthyGlyph,
        string degradedGlyph,
        string outageGlyph,
        string acknowledgedGlyph,
        string markerSeparator)
    {
        NormalRowMarker = normalRowMarker;
        SelectedRowMarker = selectedRowMarker;
        HoveredRowMarker = hoveredRowMarker;
        HealthyGlyph = healthyGlyph;
        DegradedGlyph = degradedGlyph;
        OutageGlyph = outageGlyph;
        AcknowledgedGlyph = acknowledgedGlyph;
        MarkerSeparator = markerSeparator;
    }

    /// <summary>
    ///     Gets the built-in glyph set.
    /// </summary>
    public static HealthBoardGlyphSet Default => new();

    /// <summary>
    ///     Gets the marker for non-selected and non-hovered rows.
    /// </summary>
    public string NormalRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker for selected rows.
    /// </summary>
    public string SelectedRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker for hovered rows.
    /// </summary>
    public string HoveredRowMarker { get; init; }

    /// <summary>
    ///     Gets the glyph for healthy services.
    /// </summary>
    public string HealthyGlyph { get; init; }

    /// <summary>
    ///     Gets the glyph for degraded services.
    /// </summary>
    public string DegradedGlyph { get; init; }

    /// <summary>
    ///     Gets the glyph for outage services.
    /// </summary>
    public string OutageGlyph { get; init; }

    /// <summary>
    ///     Gets the glyph for acknowledged rows.
    /// </summary>
    public string AcknowledgedGlyph { get; init; }

    /// <summary>
    ///     Gets the separator between marker, status, and text segments.
    /// </summary>
    public string MarkerSeparator { get; init; }
}
