using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>Represents a selectable data-entry form surface with labeled fields.</summary>
public sealed class Form : Control
{
    private readonly List<FormField> _fields = [];
    private int _hoveredIndex = -1;
    private int _lastViewportRows = 8;
    private int _scrollOffset;

    /// <summary>Gets or sets form title text.</summary>
    public string Title { get; set; } = "Form";

    /// <summary>Gets or sets prefix rendered before <see cref="Title" />.</summary>
    public string TitlePrefix { get; set; } = string.Empty;

    /// <summary>Gets or sets suffix rendered after <see cref="Title" />.</summary>
    public string TitleSuffix { get; set; } = string.Empty;

    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether focus marker is shown while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets selected-row marker.</summary>
    public string SelectedMarker { get; set; } = ">";

    /// <summary>Gets or sets unselected-row marker.</summary>
    public string UnselectedMarker { get; set; } = " ";

    /// <summary>Gets or sets required-field marker.</summary>
    public string RequiredMarker { get; set; } = "*";

    /// <summary>Gets or sets empty-state text.</summary>
    public string EmptyText { get; set; } = "(no fields)";

    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets inner content padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets title style while not focused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets title style while focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets border style while not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets border style while focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets field-label style.</summary>
    public TesseraStyle LabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets field-value style.</summary>
    public TesseraStyle ValueStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets required-marker style.</summary>
    public TesseraStyle RequiredMarkerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into hovered rows.</summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into selected rows.</summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into selected rows while focused.</summary>
    public TesseraStyle FocusedSelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged while disabled.</summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets empty-state style.</summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets configured fields.</summary>
    public IReadOnlyList<FormField> Fields => _fields;

    /// <summary>Gets selected field index, or <c>-1</c> when empty.</summary>
    public int SelectedIndex { get; private set; } = -1;

    /// <summary>Gets selected field, if any.</summary>
    public FormField? SelectedField =>
        SelectedIndex >= 0 && SelectedIndex < _fields.Count ? _fields[SelectedIndex] : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Occurs when selected field changes.</summary>
    public event EventHandler<ListSelectionChangedEventArgs<FormField>>? SelectionChanged;

    /// <summary>Replaces all form fields.</summary>
    /// <param name="fields">Fields in visual order.</param>
    public void SetFields(IEnumerable<FormField> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        _fields.Clear();
        foreach (var field in fields)
        {
            _fields.Add(new FormField(field.Name, field.Label, field.Value, field.HelperText, field.IsRequired,
                field.IsDisabled));
        }

        SelectedIndex = ResolveFirstSelectable();
        _hoveredIndex = -1;
        _scrollOffset = 0;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _fields.Count == 0 || message is not KeyPressed key)
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

        if (key.Is(Key.PageDown))
        {
            return MoveByViewport(+1);
        }

