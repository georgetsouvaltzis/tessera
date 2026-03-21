using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a sectioned inspector with collapsible key/value and detail rows.
/// </summary>
public sealed class InspectorPanel : Control
{
    private readonly List<InspectorSection> _sections = [];
    private int _selectedRowIndex = -1;
    private int _scrollOffset;

    /// <summary>Gets or sets panel title.</summary>
    public string Title { get; set; } = "Inspector";
    /// <summary>Gets or sets marker appended to <see cref="Title"/> while focused.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Gets or sets whether <see cref="FocusMarker"/> is rendered while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Gets or sets marker shown for expanded sections.</summary>
    public string ExpandedMarker { get; set; } = "▾";
    /// <summary>Gets or sets marker shown for collapsed sections.</summary>
    public string CollapsedMarker { get; set; } = "▸";
    /// <summary>Gets or sets preferred key column width.</summary>
    public int PreferredKeyWidth { get; set; } = 20;
    /// <summary>Gets or sets text shown when no sections exist.</summary>
    public string EmptyText { get; set; } = "(empty)";

    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Gets or sets content padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets title style while not focused.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets title style while focused.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets border style while not focused.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets border style while focused.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets section title style.</summary>
    public TeaStyle SectionStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets selected section title style.</summary>
    public TeaStyle SelectedSectionStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets key style for field rows.</summary>
    public TeaStyle KeyStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets value style for field rows.</summary>
    public TeaStyle ValueStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets detail style for detail rows.</summary>
    public TeaStyle DetailStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets marker style for expand/collapse glyphs.</summary>
    public TeaStyle MarkerStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets style merged into selected row text.</summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets style merged into selected row text while focused.</summary>
    public TeaStyle FocusedSelectedRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets style merged while disabled.</summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets empty-state text style.</summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets configured sections.</summary>
    public IReadOnlyList<InspectorSection> Sections => _sections;
    /// <summary>Gets selected visible-row index, or <c>-1</c> when empty.</summary>
    public int SelectedRowIndex => _selectedRowIndex;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces panel sections.
    /// </summary>
    /// <param name="sections">Sections to render.</param>
    public void SetSections(IEnumerable<InspectorSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);
        _sections.Clear();
        foreach (var section in sections)
        {
            if (section is not null)
            {
                _sections.Add(Clone(section));
            }
        }

