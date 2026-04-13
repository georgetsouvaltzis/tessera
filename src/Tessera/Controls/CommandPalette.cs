using System.ComponentModel;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
///     Represents a searchable command launcher overlay.
/// </summary>
public sealed class CommandPalette : Control
{
    private readonly List<int> _allItemIndices = [];
    private readonly List<int> _filterSeedIndices = [];
    private readonly List<int> _filteredIndices = [];
    private readonly List<CommandPaletteRenderCache> _itemRenderCache = [];
    private readonly List<CommandPaletteItem> _items = [];
    private readonly TextInputModel _query = new();
    private long _consumedExecutionVersion;
    private long _executionVersion;
    private CommandPaletteGlyphSet _glyphs = CommandPaletteGlyphSet.Default;
    private int _hoveredFilteredIndex = -1;
    private string _lastFilter = string.Empty;
    private int _selectedFilteredIndex;

    /// <summary>
    ///     Gets or sets the overlay title.
    /// </summary>
    public string Title { get; set; } = "Command Palette";

    /// <summary>
    ///     Gets or sets the marker appended to the title when focused and <see cref="ShowFocusMarker" /> is enabled.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets a value indicating whether <see cref="FocusMarker" /> should be shown while focused.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see langword="false" /> to preserve existing command palette title rendering.
    /// </remarks>
    public bool ShowFocusMarker { get; set; }

