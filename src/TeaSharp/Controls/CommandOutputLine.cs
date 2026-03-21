namespace TeaSharp.Controls;

/// <summary>
/// Identifies the stream channel for a <see cref="CommandOutputLine" />.
/// </summary>
public enum CommandOutputChannel
{
    /// <summary>
    /// Standard output stream.
    /// </summary>
    StdOut = 0,

    /// <summary>
    /// Standard error stream.
    /// </summary>
    StdErr = 1,

    /// <summary>
    /// System/meta message stream.
    /// </summary>
    System = 2,
}

/// <summary>
/// Represents one line in <see cref="CommandOutput" />.
/// </summary>
public sealed class CommandOutputLine
{
    /// <summary>
    /// Initializes a command output line.
    /// </summary>
    /// <param name="text">Line text.</param>
    /// <param name="channel">Stream channel.</param>
    /// <param name="timestamp">Timestamp attached to the line.</param>
    public CommandOutputLine(string text, CommandOutputChannel channel, DateTimeOffset timestamp)
    {
        Text = text ?? string.Empty;
        Channel = channel;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Gets or sets line text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets line channel.
    /// </summary>
    public CommandOutputChannel Channel { get; set; }

    /// <summary>
    /// Gets or sets line timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
