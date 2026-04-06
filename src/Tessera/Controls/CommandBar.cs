using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a compact single-line command bar with keyboard and pointer activation.
/// </summary>
public sealed class CommandBar : Control
{
    private readonly List<CommandBarItem> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Occurs when a command is activated.
    /// </summary>
    public event EventHandler<CommandBarItemActivatedEventArgs>? ItemActivated;

    /// <summary>
    /// Gets the configured command items.
    /// </summary>
    public IReadOnlyList<CommandBarItem> Items => _items;

    /// <summary>
    /// Gets the currently selected command index.
    /// Returns <c>-1</c> when no items are configured.
    /// </summary>
    public int SelectedIndex => _items.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets the currently selected command item.
    /// </summary>
    public CommandBarItem? SelectedItem => _items.Count == 0 ? null : _items[_selectedIndex];

    /// <summary>
    /// Gets or sets the optional title shown before command entries.
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
    /// Gets or sets a value indicating whether the title focus marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the style applied to the title when not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to the title when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the base style used for command labels.
    /// </summary>
    public TesseraStyle ItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged on top of <see cref="ItemStyle"/> for hovered commands.
    /// </summary>
    public TesseraStyle HoveredItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged on top of <see cref="ItemStyle"/> for the selected command.
    /// </summary>
    public TesseraStyle SelectedItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged on top of <see cref="ItemStyle"/> for disabled commands.
    /// </summary>
    public TesseraStyle DisabledItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style used for separators between command labels.
    /// </summary>
    public TesseraStyle SeparatorStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the text used between command labels.
    /// </summary>
    public string ItemSeparator
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets the text rendered in front of the selected command.
    /// </summary>
    public string SelectedPrefix
    {
        get;
        set => field = value ?? string.Empty;
    } = "[";

    /// <summary>
    /// Gets or sets the text rendered after the selected command.
    /// </summary>
    public string SelectedSuffix
    {
        get;
        set => field = value ?? string.Empty;
    } = "]";

