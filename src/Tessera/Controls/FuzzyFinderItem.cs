namespace Tessera.Controls;

/// <summary>
///     Represents one searchable entry in a <see cref="FuzzyFinder" />.
/// </summary>
/// <param name="Id">Stable identifier used by selection handlers.</param>
/// <param name="Label">Primary text displayed in the result list.</param>
/// <param name="Description">Optional secondary text used for display and matching.</param>
public sealed record FuzzyFinderItem(string Id, string Label, string Description = "");

/// <summary>
///     Provides details when the highlighted fuzzy-finder row changes.
/// </summary>
public sealed class FuzzyFinderSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new selection-changed payload.
    /// </summary>
    /// <param name="previousIndex">Previous selected result index.</param>
    /// <param name="selectedIndex">Current selected result index.</param>
    /// <param name="previousItem">Previously selected item.</param>
    /// <param name="selectedItem">Currently selected item.</param>
    public FuzzyFinderSelectionChangedEventArgs(int previousIndex, int selectedIndex, FuzzyFinderItem? previousItem,
        FuzzyFinderItem? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    ///     Gets the previously selected result index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    ///     Gets the current selected result index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    ///     Gets the previously selected item.
    /// </summary>
    public FuzzyFinderItem? PreviousItem { get; }

    /// <summary>
    ///     Gets the current selected item.
    /// </summary>
    public FuzzyFinderItem? SelectedItem { get; }
}

/// <summary>
///     Provides details when a fuzzy-finder item is selected by activation input.
/// </summary>
public sealed class FuzzyFinderItemSelectedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new item-selected payload.
    /// </summary>
    /// <param name="item">The selected item.</param>
    /// <param name="query">The active query text.</param>
    public FuzzyFinderItemSelectedEventArgs(FuzzyFinderItem item, string query)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Query = query;
    }

    /// <summary>
    ///     Gets the selected item.
    /// </summary>
    public FuzzyFinderItem Item { get; }

    /// <summary>
    ///     Gets the selected item identifier.
    /// </summary>
    public string ItemId => Item.Id;

    /// <summary>
    ///     Gets the query text active at selection time.
    /// </summary>
    public string Query { get; }
}
