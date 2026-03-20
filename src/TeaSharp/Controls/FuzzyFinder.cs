using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact fuzzy picker with query input and ranked results.
/// </summary>
public sealed partial class FuzzyFinder : Control
{
    private readonly List<FuzzyFinderItem> _items = [];
    private readonly List<ResultRow> _results = [];
    private readonly TextInputModel _query = new();
    private int _selectedIndex;
    private int _scrollOffset;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Initializes a new fuzzy finder.
    /// </summary>
    public FuzzyFinder()
    {
        _query.Placeholder = "Type to filter...";
    }

    /// <summary>
    /// Occurs when the highlighted result changes.
    /// </summary>
    public event EventHandler<FuzzyFinderSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs when a result is selected via activation input.
    /// </summary>
    public event EventHandler<FuzzyFinderItemSelectedEventArgs>? ItemSelected;

    /// <summary>
    /// Gets or sets the optional title shown in the border.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Fuzzy Finder";

    /// <summary>
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether the title focus marker should be shown.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets query text placeholder.
    /// </summary>
    public string Placeholder
    {
        get => _query.Placeholder;
        set => _query.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets query text.
    /// </summary>
    public string QueryText
    {
        get => _query.Value;
        set => SetQuery(value);
    }

    /// <summary>
    /// Gets or sets whether results should be visible.
    /// </summary>
    public bool IsOpen
    {
        get;
        private set;
    } = true;

    /// <summary>
    /// Gets or sets max visible result rows.
    /// </summary>
    public int MaxVisibleResults
    {
        get;
        set;
    } = 8;

    /// <summary>
    /// Gets configured source items.
    /// </summary>
    public IReadOnlyList<FuzzyFinderItem> Items => _items;

    /// <summary>
    /// Gets current result count.
    /// </summary>
    public int ResultCount => _results.Count;

    /// <summary>
    /// Gets selected result index.
    /// Returns <c>-1</c> when no results are available.
    /// </summary>
    public int SelectedIndex => _results.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets currently selected item.
    /// </summary>
    public FuzzyFinderItem? SelectedItem => _results.Count == 0
        ? null
        : _items[_results[_selectedIndex].ItemIndex];

    /// <summary>
    /// Gets the last selected item identifier.
    /// </summary>
    public string? LastSelectedItemId { get; private set; }

    /// <summary>
    /// Gets or sets style for query text.
    /// </summary>
    public TeaStyle ValueTextStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for query placeholder.
    /// </summary>
    public TeaStyle PlaceholderTextStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for non-selected result rows.
    /// </summary>
    public TeaStyle ListItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered result rows.
    /// </summary>
    public TeaStyle HoveredItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected result rows.
    /// </summary>
    public TeaStyle SelectedItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for matched characters in result labels.
    /// </summary>
    public TeaStyle MatchHighlightStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces the item source.
    /// </summary>
    /// <param name="items">Items to index for fuzzy filtering.</param>
    public void SetItems(IEnumerable<FuzzyFinderItem> items)
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

        RefreshResults();
    }

    /// <summary>
    /// Replaces the item source from plain labels.
    /// </summary>
    /// <param name="items">Labels to index for fuzzy filtering.</param>
    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        SetItems(items.Select(static value => new FuzzyFinderItem(value ?? string.Empty, value ?? string.Empty)));
    }

    /// <summary>
    /// Sets query text and refreshes results.
    /// </summary>
    /// <param name="query">The query text.</param>
    public void SetQuery(string query)
    {
        var normalized = query ?? string.Empty;
        if (string.Equals(_query.Value, normalized, StringComparison.Ordinal))
        {
            return;
        }

        _query.SetValue(normalized);
        IsOpen = true;
        RefreshResults();
    }

    /// <summary>
    /// Clears query text.
    /// </summary>
    public void ClearQuery()
    {
        SetQuery(string.Empty);
    }

    /// <summary>
    /// Opens the result list.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
        RequestFocus();
    }

    /// <summary>
    /// Closes the result list.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 32 + Padding.Horizontal;
        var height = 8 + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(title, style);
    }
}