    /// <summary>
    /// Gets the last activated command id.
    /// </summary>
    public string? LastActivatedItemId { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command bar currently owns focus.
    /// </summary>
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command bar should ignore interaction.
    /// </summary>
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command bar remains interactive but does not activate commands.
    /// </summary>
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces the configured command items.
    /// </summary>
    /// <param name="items">The command items to display.</param>
    public void SetItems(IEnumerable<CommandBarItem> items)
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

    /// <summary>
    /// Selects a command by index with bounds clamping.
    /// </summary>
    /// <param name="index">The requested command index.</param>
    public void Select(int index)
    {
        if (_items.Count == 0)
        {
            return;
        }

        _selectedIndex = Math.Clamp(index, 0, _items.Count - 1);
    }

    /// <summary>
    /// Activates the currently selected command.
    /// </summary>
    /// <returns><see langword="true"/> when a command was activated; otherwise, <see langword="false"/>.</returns>
    public bool ActivateSelected()
    {
        if (_items.Count == 0 || IsReadOnly)
        {
            return false;
        }

        var item = _items[_selectedIndex];
        if (item.IsDisabled)
        {
            return false;
        }

        ActivateItem(item);
        return true;
    }

    /// <summary>
    /// Handles keyboard navigation and activation input.
    /// </summary>
    /// <param name="message">The message to process.</param>
    /// <returns><see langword="true"/> when the message changed selection or activated a command; otherwise, <see langword="false"/>.</returns>
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

                var changed = SetSelectedIndex(index);
                if (!IsReadOnly && !_items[index].IsDisabled)
                {
                    ActivateItem(_items[index]);
                    return true;
                }

                return changed;
            }
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return MoveSelection(1);
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return MoveSelection(-1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_items.Count - 1);
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            return ActivateSelected();
        }

        return false;
    }

    /// <summary>
    /// Handles pointer hover, wheel navigation, and click activation input.
    /// </summary>
    /// <param name="message">The message to process.</param>
    /// <param name="bounds">The current command-bar bounds.</param>
    /// <returns><see langword="true"/> when the message changed state or activated a command; otherwise, <see langword="false"/>.</returns>
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || _items.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var inRow = bounds.Contains(pointer.X, pointer.Y) && pointer.Y == bounds.Y;
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
                return MoveSelection(1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1);
            }
        }

        var hit = HitTestItemIndex(pointer.X, bounds);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hit);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            changed |= SetHoveredIndex(hit);
            if (hit >= 0)
            {
                changed |= SetSelectedIndex(hit);
                if (!IsReadOnly && !_items[hit].IsDisabled)
                {
                    ActivateItem(_items[hit]);
                    return true;
                }
            }
        }

        return changed || Handle(message);
    }

    /// <summary>
    /// Renders the command bar into a single clipped row.
    /// </summary>
    /// <param name="canvas">The target canvas.</param>
    /// <param name="rect">The bounds assigned to the command bar.</param>
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var x = clipped.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            canvas.WriteText(x, clipped.Y, RenderTitle(title), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(title) + (_items.Count > 0 ? 1 : 0);
        }

        for (var index = 0; index < _items.Count && x < clipped.Right; index++)
        {
            if (index > 0 && !string.IsNullOrEmpty(ItemSeparator))
            {
                canvas.WriteText(x, clipped.Y, RenderStyled(ItemSeparator, SeparatorStyle), clipped.Right - x);
                x += ControlTextLayout.MeasureDisplayWidth(ItemSeparator);
            }

            if (x >= clipped.Right)
            {
                break;
            }

            var label = FormatItemLabel(index);
            canvas.WriteText(x, clipped.Y, RenderStyled(label, ResolveItemStyle(index)), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(label);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            width += ControlTextLayout.MeasureDisplayWidth(title) + (_items.Count > 0 ? 1 : 0);
        }

        for (var index = 0; index < _items.Count; index++)
        {
            if (index > 0)
            {
                width += ControlTextLayout.MeasureDisplayWidth(ItemSeparator);
            }

            width += ControlTextLayout.MeasureDisplayWidth(FormatItemLabel(index));
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(width == 0 ? 0 : 1, 0, availableBounds.Height));
    }

    private bool MoveSelection(int delta)
    {
        if (_items.Count == 0 || delta == 0)
        {
            return false;
        }

        var target = (_selectedIndex + delta + _items.Count) % _items.Count;
        return SetSelectedIndex(target);
    }

    private bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _items.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = clamped;
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

    private void ActivateItem(CommandBarItem item)
    {
        LastActivatedItemId = item.Id;
        ItemActivated?.Invoke(this, new CommandBarItemActivatedEventArgs(item));
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
        return RenderStyled(title, style);
    }

    private string FormatItemLabel(int index)
    {
        var item = _items[index];
        var core = item.Shortcut == '\0'
            ? item.Text
            : $"{item.Text}({item.Shortcut})";
        if (index == _selectedIndex)
        {
            return $"{SelectedPrefix}{core}{SelectedSuffix}";
        }

        return $" {core} ";
    }

    private TesseraStyle ResolveItemStyle(int index)
    {
        var style = ItemStyle;
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }
        else if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (_items[index].IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private static string RenderStyled(string text, TesseraStyle style)
    {
        if (style.IsEmpty || string.IsNullOrEmpty(text))
        {
            return text;
        }

        return style.Render(text);
    }

    private int HitTestItemIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            cursor += ControlTextLayout.MeasureDisplayWidth(title) + (_items.Count > 0 ? 1 : 0);
        }

        for (var index = 0; index < _items.Count && cursor < bounds.Right; index++)
        {
            if (index > 0)
            {
                cursor += ControlTextLayout.MeasureDisplayWidth(ItemSeparator);
            }

            var label = FormatItemLabel(index);
            var width = ControlTextLayout.MeasureDisplayWidth(label);
            var end = cursor + width;
            if (x >= cursor && x < end)
            {
                return index;
            }

            cursor = end;
        }

        return -1;
    }
}
