namespace Tessera.Core.Abstractions;

/// <summary>
///     Combines the rendered frame with terminal-side side effects and input hooks.
/// </summary>
/// <param name="Frame">The rendered frame to flush to the terminal.</param>
public readonly record struct ScreenOutput(ScreenFrame Frame)
{
    /// <summary>
    ///     Gets terminal commands that should accompany the frame.
    /// </summary>
    public TerminalOutput Terminal { get; init; }

    /// <summary>
    ///     Gets low-level input hooks that should be active for the frame.
    /// </summary>
    public InputHooks Input { get; init; }

    /// <summary>
    ///     Creates screen output from plain text content.
    /// </summary>
    /// <param name="content">The rendered terminal content.</param>
    /// <returns>A screen output object wrapping the supplied content.</returns>
    public static ScreenOutput From(string content)
    {
        return new ScreenOutput(ScreenFrame.From(content));
    }

    /// <summary>
    ///     Creates a copy with updated frame content.
    /// </summary>
    /// <param name="content">The replacement terminal content.</param>
    /// <returns>A copy of the output with updated content.</returns>
    public ScreenOutput WithContent(string content)
    {
        return this with { Frame = Frame with { Content = content } };
    }
}