    /// <summary>
    ///     Gets or sets style merged into the title when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into the title when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into query text when the placeholder is not visible.
    /// </summary>
    public TesseraStyle QueryTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into placeholder text.
    /// </summary>
    public TesseraStyle PlaceholderTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets base style applied to command rows.
    /// </summary>
    public TesseraStyle ItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into the selected command row.
    /// </summary>
    public TesseraStyle SelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into hovered command rows.
    /// </summary>
    public TesseraStyle HoveredItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into muted rows.
    /// </summary>
    public TesseraStyle MutedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged when the control is disabled.
    /// </summary>
    public TesseraStyle DisabledItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets glyphs used for query prompt and row markers.
    /// </summary>
    public CommandPaletteGlyphSet Glyphs
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
            RefreshFilter();
        }
    }

    /// <summary>
    ///     Gets or sets the frame border style for the overlay panel.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.Rounded;

    /// <summary>
    ///     Gets or sets inner padding applied inside the overlay frame.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets a value indicating whether the palette is currently visible.
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    ///     Gets or sets the maximum number of visible command rows.
    /// </summary>
    public int MaxVisibleItems
    {
        get;
        set;
    } = 8;

    /// <summary>
    ///     Gets or sets the current query text.
    /// </summary>
    public string QueryText
    {
        get => _query.Value;
        set => SetQueryText(value);
    }

    /// <summary>
    ///     Gets the last executed command id.
    /// </summary>
    public string? LastExecutedItemId { get; private set; }

    /// <summary>
    ///     Gets the configured command entries.
    /// </summary>
    public IReadOnlyList<CommandPaletteItem> Items => _items;

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    ///     Occurs when a command is executed from the current filtered selection.
    /// </summary>
    public event EventHandler<CommandPaletteItemExecutedEventArgs>? ItemExecuted;

    /// <summary>
    ///     Replaces the palette command entries.
    /// </summary>
    /// <param name="items">The command entries to expose.</param>
    public void SetItems(IEnumerable<CommandPaletteItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        foreach (var item in items)
        {

            _items.Add(item);
        }

        RebuildItemRenderCache();
        RefreshFilter();
    }

    /// <summary>
    ///     Clears the current query text.
    /// </summary>
    public void ClearQuery()
    {
        SetQueryText(string.Empty);
    }

    /// <summary>
    ///     Opens the palette and requests focus.
    /// </summary>
    public void Open()
    {
        RequestFocus();
        if (IsVisible)
        {
            return;
        }

        IsVisible = true;
        ClearQuery();
    }

    /// <summary>
    ///     Closes the palette.
    /// </summary>
    public void Close()
    {
        IsVisible = false;
    }

    /// <summary>
    ///     Replaces the current query text.
    /// </summary>
    /// <param name="query">The query value.</param>
    public void SetQueryText(string query)
    {
        _query.SetValue(query);
        RefreshFilter();
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused)
        {
            return false;
        }

        if (!IsVisible)
        {
            return message switch
            {
                KeyPressed key when key.IsCharacter('p', ModifierKeys.Ctrl) => OpenFromShortcut(),
                _ => false
            };
        }

        if (message is KeyPressed input)
        {
            if (input.Is(Key.Escape))
            {
                Close();
                return true;
            }

            if (input.Is(Key.Down) && _filteredIndices.Count > 0)
            {
                MoveNext();
                return true;
            }

            if (input.Is(Key.Up) && _filteredIndices.Count > 0)
            {
                MovePrevious();
                return true;
            }

            if (input.Is(Key.Enter))
            {
                return ExecuteSelected();
            }
        }

        var inputResult = _query.Update(message);
        if (!inputResult.Changed)
        {
            return inputResult.Submitted && ExecuteSelected();
        }

        RefreshFilter();
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (!IsVisible || message is not PointerInput pointer ||
            !TryResolveModal(bounds, out var modal, out var content))
        {
            return Handle(message);
        }

        var insideModal = modal.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!insideModal)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredFilteredIndex(-1);
            }

            if (pointer is not { Kind: PointerEventKind.Press, Button: PointerButton.Left })
            {
                return changed;
            }

            Close();
            return true;
        }

        if (pointer.Kind == PointerEventKind.Wheel && _filteredIndices.Count > 0)
        {
            return HandleWheel(pointer.Button, changed);
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredFilteredIndex(-1);
            }

            return changed;
        }

        var hovered = RowToFilteredIndex(content, pointer.Y);
        return pointer.Kind switch
        {
            PointerEventKind.Motion => SetHoveredFilteredIndex(hovered),
            PointerEventKind.Press when pointer is { Button: PointerButton.Left } && hovered >= 0
                => HandlePress(hovered, changed),
            _ => changed
        };
    }

    /// <summary>
    ///     Executes try consume execution.
    /// </summary>
    /// <param name="itemId">The item id value.</param>
    /// <returns><see langword="true" /> when try consume execution succeeds.</returns>
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

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (!TryResolveModal(clipped, out var modal, out var content))
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitleText();
        content = FrameLayout.DrawFrameAndResolveContent(canvas, modal, title, Border, Padding,
            ResolveBorderStyleText());

        var queryPrompt = ResolveQueryPrompt();
        var queryWidth = Math.Max(1, content.Width - ControlTextLayout.MeasureDisplayWidth(queryPrompt));
        var frame = _query.BuildFrame(queryWidth);
        var queryStyle = frame.PlaceholderVisible ? PlaceholderTextStyle : QueryTextStyle;
        if (IsDisabled)
        {
            queryStyle = queryStyle.Merge(DisabledItemStyle).Merge(MutedItemStyle);
        }

        canvas.WriteText(content.X, content.Y, ApplyStyle(string.Concat(queryPrompt, frame.Text), queryStyle),
            content.Width);
        if (content.Height <= 1)
        {
            return;
        }

        if (_filteredIndices.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ApplyStyle("(no commands)", MutedItemStyle), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filteredIndices.Count);
        var end = Math.Min(_filteredIndices.Count, start + visibleRows);
        var row = 0;
        for (var filteredIndex = start; filteredIndex < end; filteredIndex++, row++)
        {
            var itemIndex = _filteredIndices[filteredIndex];
            var rowText = ResolveRowText(_itemRenderCache[itemIndex], filteredIndex);
            canvas.WriteText(content.X, content.Y + 1 + row, ApplyStyle(rowText, ResolveItemStyle(filteredIndex)),
                content.Width);
        }
    }

    private void RefreshFilter()
    {
        var filter = _query.Value.Trim();
        if (string.Equals(filter, _lastFilter, StringComparison.Ordinal))
        {
            ClampSelectionAndHover();
            return;
        }

        var canNarrow = filter.Length > 0
                        && _lastFilter.Length > 0
                        && filter.StartsWith(_lastFilter, StringComparison.OrdinalIgnoreCase);
        if (canNarrow)
        {
            _filterSeedIndices.Clear();
            _filterSeedIndices.AddRange(_filteredIndices);
        }

        _filteredIndices.Clear();
        var source = canNarrow ? _filterSeedIndices : _allItemIndices;
        if (filter.Length == 0)
        {
            _filteredIndices.AddRange(_allItemIndices);
            _lastFilter = string.Empty;
            ClampSelectionAndHover();
            return;
        }

        _filteredIndices.AddRange(source.Where(itemIndex =>
            _itemRenderCache[itemIndex].SearchText.Contains(filter, StringComparison.OrdinalIgnoreCase)));

        _lastFilter = filter;
        ClampSelectionAndHover();
    }

    private void ClampSelectionAndHover()
    {
        if (_filteredIndices.Count == 0)
        {
            _selectedFilteredIndex = 0;
            _hoveredFilteredIndex = -1;
            return;
        }

        _selectedFilteredIndex = Math.Clamp(_selectedFilteredIndex, 0, _filteredIndices.Count - 1);
        if (_hoveredFilteredIndex >= _filteredIndices.Count)
        {
            _hoveredFilteredIndex = _filteredIndices.Count - 1;
        }
    }

    private void MoveNext()
    {
        if (_filteredIndices.Count > 0)
        {
            _selectedFilteredIndex = (_selectedFilteredIndex + 1) % _filteredIndices.Count;
        }
    }

    private void MovePrevious()
    {
        if (_filteredIndices.Count > 0)
        {
            _selectedFilteredIndex = (_selectedFilteredIndex + _filteredIndices.Count - 1) % _filteredIndices.Count;
        }
    }

    private bool SetHoveredFilteredIndex(int index)
    {
        if (_hoveredFilteredIndex == index)
        {
            return false;
        }

        _hoveredFilteredIndex = index;
        return true;
    }

    private bool SetSelectedFilteredIndex(int index)
    {
        if (_selectedFilteredIndex == index)
        {
            return false;
        }

        _selectedFilteredIndex = index;
        return true;
    }

    private void RebuildItemRenderCache()
    {
        _itemRenderCache.Clear();
        _allItemIndices.Clear();
        _filterSeedIndices.Clear();
        _filteredIndices.Clear();
        for (var index = 0; index < _items.Count; index++)
        {
            var item = _items[index];
            _itemRenderCache.Add(CommandPaletteRenderCache.Create(item, _glyphs));
            _allItemIndices.Add(index);
        }

        _filteredIndices.AddRange(_allItemIndices);
        _lastFilter = string.Empty;
    }

    private string ResolveRowText(CommandPaletteRenderCache row, int filteredIndex)
    {
        if (filteredIndex == _selectedFilteredIndex)
        {
            return row.SelectedRowText;
        }

        return filteredIndex == _hoveredFilteredIndex ? row.HoveredRowText : row.NormalRowText;
    }

    private bool ExecuteSelected()
    {
        if (_filteredIndices.Count == 0)
        {
            Close();
            return true;
        }

        var item = _items[_filteredIndices[Math.Clamp(_selectedFilteredIndex, 0, _filteredIndices.Count - 1)]];
        LastExecutedItemId = item.Id;
        _executionVersion++;
        ItemExecuted?.Invoke(this, new CommandPaletteItemExecutedEventArgs(item));
        Close();
        return true;
    }

    private static int ComputeWindowStart(int highlightedIndex, int rows, int count)
    {
        if (count <= rows)
        {
            return 0;
        }

        var half = rows / 2;
        var start = highlightedIndex - half;
        if (start < 0)
        {
            return 0;
        }

        var maxStart = count - rows;
        return start > maxStart ? maxStart : start;
    }

    private int RowToFilteredIndex(Rect content, int y)
    {
        if (content.Height <= 1 || _filteredIndices.Count == 0)
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var row = y - (content.Y + 1);
        if (row < 0 || row >= visibleRows)
        {
            return -1;
        }

        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filteredIndices.Count);
        var filteredIndex = start + row;
        return filteredIndex >= 0 && filteredIndex < _filteredIndices.Count
            ? filteredIndex
            : -1;
    }

    private bool TryResolveModal(Rect bounds, out Rect modal, out Rect content)
    {
        modal = default;
        content = default;
        if (bounds.IsEmpty || bounds.Width < 24 || bounds.Height < 6)
        {
            return false;
        }

        var modalWidth = Math.Min(bounds.Width - 2, Math.Max(24, bounds.Width * 2 / 3));
        var modalHeight = Math.Min(bounds.Height - 2, Math.Max(8, bounds.Height * 2 / 3));
        var modalX = bounds.X + (bounds.Width - modalWidth) / 2;
        var modalY = bounds.Y + (bounds.Height - modalHeight) / 2;
        modal = new Rect(modalX, modalY, modalWidth, modalHeight);
        content = FrameLayout.ResolveContentRect(modal, Border, Padding);
        return !content.IsEmpty;
    }

    private string ResolveQueryPrompt()
    {
        return string.IsNullOrEmpty(_glyphs.QueryPrompt)
            ? string.Empty
            : string.Concat(_glyphs.QueryPrompt, _glyphs.MarkerSeparator);
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

    private TesseraStyle ResolveBorderStyleText()
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

    private TesseraStyle ResolveItemStyle(int filteredIndex)
    {
        var style = ItemStyle;
        if (filteredIndex == _selectedFilteredIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (filteredIndex == _hoveredFilteredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle).Merge(MutedItemStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private readonly record struct CommandPaletteRenderCache(
        string SearchText,
        string NormalRowText,
        string SelectedRowText,
        string HoveredRowText)
    {
        public static CommandPaletteRenderCache Create(CommandPaletteItem item, CommandPaletteGlyphSet glyphs)
        {
            var summary = string.IsNullOrWhiteSpace(item.Description)
                ? item.Title
                : string.Concat(item.Title, " - ", item.Description);
            var search = string.Concat(item.Title, "\n", item.Description, "\n", item.Id);
            return new CommandPaletteRenderCache(
                search,
                string.Concat(glyphs.NormalRowMarker, glyphs.MarkerSeparator, summary),
                string.Concat(glyphs.SelectedRowMarker, glyphs.MarkerSeparator, summary),
                string.Concat(glyphs.HoveredRowMarker, glyphs.MarkerSeparator, summary));
        }
    }

    private bool HandleWheel(PointerButton button, bool changed)
    {
        return button switch
        {
            PointerButton.WheelDown => MoveNextAndHandle(),
            PointerButton.WheelUp => MovePreviousAndHandle(),
            _ => changed
        };
    }

    private bool HandlePress(int hovered, bool changed)
    {
        changed |= SetHoveredFilteredIndex(hovered);
        changed |= SetSelectedFilteredIndex(hovered);
        changed |= ExecuteSelected();
        return changed;
    }

    private bool OpenFromShortcut()
    {
        Open();
        return true;
    }

    private bool MoveNextAndHandle()
    {
        MoveNext();
        return true;
    }

    private bool MovePreviousAndHandle()
    {
        MovePrevious();
        return true;
    }
}
