using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a horizontal menu bar that raises menu-item activation events.
/// </summary>
public sealed class MenuBar : Control
{
    private readonly List<MenuItem> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;
    private long _activationVersion;
    private long _consumedActivationVersion;

    /// <summary>
    /// Occurs when a configured menu item is activated.
    /// </summary>
    public event EventHandler<MenuItemActivatedEventArgs>? ItemActivated;

    /// <summary>
    /// Gets the currently selected menu index.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets the configured menu items.
    /// </summary>
    public IReadOnlyList<MenuItem> Items => _items;

    /// <summary>
    /// Gets or sets default style for menu items.
    /// </summary>
    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected item labels.
    /// </summary>
    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered item labels.
    /// </summary>
    public TeaStyle HoveredItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into the selected item while focused.
    /// </summary>
    public TeaStyle FocusedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when the control is disabled.
    /// </summary>
    public TeaStyle DisabledItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets glyphs used for selected/hovered wrappers and shortcut delimiters.
    /// </summary>
    public MenuBarGlyphSet Glyphs { get; set; } = MenuBarGlyphSet.Default;

    /// <summary>
    /// Gets or sets the optional frame border style.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="BorderStyle.None"/> to preserve previous single-row rendering.
    /// </remarks>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.None;

    /// <summary>
    /// Gets or sets inner padding applied inside the optional frame.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

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

    public string? LastActivatedItemId { get; private set; }

    /// <summary>
    /// Replaces the configured menu items.
    /// </summary>
    /// <param name="items">The menu items to display. Item ids should remain stable and unique for activation handling.</param>
    public void SetItems(IEnumerable<MenuItem> items)
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

        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
        _hoveredIndex = -1;
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Modifiers == ModifierKeys.None && key.Key == Key.Character && key.Text.Length == 1)
        {
            var shortcut = char.ToLowerInvariant(key.Text[0]);
            for (var index = 0; index < _items.Count; index++)
            {
                if (char.ToLowerInvariant(_items[index].Shortcut) != shortcut)
                {
                    continue;
                }

                _selectedIndex = index;
                if (!IsReadOnly)
                {
                    ActivateItem(_items[index]);
                }

                return true;
            }
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            _selectedIndex = (_selectedIndex + 1) % _items.Count;
            return true;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            _selectedIndex = (_selectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        if (!IsReadOnly && (key.Is(Key.Enter) || key.IsCharacter(' ')))
        {
            ActivateItem(_items[_selectedIndex]);
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || _items.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inRow = content.Contains(pointer.X, pointer.Y) && pointer.Y == content.Y;
        var changed = false;
        if (!inRow)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            return changed || Handle(message);
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

        var hovered = HitTestItemIndex(pointer.X, content);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            if (hovered >= 0 && _selectedIndex != hovered)
            {
                _selectedIndex = hovered;
                return SetHoveredIndex(hovered) || true;
            }

            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press)
        {
            changed |= SetHoveredIndex(hovered);
            if (pointer.Button == PointerButton.Left && hovered >= 0)
            {
                if (_selectedIndex != hovered)
                {
                    _selectedIndex = hovered;
                    changed = true;
                }

                if (!IsReadOnly)
                {
                    ActivateItem(_items[_selectedIndex]);
                    return true;
                }
            }
        }

        return changed || Handle(message);
    }

    /// <summary>
    /// Attempts to consume a pending menu activation from the wrapped legacy component.
    /// </summary>
    /// <param name="itemId">Receives the activated item id when available.</param>
    /// <returns><see langword="true"/> when an activation was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeActivation(out string itemId)
    {
        if (_activationVersion == _consumedActivationVersion || string.IsNullOrEmpty(LastActivatedItemId))
        {
            itemId = string.Empty;
            return false;
        }

        _consumedActivationVersion = _activationVersion;
        itemId = LastActivatedItemId;
        return true;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _items.Count == 0)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, null, Border, Padding, ResolveBorderStyleText());
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var x = content.X;
        for (var index = 0; index < _items.Count && x < content.Right; index++)
        {
            var rawLabel = FormatLabel(index, hovered: index == _hoveredIndex);
            canvas.WriteText(x, content.Y, ApplyStyle(rawLabel, ResolveItemStyle(index)), content.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(rawLabel) + 1;
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

        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = (_items.Count == 0 ? 0 : 1) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatLabel(int index, bool hovered)
    {
        var item = _items[index];
        var core = item.Shortcut == '\0'
            ? item.Text
            : string.Concat(item.Text, Glyphs.ShortcutOpen, item.Shortcut, Glyphs.ShortcutClose);
        var label = index == _selectedIndex
            ? string.Concat(Glyphs.SelectedPrefix, core, Glyphs.SelectedSuffix)
            : string.Concat(Glyphs.UnselectedPrefix, core, Glyphs.UnselectedSuffix);
        return hovered && index != _selectedIndex
            ? string.Concat(Glyphs.HoveredPrefix, core, Glyphs.HoveredSuffix)
            : label;
    }

    private int HitTestItemIndex(int x, Rect content)
    {
        var cursor = content.X;
        for (var index = 0; index < _items.Count && cursor < content.Right; index++)
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

    private TeaStyle ResolveItemStyle(int index)
    {
        var style = ItemStyle;
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedItemStyle);
            }
        }

        if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
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
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private void ActivateItem(MenuItem item)
    {
        LastActivatedItemId = item.Id;
        _activationVersion++;
        ItemActivated?.Invoke(this, new MenuItemActivatedEventArgs(item));
    }
}
