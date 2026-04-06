using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class DataForm<TModel>
{
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
            Border == BorderStyle.None ? null : RenderTitleFrameText(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        var y = content.Y;
        if (ShouldRenderInlineTitle())
        {
            WriteStyledText(canvas, content.X, y, RenderTitleText(), ResolveTitleStyle(), content.Width);
            y++;
        }

        var statusRow = ShouldRenderStatusLine(content, y) ? 1 : 0;
        var rowsHeight = Math.Max(0, content.Bottom - y - statusRow);
        _lastViewportRows = Math.Max(1, rowsHeight);

        if (_fields.Count == 0 || rowsHeight <= 0)
        {
            if (rowsHeight > 0)
            {
                WriteStyledText(canvas, content.X, y, EmptyText, ResolveEmptyStyle(), content.Width);
            }

            if (statusRow > 0)
            {
                RenderStatusLine(canvas, content);
            }

            return;
        }

        EnsureSelectionVisible(rowsHeight);
        var labelWidth = ResolveLabelWidth();
        var visible = Math.Min(rowsHeight, _fields.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var index = _scrollOffset + row;
            RenderFieldRow(canvas, content.X, y + row, content.Width, labelWidth, index, _fields[index]);
        }

        if (statusRow > 0)
        {
            RenderStatusLine(canvas, content);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var labelWidth = ResolveLabelWidth();
        var width = Math.Max(24, labelWidth + 18 + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        if (ShouldRenderInlineTitle() || !string.IsNullOrWhiteSpace(Title))
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(RenderTitleText()) + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        }

        var height = Math.Max(1, _fields.Count) + 1 + (ShouldRenderInlineTitle() ? 1 : 0) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderFieldRow(Canvas canvas, int x, int y, int width, int labelWidth, int index, DataFormField<TModel> field)
    {
        var marker = index == _selectedIndex ? SelectedMarker : UnselectedMarker;
        var label = NormalizeSingleLine(field.Label).PadRight(labelWidth);
        var prefix = string.Concat(marker, " ", label, FieldSeparatorText);
        var prefixStyle = ResolvePrefixStyle(index, field);
        WriteStyledText(canvas, x, y, prefix, prefixStyle, width);

        var prefixWidth = Math.Min(width, ControlTextLayout.MeasureDisplayWidth(prefix));
        if (prefixWidth >= width)
        {
            return;
        }

        var value = ResolveDisplayedValue(index, field, out var isPlaceholder);
        var valueStyle = ResolveValueSegmentStyle(index, field, isPlaceholder);
        WriteStyledText(canvas, x + prefixWidth, y, value, valueStyle, width - prefixWidth);
    }

    private void RenderStatusLine(Canvas canvas, Rect content)
    {
        if (!TryResolveStatusLine(out var text, out var style))
        {
            return;
        }

        WriteStyledText(canvas, content.X, content.Bottom - 1, text, style, content.Width);
    }

    private bool ShouldRenderStatusLine(Rect content, int rowsTop)
    {
        if (_fields.Count == 0)
        {
            return false;
        }

        return content.Bottom - rowsTop >= 2;
    }

    private bool TryResolveStatusLine(out string text, out TesseraStyle style)
    {
        if (IsDisabled)
        {
            text = "Disabled. Selection and editing are blocked.";
            style = ResolveEmptyStyle().Merge(DisabledStyle);
            return true;
        }

        if (!string.IsNullOrWhiteSpace(_lastCommitError))
        {
            text = string.Concat("! Validation failed: ", _lastCommitError);
            style = SelectedFieldStyle.Merge(FocusedSelectedFieldStyle).Merge(ErrorStyle);
            return true;
        }

        if (SelectedField is null)
        {
            text = string.Empty;
            style = TesseraStyle.Empty;
            return false;
        }

        if (_isEditing)
        {
            text = string.Concat("Editing ", NormalizeSingleLine(SelectedField.Label), ". Enter commits, Esc cancels.");
            style = ValueStyle.Merge(FocusedSelectedFieldStyle);
            return true;
        }

        if (CanEditCurrentField())
        {
            text = string.Concat("Selected ", NormalizeSingleLine(SelectedField.Label), ". Press Enter to edit.");
            style = PlaceholderStyle.IsEmpty ? ValueStyle : PlaceholderStyle;
            return true;
        }

        if (Model is null)
        {
            text = "No model bound. Call SetModel(...) before editing.";
            style = ResolveEmptyStyle();
            return true;
        }

        text = string.Concat("Selected ", NormalizeSingleLine(SelectedField.Label), ". Read-only.");
        style = ValueStyle.Merge(ReadOnlyFieldStyle);
        return true;
    }

    private int ResolveRenderableRowsHeight(Rect content, int rowTop)
    {
        var statusRow = ShouldRenderStatusLine(content, rowTop) ? 1 : 0;
        return Math.Max(0, content.Bottom - rowTop - statusRow);
    }

    private int ResolveRowsTop(Rect content)
    {
        return ShouldRenderInlineTitle() ? content.Y + 1 : content.Y;
    }

    private bool ShouldRenderInlineTitle() => Border == BorderStyle.None && !string.IsNullOrWhiteSpace(Title);

    private int ResolveLabelWidth()
    {
        var width = 6;
        for (var index = 0; index < _fields.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(_fields[index].Label));
        }

        return Math.Clamp(width, 4, Math.Max(4, MaxLabelWidth));
    }

    private string ResolveDisplayedValue(int index, DataFormField<TModel> field, out bool isPlaceholder)
    {
        if (index == _selectedIndex && _isEditing)
        {
            isPlaceholder = false;
            return string.Concat(_editBuffer, "|");
        }

        if (Model is null)
        {
            isPlaceholder = true;
            return NoModelText;
        }

        var value = SafeReadValue(field, Model);
        if (string.IsNullOrEmpty(value))
        {
            var placeholder = string.IsNullOrWhiteSpace(field.Placeholder) ? NoModelText : field.Placeholder;
            isPlaceholder = true;
            return placeholder;
        }

        isPlaceholder = false;
        return value;
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_fields.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _selectedIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _fields.Count - viewportRows));
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolveTitleStyle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolvePrefixStyle(int index, DataFormField<TModel> field)
    {
        var style = TesseraStyle.Empty;
        if (field.IsReadOnly || !field.CanWrite)
        {
            style = style.Merge(ReadOnlyFieldStyle);
        }

        if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredFieldStyle);
        }

        if (index == _selectedIndex)
        {
            if (!_isEditing)
            {
                style = style.Merge(SelectedFieldStyle);
                if (IsFocused)
                {
                    style = style.Merge(FocusedSelectedFieldStyle);
                }
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style.Merge(LabelStyle);
    }

    private TesseraStyle ResolveValueSegmentStyle(int index, DataFormField<TModel> field, bool isPlaceholder)
    {
        var style = TesseraStyle.Empty;
        if (field.IsReadOnly || !field.CanWrite)
        {
            style = style.Merge(ReadOnlyFieldStyle);
        }

        if (index == _hoveredIndex && !(index == _selectedIndex && _isEditing))
        {
            style = style.Merge(HoveredFieldStyle);
        }

        if (index == _selectedIndex)
        {
            if (_isEditing)
            {
                style = style.Merge(SelectedFieldStyle);
                if (IsFocused)
                {
                    style = style.Merge(FocusedSelectedFieldStyle);
                }
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        style = style.Merge(isPlaceholder ? PlaceholderStyle : ValueStyle);
        if (index == _selectedIndex && !string.IsNullOrWhiteSpace(_lastCommitError))
        {
            style = style.Merge(ErrorStyle);
        }

        return style;
    }

    private TesseraStyle ResolveEmptyStyle()
    {
        return IsDisabled ? PlaceholderStyle.Merge(DisabledStyle) : PlaceholderStyle;
    }

    private string RenderTitleText()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title, " ", FocusMarker);
        }

        return Title;
    }

    private string RenderTitleFrameText()
    {
        var title = RenderTitleText();
        var style = ResolveTitleStyle();
        return style.IsEmpty ? title : style.Render(title);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, DataFormField<TModel>? previousField)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousField, SelectedField))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new DataFormSelectionChangedEventArgs<TModel>(previousIndex, _selectedIndex, previousField, SelectedField));
    }

    private static string NormalizeSingleLine(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Replace('\r', ' ').Replace('\n', ' ');
    }

    private static void WriteStyledText(Canvas canvas, int x, int y, string text, TesseraStyle style, int width)
    {
        if (width <= 0)
        {
            return;
        }

        canvas.WriteText(x, y, style.IsEmpty ? text : style.Render(text), width);
    }
}
