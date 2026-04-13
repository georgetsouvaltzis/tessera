using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.TransitBoard;

internal sealed class TransitChipStripControl : Control
{
    private readonly List<(int Start, int End)> _hitZones = [];
    private readonly List<TransitChipItem> _items = [];
    private int _selectedIndex;

    public string Title { get; set; } = string.Empty;
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle DividerStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;
    public string FocusMarker { get; set; } = "◆";
    public bool SelectedSubtitleOnly { get; set; } = true;

    public TransitChipItem? SelectedItem =>
        _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    public event EventHandler<TransitChipChangedEventArgs>? SelectionChanged;

    public void SetItems(IEnumerable<TransitChipItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previous = SelectedItem;
        _items.Clear();
        _items.AddRange(items);
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));

        if (previous?.Id != SelectedItem?.Id)
        {
            SelectionChanged?.Invoke(this, new TransitChipChangedEventArgs(previous, SelectedItem));
        }
    }

    public bool SelectById(string id)
    {
        var index = _items.FindIndex(item => string.Equals(item.Id, id, StringComparison.Ordinal));
        return SetSelectedIndex(index);
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_items.Count - 1);
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left &&
            bounds.Contains(pointer.X, pointer.Y))
        {
            RequestFocus();
            var lineY = bounds.Y + (string.IsNullOrEmpty(Title) ? 0 : 1);
            if (pointer.Y != lineY)
            {
                return false;
            }

            var relativeX = pointer.X - bounds.X;
            for (var index = 0; index < _hitZones.Count; index++)
            {
                var zone = _hitZones[index];
                if (relativeX >= zone.Start && relativeX <= zone.End)
                {
                    return SetSelectedIndex(index);
                }
            }
        }

        return Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        _hitZones.Clear();

        var titleStyle = IsFocused && !FocusedTitleStyle.IsEmpty ? FocusedTitleStyle : TitleStyle;
        string titleText;
        if (string.IsNullOrEmpty(Title))
        {
            titleText = string.Empty;
        }
        else
        {
            var label = IsFocused ? $"{Title} {FocusMarker}" : Title;
            titleText = Render(titleStyle, label);
        }

        var chipY = clipped.Y;
        if (!string.IsNullOrEmpty(titleText))
        {
            canvas.WriteText(clipped.X, clipped.Y, titleText, clipped.Width);
            chipY++;
        }

        if (chipY >= clipped.Bottom)
        {
            return;
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(clipped.X, chipY, Render(EmptyStyle, "(no routes)"), clipped.Width);
            return;
        }

        var x = clipped.X;
        var limit = clipped.Right;
        for (var index = 0; index < _items.Count; index++)
        {
            var item = _items[index];
            var separator = index == 0 ? string.Empty : "  ";
            var showSubtitle = !SelectedSubtitleOnly || index == _selectedIndex;
            var subtitleText = showSubtitle && !string.IsNullOrWhiteSpace(item.Subtitle)
                ? $" {item.Subtitle}"
                : string.Empty;
            var rawLabel = $"{separator}[{item.Label}]{subtitleText}";
            var renderedLabel =
                $"{separator}{Render(index == _selectedIndex ? item.PrimaryStyle : item.SecondaryStyle, $"[{item.Label}]")}{(subtitleText.Length == 0 ? string.Empty : Render(item.SecondaryStyle, subtitleText))}";
            var width = Math.Min(rawLabel.Length, Math.Max(0, limit - x));
            if (width <= 0)
            {
                break;
            }

            canvas.WriteText(x, chipY, renderedLabel, limit - x);
            _hitZones.Add((x - clipped.X, x - clipped.X + width - 1));
            x += width;
        }

        if (chipY + 1 < clipped.Bottom)
        {
            canvas.WriteText(clipped.X, chipY + 1, Render(DividerStyle, new string('─', clipped.Width)), clipped.Width);
        }
    }

    private bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0 || index < 0 || index >= _items.Count || index == _selectedIndex)
        {
            return false;
        }

        var previous = SelectedItem;
        _selectedIndex = index;
        SelectionChanged?.Invoke(this, new TransitChipChangedEventArgs(previous, SelectedItem));
        return true;
    }

    private static string Render(TesseraStyle style, string text)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}

internal sealed record TransitChipChangedEventArgs(TransitChipItem? PreviousItem, TransitChipItem? SelectedItem);
