using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a searchable overlay for quick item navigation and submission.
/// </summary>
public sealed partial class QuickOpenOverlay : Control
{
    private readonly List<QuickOpenItem> _items = [];
    private readonly List<int> _filteredIndices = [];
    private QuickOpenOverlayGlyphSet _glyphs = QuickOpenOverlayGlyphSet.Default;
    private int _selectedFilteredIndex;
    private int _hoveredFilteredIndex = -1;
    private string _query = string.Empty;

    /// <summary>
    /// Occurs when the selected item is submitted.
    /// </summary>
    public event EventHandler<QuickOpenOverlaySubmittedEventArgs>? Submitted;

    /// <summary>
    /// Occurs when the overlay is dismissed without submission.
    /// </summary>
    public event EventHandler? Cancelled;

    /// <summary>
    /// Gets or sets overlay title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Quick Open";

    /// <summary>
    /// Gets or sets marker appended to title while focused and <see cref="ShowFocusMarker" /> is enabled.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets fallback text rendered when no items are configured.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(empty)";

    /// <summary>
    /// Gets or sets fallback text rendered when query filtering has no matches.
    /// </summary>
    public string NoMatchesText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no matches)";

    /// <summary>
    /// Gets or sets query placeholder text.
    /// </summary>
    public string Placeholder
    {
        get;
        set => field = value ?? string.Empty;
    } = "Type to search...";

    /// <summary>
    /// Gets or sets title style while not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets query style when query text is present.
    /// </summary>
    public TesseraStyle QueryTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets query style when placeholder is visible.
    /// </summary>
    public TesseraStyle PlaceholderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets base row style.
    /// </summary>
    public TesseraStyle ItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets selected row style.
    /// </summary>
    public TesseraStyle SelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets hovered row style.
    /// </summary>
    public TesseraStyle HoveredItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets match-marker style rendered when query is non-empty.
    /// </summary>
    public TesseraStyle MatchMarkerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while disabled.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border glyph style while not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border glyph style while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets glyphs used by query and row rendering.
    /// </summary>
    public QuickOpenOverlayGlyphSet Glyphs
    {
        get => _glyphs;
        set => _glyphs = value;
    }

    /// <summary>
    /// Gets or sets overlay border style.
    /// </summary>
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Rounded;

    /// <summary>
    /// Gets or sets inner padding applied inside the frame.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets maximum visible rows.
    /// </summary>
    public int MaxVisibleItems { get; set; } = 9;

    /// <summary>
    /// Gets a value indicating whether the overlay is open.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Gets the current query text.
    /// </summary>
    public string Query => _query;

    /// <summary>
    /// Gets configured items.
    /// </summary>
    public IReadOnlyList<QuickOpenItem> Items => _items;

    /// <summary>
    /// Gets selected index within current filtered rows.
    /// </summary>
    public int SelectedIndex => _filteredIndices.Count == 0 ? -1 : _selectedFilteredIndex;

    /// <summary>
    /// Gets the selected item.
    /// </summary>
    public QuickOpenItem? SelectedItem => _filteredIndices.Count == 0
        ? null
        : _items[_filteredIndices[_selectedFilteredIndex]];

    /// <summary>
    /// Gets currently visible filtered row count.
    /// </summary>
    public int FilteredCount => _filteredIndices.Count;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces overlay items.
    /// </summary>
    /// <param name="items">Items to show.</param>
    public void SetItems(IEnumerable<QuickOpenItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        foreach (var item in items)
        {
            if (item is not null)
            {
                _items.Add(item);
            }
        }

        RefreshFilter();
    }

    /// <summary>
    /// Sets query text and refreshes filtering.
    /// </summary>
    /// <param name="query">Query text.</param>
    public void SetQuery(string query)
    {
        _query = query ?? string.Empty;
        RefreshFilter();
    }

    /// <summary>
    /// Sets selected filtered row index.
    /// </summary>
    /// <param name="index">Target index.</param>
    /// <returns><see langword="true" /> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_filteredIndices.Count == 0)
        {
            _selectedFilteredIndex = 0;
            return false;
        }

        var clamped = Math.Clamp(index, 0, _filteredIndices.Count - 1);
        if (_selectedFilteredIndex == clamped)
        {
            return false;
        }

        _selectedFilteredIndex = clamped;
        return true;
    }

    /// <summary>
    /// Opens the overlay.
    /// </summary>
    public void Open()
    {
        RequestFocus();
        IsOpen = true;
        _hoveredFilteredIndex = -1;
    }

    /// <summary>
    /// Closes the overlay.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
        _hoveredFilteredIndex = -1;
    }

    private void RefreshFilter()
    {
        _filteredIndices.Clear();
        if (_query.Length == 0)
        {
            for (var index = 0; index < _items.Count; index++)
            {
                _filteredIndices.Add(index);
            }
        }
        else
        {
            for (var index = 0; index < _items.Count; index++)
            {
                var item = _items[index];
                if (item.Label.Contains(_query, StringComparison.OrdinalIgnoreCase)
                    || item.Description.Contains(_query, StringComparison.OrdinalIgnoreCase)
                    || item.Id.Contains(_query, StringComparison.OrdinalIgnoreCase))
                {
                    _filteredIndices.Add(index);
                }
            }
        }

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

    private bool TryResolveOverlay(Rect bounds, out Rect overlay, out Rect content)
    {
        overlay = default;
        content = default;
        if (bounds.IsEmpty || bounds.Width < 28 || bounds.Height < 6)
        {
            return false;
        }

        var width = Math.Min(bounds.Width - 2, Math.Max(28, bounds.Width * 2 / 3));
        var height = Math.Min(bounds.Height - 2, Math.Max(8, bounds.Height * 2 / 3));
        var x = bounds.X + (bounds.Width - width) / 2;
        var y = bounds.Y + (bounds.Height - height) / 2;
        overlay = new Rect(x, y, width, height);
        content = FrameLayout.ResolveContentRect(overlay, BorderStyle, Padding);
        return !content.IsEmpty;
    }

    private static int ComputeWindowStart(int selectedIndex, int rows, int count)
    {
        if (count <= rows)
        {
            return 0;
        }

        var half = rows / 2;
        var start = selectedIndex - half;
        if (start < 0)
        {
            return 0;
        }

        var maxStart = count - rows;
        return start > maxStart ? maxStart : start;
    }

    private static string BuildSummary(QuickOpenItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Description))
        {
            return item.Label;
        }

        return string.Concat(item.Label, " - ", item.Description);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