        if (key.Is(Key.PageUp))
        {
            return MoveByViewport(-1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(ResolveFirstSelectable());
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(ResolveLastSelectable());
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveSelection(+1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1);
            }

            return false;
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1) || Handle(message);
            }

            return Handle(message);
        }

        EnsureSelectionVisible(content.Height);
        var rowIndex = _scrollOffset + (pointer.Y - content.Y);
        if (rowIndex < 0 || rowIndex >= _fields.Count)
        {
            return SetHoveredIndex(-1) || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(rowIndex);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            return SetSelectedIndex(rowIndex);
        }

        return false;
    }

    /// <inheritdoc />
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
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_fields.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveEmptyStyle()), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(content.Height);
        var visibleRows = Math.Min(content.Height, _fields.Count - _scrollOffset);
        for (var row = 0; row < visibleRows; row++)
        {
            var index = _scrollOffset + row;
            var rowText = FormatFieldRow(_fields[index], index == SelectedIndex);
            canvas.WriteText(content.X, content.Y + row, rowText, content.Width);
        }
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var i = 0; i < _fields.Count; i++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatFieldRow(_fields[i], true)));
        }

        var height = Math.Max(3, _fields.Count + 2);
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatFieldRow(FormField field, bool selected)
    {
        var marker = selected ? SelectedMarker : UnselectedMarker;
        var required = field.IsRequired && !string.IsNullOrWhiteSpace(RequiredMarker)
            ? ApplyStyle(RequiredMarker, ResolveRequiredStyle(selected, field.IsDisabled))
            : string.Empty;

        var rowStyle = ResolveRowStyle(selected ? SelectedIndex : -1, field.IsDisabled);
        var label = ApplyStyle($"{field.Label}{required}", LabelStyle.Merge(rowStyle));
        var value = ApplyStyle(field.Value, ValueStyle.Merge(rowStyle));
        var markerText = ApplyStyle(marker, rowStyle);
        return $"{markerText} {label} : {value}";
    }

    private bool MoveSelection(int direction)
    {
        if (_fields.Count == 0)
        {
            return false;
        }

        var index = SelectedIndex;
        if (index < 0)
        {
            index = direction > 0 ? -1 : _fields.Count;
        }

        for (var i = 0; i < _fields.Count; i++)
        {
            index += direction;
            if (index < 0)
            {
                index = _fields.Count - 1;
            }

            if (index >= _fields.Count)
            {
                index = 0;
            }

            if (!_fields[index].IsDisabled)
            {
                return SetSelectedIndex(index);
            }
        }

        return false;
    }

    private bool MoveByViewport(int direction)
    {
        var step = Math.Max(1, _lastViewportRows - 1);
        var target = (SelectedIndex < 0 ? 0 : SelectedIndex) + direction * step;
        return SetSelectedIndex(Math.Clamp(target, 0, _fields.Count - 1)) || MoveSelection(direction);
    }

    private bool SetSelectedIndex(int index)
    {
        if (index < 0 || index >= _fields.Count || _fields[index].IsDisabled || SelectedIndex == index)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedField;
        SelectedIndex = index;
        EnsureSelectionVisible(_lastViewportRows);
        SelectionChanged?.Invoke(this,
            new ListSelectionChangedEventArgs<FormField>(previousIndex, SelectedIndex, previousItem,
                _fields[SelectedIndex]));
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

    private int ResolveFirstSelectable()
    {
        for (var i = 0; i < _fields.Count; i++)
        {
            if (!_fields[i].IsDisabled)
            {
                return i;
            }
        }

        return -1;
    }

    private int ResolveLastSelectable()
    {
        for (var i = _fields.Count - 1; i >= 0; i--)
        {
            if (!_fields[i].IsDisabled)
            {
                return i;
            }
        }

        return -1;
    }

    private void EnsureSelectionVisible(int viewportHeight)
    {
        if (SelectedIndex < 0 || _fields.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var viewport = Math.Max(1, viewportHeight);
        if (SelectedIndex < _scrollOffset)
        {
            _scrollOffset = SelectedIndex;
            return;
        }

        if (SelectedIndex >= _scrollOffset + viewport)
        {
            _scrollOffset = SelectedIndex - viewport + 1;
        }
    }

    private TesseraStyle ResolveRowStyle(int index, bool fieldDisabled)
    {
        var style = TesseraStyle.Empty;
        if (index >= 0 && index == _hoveredIndex)
        {
            style = style.Merge(HoveredRowStyle);
        }

        if (index >= 0 && index == SelectedIndex)
        {
            style = style.Merge(SelectedRowStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedRowStyle);
            }
        }

        if (fieldDisabled || IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TesseraStyle ResolveRequiredStyle(bool selected, bool fieldDisabled)
    {
        var index = selected ? SelectedIndex : -1;
        return RequiredMarkerStyle.Merge(ResolveRowStyle(index, fieldDisabled));
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolveEmptyStyle()
    {
        return IsDisabled ? EmptyStyle.Merge(DisabledStyle) : EmptyStyle;
    }

    private string RenderTitle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return ApplyStyle(MeasureTitle(), style);
    }

    private string MeasureTitle()
    {
        var title = $"{TitlePrefix}{Title}{TitleSuffix}";
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{title} {FocusMarker}"
            : title;
    }

    private static string ApplyStyle(string value, TesseraStyle style)
    {
        return style.IsEmpty ? value : style.Render(value);
    }
}
