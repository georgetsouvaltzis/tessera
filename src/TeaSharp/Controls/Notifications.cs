using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a stacked notification feed.
/// </summary>
public sealed class Notifications : Control
{
    private readonly List<NotificationItem> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Notifications";

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker { get; set; } = true;

    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle HoveredItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle UnreadItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle MutedItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle InfoItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle SuccessItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle WarningItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle ErrorItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle DisabledItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public int MaxItems
    {
        get;
        set;
    } = 128;

    public bool ShowTimestamp
    {
        get;
        set;
    } = true;

    public int Count => _items.Count;

    public override bool IsFocused
    {
        get;
        set;
    }

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    public void Push(string message, NotificationLevel level = NotificationLevel.Info, string? id = null)
    {
        _items.Add(new NotificationItem(
            id ?? Guid.NewGuid().ToString("n"),
            message ?? string.Empty,
            level,
            DateTimeOffset.UtcNow));

        if (_items.Count > MaxItems)
        {
            _items.RemoveAt(0);
        }

        _selectedIndex = Math.Max(0, _items.Count - 1);
    }

    public void Clear()
    {
        _items.Clear();
        _selectedIndex = 0;
        _hoveredIndex = -1;
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.IsCharacter('c'))
        {
            if (_items.Count == 0)
            {
                return false;
            }

            Clear();
            return true;
        }

        if (_items.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return MoveSelection(+1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return MoveSelection(-1);
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            var item = _items[_selectedIndex];
            if (item.IsRead)
            {
                return false;
            }

            _items[_selectedIndex] = item with { IsRead = true };
            return true;
        }

        if (key.IsCharacter('d'))
        {
            _items.RemoveAt(_selectedIndex);
            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, Math.Max(-1, _items.Count - 1));
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            if (pointer.Kind is not PointerEventKind.Wheel)
            {
                return changed || Handle(message);
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel && _items.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveSelection(+1) || changed;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1) || changed;
            }
        }

        if (!inside || _items.Count == 0)
        {
            return changed || Handle(message);
        }

        var hovered = ComputeWindowStart(content.Height) + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _items.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            changed |= SetHoveredIndex(hovered);
            if (_selectedIndex != hovered)
            {
                _selectedIndex = hovered;
                changed = true;
            }

            return changed;
        }

        return changed || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(empty)", MutedItemStyle), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_items.Count, start + content.Height);
        for (var row = 0; row < end - start; row++)
        {
            var index = start + row;
            var item = _items[index];
            var line = FormatLine(item, index == _selectedIndex);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveLineStyle(item, index == _selectedIndex, index == _hoveredIndex)), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, Title.Length + 4);
        width = Math.Max(width, _items.Count == 0 ? 7 : _items.Max(static item => item.Message.Length + 14));
        var height = Math.Clamp(Math.Min(MaxItems, Math.Max(1, _items.Count)) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2), 0, availableBounds.Height);
        return new LayoutMeasurement(
            Math.Clamp(width + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2), 0, availableBounds.Width),
            height);
    }

    private string FormatLine(NotificationItem item, bool selected)
    {
        var cursor = selected ? ">" : " ";
        var readMark = item.IsRead ? " " : "•";
        var timestamp = ShowTimestamp ? $"{item.CreatedAt:HH:mm:ss} " : string.Empty;
        return $"{cursor}{readMark} {timestamp}{item.Message}";
    }

    private TeaStyle ResolveLineStyle(NotificationItem item, bool selected, bool hovered)
    {
        var style = ItemStyle;
        if (selected)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (!item.IsRead)
        {
            style = style.Merge(UnreadItemStyle);
        }
        else
        {
            style = style.Merge(MutedItemStyle);
        }

        style = style.Merge(item.Level switch
        {
            NotificationLevel.Success => SuccessItemStyle,
            NotificationLevel.Warning => WarningItemStyle,
            NotificationLevel.Error => ErrorItemStyle,
            _ => InfoItemStyle,
        });

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle).Merge(MutedItemStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled || IsReadOnly)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private int ComputeWindowStart(int contentHeight)
    {
        return Math.Clamp(_selectedIndex - (contentHeight / 2), 0, Math.Max(0, _items.Count - contentHeight));
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return FrameLayout.ResolveContentRect(bounds, Border, Padding);
    }

    private bool MoveSelection(int delta)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var next = delta > 0
            ? Math.Min(_items.Count - 1, _selectedIndex + delta)
            : Math.Max(0, _selectedIndex + delta);
        if (next == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = next;
        return true;
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && FocusMarker.Length > 0
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private sealed record NotificationItem(
        string Id,
        string Message,
        NotificationLevel Level,
        DateTimeOffset CreatedAt,
        bool IsRead = false);
}
