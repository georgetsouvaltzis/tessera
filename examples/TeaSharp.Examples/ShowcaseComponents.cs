
internal sealed class UnicodeShowcaseComponent : ICanvasComponent
{
    public string Title { get; set; } = "Unicode + Runtime";

    public string CapabilitySource { get; set; } = "unknown";

    public bool Focus { get; set; } = true;

    public string LastPaste { get; set; } = "(none)";

    public string TypedPreview { get; set; } = string.Empty;

    public int Count { get; set; }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 8 || clipped.Height < 4)
        {
            return;
        }

        canvas.DrawBox(clipped, Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        string[] lines =
        [
            $"emoji: 🚀 ✅ 🎯 count={Count}",
            "grapheme: cafe\u0301 nai\u0308ve co\u0308de",
            $"focus: {(Focus ? "in" : "out")} source: {CapabilitySource}",
            $"paste: {LastPaste}",
            $"typed: {TypedPreview}",
        ];

        var rows = Math.Min(lines.Length, content.Height);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }
}
