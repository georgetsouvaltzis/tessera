namespace TeaSharp.Core.Abstractions;

public readonly record struct View(ViewFrame Frame)
{
    public ViewTerminal Terminal { get; init; }
    public ViewInput Input { get; init; }

    public static View From(string content) => new(ViewFrame.From(content));

    public View WithContent(string content) => this with
    {
        Frame = Frame with { Content = content },
    };
}
