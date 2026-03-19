using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a control for choosing multiple items from a list.
/// </summary>
public sealed class MultiSelect : Control
{
    private readonly List<(string Label, bool Checked)> _items = [];

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Checklist";

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
    /// Gets or sets the title style applied when not focused.
    /// </summary>
    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style applied when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the base style applied to item rows.
    /// </summary>
    public TeaStyle ItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into selected item rows.
    /// </summary>
    public TeaStyle SelectedItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into checked item rows.
    /// </summary>
    public TeaStyle CheckedItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged when the control is disabled.
    /// </summary>
    public TeaStyle DisabledItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the marker shown before the selected row.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "›";

    /// <summary>
    /// Gets or sets the marker shown before unselected rows.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets the marker shown for checked rows.
    /// </summary>
    public string CheckedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "[x]";

    /// <summary>
    /// Gets or sets the marker shown for unchecked rows.
    /// </summary>
    public string UncheckedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "[ ]";

    public int SelectedIndex { get; private set; }

    public string? SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _items.Count
            ? _items[SelectedIndex].Label
            : null;

    public IReadOnlyList<string> CheckedItems =>
        _items.Where(static item => item.Checked).Select(static item => item.Label).ToArray();

    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        SetItems(items.Select(static item => (item, false)));
    }

    public void SetItems(IEnumerable<(string Label, bool Checked)> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down))
        {
            var next = Math.Min(_items.Count - 1, SelectedIndex + 1);
            if (next == SelectedIndex)
            {
                return false;
            }

            SelectedIndex = next;
            return true;
        }

        if (key.Is(Key.Up))
        {
            var previous = Math.Max(0, SelectedIndex - 1);
            if (previous == SelectedIndex)
            {
                return false;
            }

            SelectedIndex = previous;
            return true;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            var item = _items[SelectedIndex];
            _items[SelectedIndex] = (item.Label, !item.Checked);
            return true;
        }

        return false;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, RenderTitle());
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var row = 0; row < rows; row++)
        {
            var item = _items[row];
            var selected = row == SelectedIndex ? SelectedMarker : UnselectedMarker;
            var marker = item.Checked ? CheckedMarker : UncheckedMarker;
            var line = $"{selected} {marker} {item.Label}";
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveItemStyle(row, item.Checked)), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(FormatTitleText(includeFocusMarkerWhenUnfocused: true)) + 4;
        var prefixWidth = Math.Max(
                ControlTextLayout.MeasureDisplayWidth(SelectedMarker),
                ControlTextLayout.MeasureDisplayWidth(UnselectedMarker))
            + 1
            + Math.Max(
                ControlTextLayout.MeasureDisplayWidth(CheckedMarker),
                ControlTextLayout.MeasureDisplayWidth(UncheckedMarker))
            + 1;
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, prefixWidth + ControlTextLayout.MeasureDisplayWidth(_items[index].Label) + 2);
        }

        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatTitleText(bool includeFocusMarkerWhenUnfocused = false)
    {
        if ((IsFocused || includeFocusMarkerWhenUnfocused) && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TeaStyle ResolveItemStyle(int row, bool isChecked)
    {
        var style = ItemStyle;
        if (isChecked)
        {
            style = style.Merge(CheckedItemStyle);
        }

        if (row == SelectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
