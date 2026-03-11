namespace TeaSharp.Core.Abstractions;

public readonly record struct ScreenFrame(string Content)
{
    public int? CursorX { get; init; }
    public int? CursorY { get; init; }
    public CursorStyle? CursorStyle { get; init; }

    public static ScreenFrame From(string content) => new(content);
}
