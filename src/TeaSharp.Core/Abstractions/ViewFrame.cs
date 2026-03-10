namespace TeaSharp.Core.Abstractions;

public readonly record struct ViewFrame(string Content)
{
    public int? CursorX { get; init; }
    public int? CursorY { get; init; }
    public CursorStyle? CursorStyle { get; init; }

    public static ViewFrame From(string content) => new(content);
}
