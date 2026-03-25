using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a pointer-anchored menu of contextual actions.
/// </summary>
public sealed class ContextMenu : Control
{
    private readonly List<ContextMenuItem> _items = [];
    private readonly List<ContextMenuRenderCache> _itemRenderCache = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;
    private int _cachedItemWidth = 12;
    private long _executionVersion;
    private long _consumedExecutionVersion;
    private ContextMenuGlyphSet _glyphs = ContextMenuGlyphSet.Default;

    /// <summary>
    /// Occurs when a menu item is executed.
    /// </summary>
    public event EventHandler<ContextMenuItemExecutedEventArgs>? ItemExecuted;

    /// <summary>
    /// Gets or sets the menu title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Context";

    /// <summary>
    /// Gets or sets the marker appended to the title while focused when <see cref="ShowFocusMarker"/> is enabled.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be shown while focused.
    /// </summary>
    /// <remarks>
    /// Defaults to <see langword="false"/> to preserve previous context-menu title output.
    /// </remarks>
    public bool ShowFocusMarker { get; set; }

    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle HoveredItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle DisabledItemStyle { get; set; } = TeaStyle.Empty;

    public TeaStyle MutedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets row marker glyphs used for rendering.
    /// </summary>
    public ContextMenuGlyphSet Glyphs
    {
        get => _glyphs;
        set
        {
            if (_glyphs == value)
            {
                return;
            }

            _glyphs = value;
            RebuildItemRenderCache();
        }
    }

    public bool IsVisible { get; private set; }

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.Rounded;

    public Thickness Padding
    {
        get;
        set;
    }

    public int AnchorX { get; private set; }

    public int AnchorY { get; private set; }

    public string? LastExecutedItemId { get; private set; }

    public IReadOnlyList<ContextMenuItem> Items => _items;

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

    public void SetItems(IEnumerable<ContextMenuItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
        }

