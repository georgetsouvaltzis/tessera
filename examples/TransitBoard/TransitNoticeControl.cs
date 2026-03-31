using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.TransitBoard;

internal sealed class TransitNoticeControl : Control
{
    private readonly List<TransitNotice> _items = [];

    public string Title { get; set; } = "Service Notices";
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DividerStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PrimaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SecondaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle WarningStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DelayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SuccessStyle { get; set; } = TeaStyle.Empty;

    public void SetItems(IEnumerable<TransitNotice> notices)
    {
        ArgumentNullException.ThrowIfNull(notices);
        _items.Clear();
        _items.AddRange(notices);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.WriteText(clipped.X, clipped.Y, Render(TitleStyle, Title), clipped.Width);
        if (clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, Render(DividerStyle, new string('─', clipped.Width)), clipped.Width);
        }

        for (var index = 0; index < Math.Min(_items.Count, Math.Max(0, clipped.Height - 2)); index++)
        {
            var notice = _items[index];
            var style = notice.Tone switch
            {
                "amber" => WarningStyle,
                "rose" => DelayStyle,
                "mint" => SuccessStyle,
                _ => SecondaryStyle,
            };
            var text = $"{Render(style, $"[{notice.Label}]")} {Render(PrimaryStyle, notice.Message)}";
            canvas.WriteText(clipped.X, clipped.Y + 2 + index, text, clipped.Width);
        }
    }

    private static string Render(TeaStyle style, string text) => style.IsEmpty ? text : style.Render(text);
}
