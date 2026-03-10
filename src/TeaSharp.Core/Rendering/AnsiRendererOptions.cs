using System.ComponentModel;

namespace TeaSharp.Core.Rendering;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class AnsiRendererOptions
{
    public TimeSpan FlushTimeout { get; init; } = TimeSpan.FromSeconds(2);

    public bool QueryModeReports { get; init; } = true;

    public bool QueryModeReportsOncePerMode { get; init; } = true;

    public bool IncludeKittyKeyboardBaseFlag { get; init; } = true;
}
