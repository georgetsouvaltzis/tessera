using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.MusicDeck;

internal sealed class MusicDeckQueueControl : Control
{
    private IReadOnlyList<MusicTrack> _items = Array.Empty<MusicTrack>();

    public string Title { get; set; } = "Queue";
    public string FocusMarker { get; set; } = "✦";
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public int CurrentIndex { get; set; }
    public int SelectedIndex { get; set; }
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle CurrentItemStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle MetaStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    public void SetItems(IReadOnlyList<MusicTrack> items)
    {
        _items = items ?? Array.Empty<MusicTrack>();
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = IsFocused ? $"{Title} {FocusMarker}" : Title;
        var titleText = IsFocused ? Render(FocusedTitleStyle, title) : Render(TitleStyle, title);
        canvas.DrawBox(clipped, titleText, Border, ResolveBorderStyle());

        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        var start = 0;
        if (_items.Count > content.Height)
        {
            start = Math.Clamp(SelectedIndex - (content.Height / 2), 0, _items.Count - content.Height);
        }

        for (var row = 0; row < content.Height && start + row < _items.Count; row++)
        {
            var index = start + row;
            var item = _items[index];
            var prefix = index == CurrentIndex ? "●" : index == SelectedIndex ? "◆" : "·";
            var style = index == SelectedIndex ? SelectedItemStyle : index == CurrentIndex ? CurrentItemStyle : ItemStyle;
            var text = $"{prefix} {item.Title}";
            canvas.WriteText(content.X, content.Y + row, Render(style, text), Math.Max(0, content.Width - 7));
            var durationX = Math.Max(content.X, content.Right - item.DisplayDuration.Length);
            canvas.WriteText(durationX, content.Y + row, Render(MetaStyle, item.DisplayDuration), content.Right - durationX);
        }
    }

    private TeaStyle ResolveBorderStyle() => IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
    private static string Render(TeaStyle style, string text) => style.IsEmpty || string.IsNullOrEmpty(text) ? text : style.Render(text);
}
