using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Configures the built-in ANSI renderer for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class AnsiRendererOptions
{
    /// <summary>
    /// Gets or sets the flush timeout.
    /// </summary>
    public TimeSpan FlushTimeout { get; init; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets or sets the query mode reports.
    /// </summary>
    public bool QueryModeReports { get; init; } = true;

    /// <summary>
    /// Gets or sets the query mode reports once per mode.
    /// </summary>
    public bool QueryModeReportsOncePerMode { get; init; } = true;

    /// <summary>
    /// Gets or sets the include kitty keyboard base flag.
    /// </summary>
    public bool IncludeKittyKeyboardBaseFlag { get; init; } = true;
}
