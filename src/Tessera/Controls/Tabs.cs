using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a tab-strip for switching between named views.
/// </summary>
public sealed class Tabs : Control
{
    private readonly List<string> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Executes tabs.
    /// </summary>
    /// <param name="items">The items value.</param>
    /// <returns>The result of tabs.</returns>
    public Tabs(IEnumerable<string> items)
    {
        SetItems(items ?? Array.Empty<string>());
    }

    /// <summary>
    /// Executes tabs.
    /// </summary>
    /// <param name="items">The items value.</param>
    /// <returns>The result of tabs.</returns>
    public Tabs(params string[] items)
        : this((IEnumerable<string>)items)
    {
    }

    /// <summary>
    /// Represents selection changed.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Represents items.
    /// </summary>
    public IReadOnlyList<string> Items => _items;

    /// <summary>
    /// Represents selected index.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets or sets the optional title shown before tab labels.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be rendered in the title when focused.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style applied when the control is not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the title style applied when the control is focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    /// Executes set items.
    /// </summary>
    /// <param name="items">The items value.</param>
    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        _items.AddRange(items.Where(static item => item is not null));
        if (_items.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _items.Count - 1);
    }

    /// <summary>
    /// Sets the selected tab index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        return TrySetSelectedIndex(Math.Clamp(index, 0, _items.Count - 1));
    }

    /// <summary>
    /// Selects a tab by index.
    /// </summary>
    /// <param name="index">Requested index.</param>
    public void Select(int index)
    {
        if (_items.Count == 0)
        {
            _selectedIndex = 0;
            return;
        }

        _ = SetSelectedIndex(index);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return TrySetSelectedIndex((_selectedIndex + 1) % _items.Count);
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return TrySetSelectedIndex((_selectedIndex + _items.Count - 1) % _items.Count);
        }

        if (key.Key == Key.Character
            && key.Modifiers == ModifierKeys.None
            && key.Text.Length == 1
            && char.IsDigit(key.Text[0]))
        {
            var requested = key.Text[0] == '0' ? 10 : key.Text[0] - '0';
            if (requested >= 1 && requested <= _items.Count)
            {
                return TrySetSelectedIndex(requested - 1);
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (_items.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var inRow = bounds.Contains(pointer.X, pointer.Y) && pointer.Y == bounds.Y;
        if (!inRow)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1) || Handle(message);
            }

            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return TrySetSelectedIndex((_selectedIndex + 1) % _items.Count);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return TrySetSelectedIndex((_selectedIndex + _items.Count - 1) % _items.Count);
            }
        }

        var hovered = HitTestTabIndex(pointer.X, bounds);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            return TrySetSelectedIndex(hovered);
        }

        return Handle(message);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1 || _items.Count == 0)
        {
            return;
        }

        var x = clipped.X;
        var plainTitle = FormatTitleText();
        if (!string.IsNullOrEmpty(plainTitle))
        {
            canvas.WriteText(x, clipped.Y, RenderTitle(plainTitle), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(plainTitle) + 1;
        }

        for (var index = 0; index < _items.Count && x < clipped.Right; index++)
        {
            var label = FormatLabel(index, hovered: index == _hoveredIndex);
            canvas.WriteText(x, clipped.Y, label, clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(label) + 1;
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        for (var index = 0; index < _items.Count; index++)
        {
            width += ControlTextLayout.MeasureDisplayWidth(FormatLabel(index, hovered: false));
            if (index > 0)
            {
                width++;
            }
        }

        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            width += ControlTextLayout.MeasureDisplayWidth(title) + (_items.Count > 0 ? 1 : 0);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(_items.Count == 0 ? 0 : 1, 0, availableBounds.Height));
    }

    private string FormatLabel(int index, bool hovered)
    {
        var label = index == _selectedIndex
            ? $"[{index + 1}:{_items[index]}]"
            : $" {index + 1}:{_items[index]} ";
        return hovered && index != _selectedIndex
            ? $">{label.Trim()}<"
            : label;
    }

    private int HitTestTabIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            cursor += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        for (var index = 0; index < _items.Count && cursor < bounds.Right; index++)
        {
            var label = FormatLabel(index, hovered: false);
            var width = ControlTextLayout.MeasureDisplayWidth(label);
            var end = cursor + width;
            if (x >= cursor && x < end)
            {
                return index;
            }

            cursor = end + 1;
        }

        return -1;
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

    private bool TrySetSelectedIndex(int index)
    {
        if (_items.Count == 0 || index == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousTab = _items[previousIndex];
        _selectedIndex = index;
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(previousIndex, _selectedIndex, previousTab, _items[_selectedIndex]));
        return true;
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle(string title)
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (style.IsEmpty || string.IsNullOrEmpty(title))
        {
            return title;
        }

        return style.Render(title);
    }
}