        RebuildItemRenderCache();
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
        _hoveredIndex = -1;
    }

    public void OpenAt(int x, int y)
    {
        RequestFocus();
        IsVisible = true;
        AnchorX = Math.Max(0, x);
        AnchorY = Math.Max(0, y);
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
    }

    public void Close()
    {
        IsVisible = false;
    }

    public override bool Handle(Message message)
    {
        if (!IsVisible || !IsFocused || IsDisabled || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Escape))
        {
            Close();
            return true;
        }

        if (_items.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            _selectedIndex = (_selectedIndex + 1) % _items.Count;
            return true;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        if (!IsReadOnly && (key.Is(Key.Enter) || key.IsCharacter(' ')))
        {
            ExecuteItem(_selectedIndex);
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (!IsVisible || IsDisabled || message is not PointerInput pointer || !TryResolveMenuBounds(bounds, out var menuBounds, out var content))
        {
            return Handle(message);
        }

        var insideMenu = ContainsWithRightTolerance(menuBounds, pointer.X, pointer.Y);
        var changed = false;
        if (!insideMenu)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            if (pointer.Kind is PointerEventKind.Press or PointerEventKind.Release)
            {
                Close();
                changed = true;
            }

            return changed;
        }

        if (_items.Count == 0)
        {
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                _selectedIndex = (_selectedIndex + 1) % _items.Count;
                return true;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
                return true;
            }
        }

        if (!ContainsWithRightTolerance(content, pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            return changed;
        }

        var hovered = RowFromPointer(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind is PointerEventKind.Press or PointerEventKind.Release)
        {
            changed |= SetHoveredIndex(hovered);
            var leftActivate = pointer.Button == PointerButton.Left;
            if (leftActivate)
            {
                var target = hovered >= 0 ? hovered : _selectedIndex;
                if (target < 0 || target >= _items.Count)
                {
                    return changed;
                }

                if (_selectedIndex != target)
                {
                    _selectedIndex = target;
                    changed = true;
                }

                if (!IsReadOnly)
                {
                    ExecuteItem(_selectedIndex);
                    return true;
                }
            }
        }

        return changed;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeExecution(out string itemId)
    {
        if (_executionVersion == _consumedExecutionVersion || string.IsNullOrEmpty(LastExecutedItemId))
        {
            itemId = string.Empty;
            return false;
        }

        _consumedExecutionVersion = _executionVersion;
        itemId = LastExecutedItemId;
        return true;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || !TryResolveMenuBounds(clipped, out var menuBounds, out var content))
        {
            return;
        }

        if (Border != BorderStyle.None)
        {
            canvas.DrawBox(menuBounds, RenderTitleText(), Border, ResolveBorderStyleText());
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(empty)", MutedItemStyle), content.Width);
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var index = 0; index < rows; index++)
        {
            var rowText = ResolveRowText(_itemRenderCache[index], index);
            canvas.WriteText(
                content.X,
                content.Y + index,
                ApplyStyle(rowText, ResolveItemStyle(index)),
                content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ResolvePreferredWidth() + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private int RowFromPointer(Rect content, int y)
    {
        var row = y - content.Y;
        return row >= 0 && row < Math.Min(content.Height, _items.Count)
            ? row
            : -1;
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

    private TeaStyle ResolveItemStyle(int index)
    {
        var style = ItemStyle;
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

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

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle).Merge(MutedItemStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private string RenderTitleText()
    {
        var title = FormatTitleText();
        if (string.IsNullOrEmpty(title))
        {
            return title;
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(title, style);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title, " ", FocusMarker);
        }

        return Title;
    }

    private static bool ContainsWithRightTolerance(Rect rect, int x, int y)
    {
        return y >= rect.Y
            && y < rect.Bottom
            && x >= rect.X
            && x <= rect.Right;
    }

    private bool TryResolveMenuBounds(Rect bounds, out Rect menuBounds, out Rect content)
    {
        menuBounds = default;
        content = default;
        if (bounds.IsEmpty)
        {
            return false;
        }

        var width = Math.Min(ResolvePreferredWidth(), bounds.Width);
        var height = Math.Min(Math.Max(3, _items.Count + 2), bounds.Height);
        var x = Math.Clamp(AnchorX, bounds.X, Math.Max(bounds.X, bounds.Right - width));
        var y = Math.Clamp(AnchorY, bounds.Y, Math.Max(bounds.Y, bounds.Bottom - height));
        menuBounds = new Rect(x, y, width, height);
        content = FrameLayout.ResolveContentRect(menuBounds, Border, Padding);
        return !content.IsEmpty;
    }

    private void RebuildItemRenderCache()
    {
        _itemRenderCache.Clear();
        var width = 12;
        for (var index = 0; index < _items.Count; index++)
        {
            var item = _items[index];
            var cache = ContextMenuRenderCache.Create(item.Title, _glyphs);
            _itemRenderCache.Add(cache);
            width = Math.Max(width, cache.MinimumRowWidth);
        }

        _cachedItemWidth = width;
    }

    private int ResolvePreferredWidth()
    {
        var width = Math.Max(12, _cachedItemWidth);

        if (Border != BorderStyle.None)
        {
            // DrawBox writes title as $" {title} " inside a width-4 slot.
            // Reserve enough room so focused markers in titles are not clipped.
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 6);
        }

        return width;
    }

    private string ResolveRowText(ContextMenuRenderCache row, int index)
    {
        if (index == _selectedIndex)
        {
            return row.SelectedRowText;
        }

        if (index == _hoveredIndex)
        {
            return row.HoveredRowText;
        }

        return row.NormalRowText;
    }

    private static int ResolveMinimumRowWidth(string title, ContextMenuGlyphSet glyphs)
    {
        var markerWidth = Math.Max(
            ControlTextLayout.MeasureDisplayWidth(glyphs.NormalRowMarker),
            Math.Max(
                ControlTextLayout.MeasureDisplayWidth(glyphs.SelectedRowMarker),
                ControlTextLayout.MeasureDisplayWidth(glyphs.HoveredRowMarker)));
        return markerWidth + ControlTextLayout.MeasureDisplayWidth(glyphs.MarkerSeparator) + ControlTextLayout.MeasureDisplayWidth(title) + 2;
    }

    private void ExecuteItem(int index)
    {
        var item = _items[index];
        LastExecutedItemId = item.Id;
        _executionVersion++;
        ItemExecuted?.Invoke(this, new ContextMenuItemExecutedEventArgs(item));
        Close();
    }

    private readonly record struct ContextMenuRenderCache(
        string Title,
        string NormalRowText,
        string SelectedRowText,
        string HoveredRowText,
        int MinimumRowWidth)
    {
        public static ContextMenuRenderCache Create(string title, ContextMenuGlyphSet glyphs)
        {
            var safeTitle = title ?? string.Empty;
            return new ContextMenuRenderCache(
                safeTitle,
                string.Concat(glyphs.NormalRowMarker, glyphs.MarkerSeparator, safeTitle),
                string.Concat(glyphs.SelectedRowMarker, glyphs.MarkerSeparator, safeTitle),
                string.Concat(glyphs.HoveredRowMarker, glyphs.MarkerSeparator, safeTitle),
                ResolveMinimumRowWidth(safeTitle, glyphs));
        }
    }
}
