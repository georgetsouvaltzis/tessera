namespace Tessera.Core.Abstractions;

public readonly record struct ScreenOutput(ScreenFrame Frame)
{
    public TerminalOutput Terminal { get; init; }
    public InputHooks Input { get; init; }

    public static ScreenOutput From(string content) => new(ScreenFrame.From(content));

    public ScreenOutput WithContent(string content) => this with
    {
        Frame = Frame with { Content = content },
    };
}
