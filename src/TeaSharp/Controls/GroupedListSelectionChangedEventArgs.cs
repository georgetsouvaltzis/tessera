namespace TeaSharp.Controls;

/// <summary>
/// Provides details when <see cref="GroupedListView{TGroup,TItem}" /> selection changes.
/// </summary>
/// <typeparam name="TGroup">The group-key type.</typeparam>
/// <typeparam name="TItem">The item type.</typeparam>
public sealed class GroupedListSelectionChangedEventArgs<TGroup, TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new selection payload.
    /// </summary>
    /// <param name="previousRowIndex">Previous visible-row index.</param>
    /// <param name="currentRowIndex">Current visible-row index.</param>
    /// <param name="previousGroupIndex">Previous group index, when any.</param>
    /// <param name="currentGroupIndex">Current group index, when any.</param>
    /// <param name="previousItemIndex">Previous item index within its group, when any.</param>
    /// <param name="currentItemIndex">Current item index within its group, when any.</param>
    /// <param name="previousItem">Previous selected item, when any.</param>
    /// <param name="currentItem">Current selected item, when any.</param>
    public GroupedListSelectionChangedEventArgs(
        int previousRowIndex,
        int currentRowIndex,
        int? previousGroupIndex,
        int? currentGroupIndex,
        int? previousItemIndex,
        int? currentItemIndex,
        TItem? previousItem,
        TItem? currentItem)
    {
        PreviousRowIndex = previousRowIndex;
        CurrentRowIndex = currentRowIndex;
        PreviousGroupIndex = previousGroupIndex;
        CurrentGroupIndex = currentGroupIndex;
        PreviousItemIndex = previousItemIndex;
        CurrentItemIndex = currentItemIndex;
        PreviousItem = previousItem;
        CurrentItem = currentItem;
    }

    /// <summary>
    /// Gets previous visible-row index.
    /// </summary>
    public int PreviousRowIndex { get; }

    /// <summary>
    /// Gets selected visible-row index.
    /// Compatibility alias for <see cref="SelectedRowIndex" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public int CurrentRowIndex { get; }

    /// <summary>
    /// Gets selected visible-row index.
    /// Canonical property for selection access.
    /// </summary>
    public int SelectedRowIndex => CurrentRowIndex;

    /// <summary>
    /// Gets previous selected group index, when any.
    /// </summary>
    public int? PreviousGroupIndex { get; }

    /// <summary>
    /// Gets selected group index, when any.
    /// Compatibility alias for <see cref="SelectedGroupIndex" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public int? CurrentGroupIndex { get; }

    /// <summary>
    /// Gets selected group index, when any.
    /// Canonical property for selection access.
    /// </summary>
    public int? SelectedGroupIndex => CurrentGroupIndex;

    /// <summary>
    /// Gets previous selected item index within its group, when any.
    /// </summary>
    public int? PreviousItemIndex { get; }

    /// <summary>
    /// Gets selected item index within its group, when any.
    /// Compatibility alias for <see cref="SelectedItemIndex" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public int? CurrentItemIndex { get; }

    /// <summary>
    /// Gets selected item index within its group, when any.
    /// Canonical property for selection access.
    /// </summary>
    public int? SelectedItemIndex => CurrentItemIndex;

    /// <summary>
    /// Gets previous selected item, when any.
    /// </summary>
    public TItem? PreviousItem { get; }

    /// <summary>
    /// Gets selected item, when any.
    /// Compatibility alias for <see cref="SelectedItem" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public TItem? CurrentItem { get; }

    /// <summary>
    /// Gets selected item, when any.
    /// Canonical property for selection access.
    /// </summary>
    public TItem? SelectedItem => CurrentItem;
}
