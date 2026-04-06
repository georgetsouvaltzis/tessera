using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Configures the built-in ANSI renderer for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class AnsiRendererOptions
{
    public TimeSpan FlushTimeout { get; init; } = TimeSpan.FromSeconds(2);

    public bool QueryModeReports { get; init; } = true;

    public bool QueryModeReportsOncePerMode { get; init; } = true;

    public bool IncludeKittyKeyboardBaseFlag { get; init; } = true;
}
