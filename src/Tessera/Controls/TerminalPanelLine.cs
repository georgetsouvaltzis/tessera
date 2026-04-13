namespace Tessera.Controls;

/// <summary>
///     Identifies terminal output channel for a <see cref="TerminalPanelLine" />.
/// </summary>
public enum TerminalPanelChannel
{
    /// <summary>
    ///     Standard output channel.
    /// </summary>
    StandardOutput = 0,

    /// <summary>
    ///     Standard error channel.
    /// </summary>
    StandardError = 1,

    /// <summary>
    ///     Command/input channel.
    /// </summary>
    Command = 2,

    /// <summary>
    ///     System/meta channel.
    /// </summary>
    System = 3
}

/// <summary>
///     Represents one terminal output row rendered by <see cref="TerminalPanel" />.
/// </summary>
public sealed class TerminalPanelLine
{
    /// <summary>
    ///     Initializes a terminal panel row.
    /// </summary>
    /// <param name="text">Row text payload.</param>
    /// <param name="channel">Logical output channel.</param>
    /// <param name="marker">Optional explicit channel marker override.</param>
    public TerminalPanelLine(
        string text,
        TerminalPanelChannel channel = TerminalPanelChannel.StandardOutput,
        string? marker = null)
    {
        Text = text;
        Channel = channel;
        Marker = marker;
    }

    /// <summary>
    ///     Gets row text payload.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Gets output channel for this row.
    /// </summary>
    public TerminalPanelChannel Channel { get; }

    /// <summary>
    ///     Gets optional channel marker override.
    /// </summary>
    public string? Marker { get; }
}
