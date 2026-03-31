using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.DataWorkbench;

internal sealed class DataWorkbenchHeaderControl : Control
{
    public string Title { get; set; } = "DataWorkbench // Investigation Console";
    public string ClockText { get; set; } = string.Empty;
    public string WorkspaceText { get; set; } = string.Empty;
    public string SourceText { get; set; } = string.Empty;
    public string ViewText { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public string PromptText { get; set; } = string.Empty;
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; }
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ClockStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BadgeStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SummaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PromptStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    public override bool IsFocused { get; set; }
    public override bool IsDisabled { get; set; }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, null, Border, ResolveBorderStyle());
        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y, ApplyStyle($"{Title}  {ClockText}", TitleStyle.Merge(ClockStyle)), content.Width);

        var badgeLine = string.Join(
            " ",
            ApplyStyle($"[{WorkspaceText}]", BadgeStyle),
            ApplyStyle($"[{SourceText}]", BadgeStyle),
            ApplyStyle($"[{ViewText}]", BadgeStyle));
        if (content.Height > 1)
        {
            canvas.WriteText(content.X, content.Y + 1, badgeLine, content.Width);
        }

        if (content.Height > 2)
        {
            canvas.WriteText(content.X, content.Y + 2, ApplyStyle(SummaryText, SummaryStyle), content.Width);
        }

        if (content.Height > 3)
        {
            canvas.WriteText(content.X, content.Y + 3, ApplyStyle(PromptText, PromptStyle), content.Width);
        }
    }

    private TeaStyle ResolveBorderStyle()
    {
        return IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
    }

    private static string ApplyStyle(string value, TeaStyle style)
    {
        return string.IsNullOrEmpty(value) || style.IsEmpty ? value : style.Render(value);
    }
}
