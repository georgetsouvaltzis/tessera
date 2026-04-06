using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a selectable dashboard tile grid.
/// </summary>
public sealed partial class DashboardGrid : Control
{
    private readonly List<DashboardTile> _tiles = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Occurs when selected tile changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<DashboardTile>>? SelectionChanged;

    /// <summary>
    /// Gets or sets control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Dashboard Grid";

    /// <summary>
    /// Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether the focus marker should be rendered when focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    public TesseraStyle TitleStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    /// <remarks>
    /// Canonical alias for cross-control title style naming consistency.
    /// </remarks>
    public TesseraStyle TitleStyle
    {
        get => TitleStyleText;
        set => TitleStyleText = value;
    }

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    /// <remarks>
    /// Canonical alias for cross-control title style naming consistency.
    /// </remarks>
    public TesseraStyle FocusedTitleStyle
    {
        get => FocusedTitleStyleText;
        set => FocusedTitleStyleText = value;
    }

    /// <summary>
    /// Gets or sets frame border style when not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets frame border style when focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets base tile style.
    /// </summary>
    public TesseraStyle TileStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected tile text.
    /// </summary>
    public TesseraStyle SelectedTileStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered tile text.
    /// </summary>
    public TesseraStyle HoveredTileStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when the control is disabled.
    /// </summary>
    public TesseraStyle DisabledTileStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets outer border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets tile border style.
    /// </summary>
    public BorderStyle TileBorder { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets configured tiles.
    /// </summary>
    public IReadOnlyList<DashboardTile> Tiles => _tiles;

    /// <summary>
    /// Gets selected tile id or <see langword="null" /> when there is no tile.
    /// </summary>
    public string? SelectedTileId => SelectedItem?.Id;

    /// <summary>
    /// Gets selected tile index or <c>-1</c> when there is no tile.
    /// </summary>
    public int SelectedIndex => _tiles.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets selected tile or <see langword="null" /> when there is no tile.
    /// </summary>
    public DashboardTile? SelectedItem => _tiles.Count == 0 ? null : _tiles[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces dashboard tiles.
    /// </summary>
    /// <param name="tiles">Tile definitions.</param>
    public void SetTiles(IEnumerable<DashboardTile> tiles)
    {
        ArgumentNullException.ThrowIfNull(tiles);
        var previousIndex = SelectedIndex;
        var previousTile = SelectedItem;
        var previousSelectedId = previousTile?.Id;

        _tiles.Clear();
        foreach (var tile in tiles)
        {
            if (tile is not null)
            {
                _tiles.Add(Clone(tile));
            }
        }

        SortTiles();
        NormalizeSelection(previousSelectedId);
        RaiseSelectionChangedIfNeeded(previousIndex, previousTile);
    }

    /// <summary>
    /// Selects a tile by index.
    /// </summary>
    /// <param name="index">Requested selected tile index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool Select(int index) => SetSelectedIndex(index);

    /// <summary>
    /// Sets selected tile by index with range clamping.
    /// </summary>
    /// <param name="index">Requested selected tile index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_tiles.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _tiles.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousTile = _tiles[previousIndex];
        _selectedIndex = clamped;
        RaiseSelectionChanged(previousIndex, previousTile);
        return true;
    }

    /// <summary>
    /// Moves an existing tile.
    /// </summary>
    /// <param name="tileId">Tile identifier.</param>
    /// <param name="column">Target column.</param>
    /// <param name="row">Target row.</param>
    /// <returns><see langword="true" /> when tile location changed; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="tileId" /> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="column" /> or <paramref name="row" /> is negative.</exception>
    public bool MoveTile(string tileId, int column, int row)
    {
        if (string.IsNullOrWhiteSpace(tileId))
        {
            throw new ArgumentException("Tile id must be non-empty.", nameof(tileId));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfNegative(row);

        var index = FindIndexById(tileId);
        if (index < 0)
        {
            return false;
        }

        var tile = _tiles[index];
        if (tile.Column == column && tile.Row == row)
        {
            return false;
        }

        var selectedId = SelectedTileId;
        _tiles[index] = tile with { Column = column, Row = row };
        SortTiles();
        NormalizeSelection(selectedId);
        return true;
    }

    /// <summary>
    /// Resizes an existing tile.
    /// </summary>
    /// <param name="tileId">Tile identifier.</param>
    /// <param name="columnSpan">New column span. Must be greater than zero.</param>
    /// <param name="rowSpan">New row span. Must be greater than zero.</param>
    /// <returns><see langword="true" /> when tile size changed; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="tileId" /> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="columnSpan" /> or <paramref name="rowSpan" /> is less than one.
    /// </exception>
    public bool ResizeTile(string tileId, int columnSpan, int rowSpan)
    {
        if (string.IsNullOrWhiteSpace(tileId))
        {
            throw new ArgumentException("Tile id must be non-empty.", nameof(tileId));
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(columnSpan, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowSpan, 1);

        var index = FindIndexById(tileId);
        if (index < 0)
        {
            return false;
        }

        var tile = _tiles[index];
        if (tile.ColumnSpan == columnSpan && tile.RowSpan == rowSpan)
        {
            return false;
        }

        var selectedId = SelectedTileId;
        _tiles[index] = tile with { ColumnSpan = columnSpan, RowSpan = rowSpan };
        SortTiles();
        NormalizeSelection(selectedId);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _tiles.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.Is(Key.Up))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Right) || key.Is(Key.Down))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_tiles.Count - 1);
        }

        if (key.Is(Key.PageUp))
        {
            return SetSelectedIndex(_selectedIndex - 4);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedIndex(_selectedIndex + 4);
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
        if (content.IsEmpty || _tiles.Count == 0)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            var hovered = HitTest(content, pointer.X, pointer.Y);
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            var hitIndex = HitTest(content, pointer.X, pointer.Y);
            var changed = SetHoveredIndex(hitIndex);
            if (hitIndex >= 0)
            {
                changed |= SetSelectedIndex(hitIndex);
            }

            return changed || hitIndex >= 0;
        }

        return Handle(message);
    }

    private void NormalizeSelection(string? preferredSelectedId)
    {
        if (_tiles.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            return;
        }

        var selectedId = preferredSelectedId;
        if (string.IsNullOrWhiteSpace(selectedId)
            && _selectedIndex >= 0
            && _selectedIndex < _tiles.Count)
        {
            selectedId = _tiles[_selectedIndex].Id;
        }

        var resolvedIndex = string.IsNullOrWhiteSpace(selectedId)
            ? -1
            : FindIndexById(selectedId);
        _selectedIndex = resolvedIndex >= 0 ? resolvedIndex : 0;
        if (_hoveredIndex >= _tiles.Count)
        {
            _hoveredIndex = -1;
        }
    }

    private bool SetHoveredIndex(int hoveredIndex)
    {
        if (_hoveredIndex == hoveredIndex)
        {
            return false;
        }

        _hoveredIndex = hoveredIndex;
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, DashboardTile? previousItem)
    {
        var selected = SelectedItem;
        if (previousIndex == SelectedIndex
            && string.Equals(previousItem?.Id, selected?.Id, StringComparison.Ordinal))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousItem);
    }

    private void RaiseSelectionChanged(int previousIndex, DashboardTile? previousItem)
    {
        SelectionChanged?.Invoke(
            this,
            new ListSelectionChangedEventArgs<DashboardTile>(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }

    private int FindIndexById(string tileId)
    {
        for (var index = 0; index < _tiles.Count; index++)
        {
            if (string.Equals(_tiles[index].Id, tileId, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }

    private void SortTiles()
    {
        _tiles.Sort(static (left, right) =>
        {
            var rowCompare = left.Row.CompareTo(right.Row);
            if (rowCompare != 0)
            {
                return rowCompare;
            }

            var columnCompare = left.Column.CompareTo(right.Column);
            if (columnCompare != 0)
            {
                return columnCompare;
            }

            return string.CompareOrdinal(left.Id, right.Id);
        });
    }

    private static DashboardTile Clone(DashboardTile tile)
    {
        return tile with { };
    }

}