        _selectedRowIndex = BuildRows().Count == 0 ? -1 : Math.Clamp(_selectedRowIndex < 0 ? 0 : _selectedRowIndex, 0, BuildRows().Count - 1);
        _scrollOffset = 0;
    }

    /// <summary>
    /// Toggles expanded state for one section.
    /// </summary>
    /// <param name="index">Section index.</param>
    /// <returns><see langword="true"/> when the section was toggled.</returns>
    public bool ToggleSection(int index)
    {
        if (index < 0 || index >= _sections.Count)
        {
            return false;
        }

        _sections[index].IsExpanded = !_sections[index].IsExpanded;
        var rows = BuildRows();
        _selectedRowIndex = rows.Count == 0 ? -1 : Math.Clamp(_selectedRowIndex, 0, rows.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, rows.Count - 1));
        return true;
    }

    /// <summary>
    /// Sets selected visible-row index.
    /// </summary>
    /// <param name="index">Requested row index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedRowIndex(int index)
    {
        var rows = BuildRows();
        if (rows.Count == 0)
        {
            if (_selectedRowIndex == -1)
            {
                return false;
            }

            _selectedRowIndex = -1;
            return true;
        }

        var clamped = Math.Clamp(index, 0, rows.Count - 1);
        if (clamped == _selectedRowIndex)
        {
            return false;
        }

        _selectedRowIndex = clamped;
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        var rows = BuildRows();
        if (rows.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedRowIndex(_selectedRowIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedRowIndex(_selectedRowIndex - 1);
        if (key.Is(Key.Home)) return SetSelectedRowIndex(0);
        if (key.Is(Key.End)) return SetSelectedRowIndex(rows.Count - 1);

        if ((key.Is(Key.Enter) || key.IsCharacter(' ')) && TryGetSelectedSectionHeader(rows, out var sectionIndex))
        {
            return ToggleSection(sectionIndex);
        }

        if (key.Is(Key.Left) && TryGetSelectedSectionOrAncestor(rows, out sectionIndex) && _sections[sectionIndex].IsExpanded)
        {
            return ToggleSection(sectionIndex);
        }

        if (key.Is(Key.Right) && TryGetSelectedSectionOrAncestor(rows, out sectionIndex) && !_sections[sectionIndex].IsExpanded)
        {
            return ToggleSection(sectionIndex);
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
        if (content.IsEmpty || !content.Contains(pointer.X, pointer.Y))
        {
            return Handle(message);
        }

        var rows = BuildRows();
        if (rows.Count == 0)
        {
            return false;
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedRowIndex(_selectedRowIndex + 1);
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedRowIndex(_selectedRowIndex - 1);
            return false;
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return false;
        }

        RequestFocus();
        var row = pointer.Y - content.Y;
        if (row < 0)
        {
            return false;
        }

        EnsureSelectionVisible(content.Height, rows.Count);
        var index = _scrollOffset + row;
        if (index < 0 || index >= rows.Count)
        {
            return false;
        }

        var changed = SetSelectedRowIndex(index);
        if (rows[index].Kind == InspectorRowKind.SectionHeader)
        {
            changed |= ToggleSection(rows[index].SectionIndex);
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, title, Border, Padding, ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        var rows = BuildRows();
        if (rows.Count == 0)
        {
            Write(canvas, content.X, content.Y, EmptyText, ResolveStyle(EmptyStyle), content.Width);
            return;
        }

        EnsureSelectionVisible(content.Height, rows.Count);
        var keyWidth = Math.Max(6, Math.Min(PreferredKeyWidth, Math.Max(6, content.Width / 2)));
        var visibleRows = Math.Min(content.Height, rows.Count - _scrollOffset);
        for (var row = 0; row < visibleRows; row++)
        {
            var index = _scrollOffset + row;
            var current = rows[index];
            RenderRow(canvas, content, content.Y + row, keyWidth, current, index == _selectedRowIndex);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var rows = BuildRows();
        var width = Math.Max(20, ControlTextLayout.MeasureDisplayWidth(Title) + 4);
        var height = Math.Max(4, Math.Max(1, rows.Count) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2));
        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderRow(Canvas canvas, Rect content, int y, int keyWidth, InspectorRow row, bool selected)
    {
        var selectedStyle = selected ? ResolveSelectedStyle() : TeaStyle.Empty;
        if (row.Kind == InspectorRowKind.SectionHeader)
        {
            var marker = _sections[row.SectionIndex].IsExpanded ? ExpandedMarker : CollapsedMarker;
            var markerText = Apply(marker, ResolveStyle(MarkerStyle.Merge(selectedStyle)));
            var title = Apply(_sections[row.SectionIndex].Title, ResolveStyle(SectionStyle.Merge(selected ? SelectedSectionStyle : TeaStyle.Empty).Merge(selectedStyle)));
            Write(canvas, content.X, y, string.Concat(markerText, " ", title), TeaStyle.Empty, content.Width);
            return;
        }

        if (row.Kind == InspectorRowKind.Field)
        {
            var field = _sections[row.SectionIndex].Fields[row.ItemIndex];
            var key = Apply(Pad(field.Key, keyWidth), ResolveStyle(KeyStyle.Merge(selectedStyle)));
            var value = Apply(field.Value, ResolveStyle(ValueStyle.Merge(selectedStyle)));
            Write(canvas, content.X, y, string.Concat("  ", key, " : ", value), TeaStyle.Empty, content.Width);
            return;
        }

        var detail = _sections[row.SectionIndex].Details[row.ItemIndex];
        var text = Apply(string.Concat("    ", detail), ResolveStyle(DetailStyle.Merge(selectedStyle)));
        Write(canvas, content.X, y, text, TeaStyle.Empty, content.Width);
    }

    private List<InspectorRow> BuildRows()
    {
        var rows = new List<InspectorRow>(_sections.Count * 3);
        for (var sectionIndex = 0; sectionIndex < _sections.Count; sectionIndex++)
        {
            rows.Add(new InspectorRow(sectionIndex, InspectorRowKind.SectionHeader, -1));
            var section = _sections[sectionIndex];
            if (!section.IsExpanded)
            {
                continue;
            }

            for (var fieldIndex = 0; fieldIndex < section.Fields.Count; fieldIndex++)
            {
                rows.Add(new InspectorRow(sectionIndex, InspectorRowKind.Field, fieldIndex));
            }

            for (var detailIndex = 0; detailIndex < section.Details.Count; detailIndex++)
            {
                rows.Add(new InspectorRow(sectionIndex, InspectorRowKind.Detail, detailIndex));
            }
        }

        return rows;
    }

    private bool TryGetSelectedSectionHeader(IReadOnlyList<InspectorRow> rows, out int sectionIndex)
    {
        sectionIndex = -1;
        if (_selectedRowIndex < 0 || _selectedRowIndex >= rows.Count)
        {
            return false;
        }

        var row = rows[_selectedRowIndex];
        if (row.Kind != InspectorRowKind.SectionHeader)
        {
            return false;
        }

        sectionIndex = row.SectionIndex;
        return true;
    }

    private bool TryGetSelectedSectionOrAncestor(IReadOnlyList<InspectorRow> rows, out int sectionIndex)
    {
        sectionIndex = -1;
        if (_selectedRowIndex < 0 || _selectedRowIndex >= rows.Count)
        {
            return false;
        }

        sectionIndex = rows[_selectedRowIndex].SectionIndex;
        return sectionIndex >= 0 && sectionIndex < _sections.Count;
    }

    private void EnsureSelectionVisible(int viewportHeight, int totalRows)
    {
        if (viewportHeight <= 0 || totalRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedRowIndex < 0)
        {
            _selectedRowIndex = 0;
        }

        if (_selectedRowIndex < _scrollOffset)
        {
            _scrollOffset = _selectedRowIndex;
        }
        else if (_selectedRowIndex >= _scrollOffset + viewportHeight)
        {
            _scrollOffset = _selectedRowIndex - viewportHeight + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, totalRows - viewportHeight));
    }

    private string RenderTitle()
    {
        var text = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? string.Concat(Title, " ", FocusMarker)
            : Title;
        return Apply(text, ResolveStyle(IsFocused ? FocusedTitleStyle : TitleStyle));
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return ResolveStyle(style);
    }

    private TeaStyle ResolveSelectedStyle()
    {
        var style = SelectedRowStyle;
        if (IsFocused)
        {
            style = style.Merge(FocusedSelectedRowStyle);
        }

        return style;
    }

    private TeaStyle ResolveStyle(TeaStyle style) => IsDisabled ? style.Merge(DisabledStyle) : style;

    private static InspectorSection Clone(InspectorSection section)
    {
        var clone = new InspectorSection(section.Title, section.IsExpanded);
        for (var index = 0; index < section.Fields.Count; index++)
        {
            var field = section.Fields[index];
            clone.Fields.Add(new InspectorField(field.Key, field.Value));
        }

        for (var index = 0; index < section.Details.Count; index++)
        {
            clone.Details.Add(section.Details[index] ?? string.Empty);
        }

        return clone;
    }

    private static string Pad(string text, int width) => text.Length >= width ? text[..width] : text.PadRight(width);
    private static string Apply(string text, TeaStyle style) => style.IsEmpty ? text : style.Render(text);
    private static void Write(Canvas canvas, int x, int y, string text, TeaStyle style, int width) => canvas.WriteText(x, y, Apply(text ?? string.Empty, style), width);

    private enum InspectorRowKind { SectionHeader, Field, Detail }
    private readonly record struct InspectorRow(int SectionIndex, InspectorRowKind Kind, int ItemIndex);
}
