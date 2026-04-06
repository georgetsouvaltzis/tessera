namespace Tessera.Controls;

/// <summary>
/// Provides details when a <see cref="KeyValueList"/> selection changes.
/// </summary>
public sealed class KeyValueListSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new selection-change payload.
    /// </summary>
    /// <param name="previousIndex">Selected index before the change.</param>
    /// <param name="currentIndex">Selected index after the change.</param>
    /// <param name="previousItem">Selected item before the change.</param>
    /// <param name="currentItem">Selected item after the change.</param>
    public KeyValueListSelectionChangedEventArgs(
        int previousIndex,
        int currentIndex,
        KeyValueListEntry? previousItem,
        KeyValueListEntry? currentItem)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousItem = previousItem;
        CurrentItem = currentItem;
    }

    /// <summary>
    /// Gets selected index before the change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets selected index after the change.
    /// Compatibility alias for <see cref="SelectedIndex" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the selected index after the change.
    /// Canonical property for selection access.
    /// </summary>
    public int SelectedIndex => CurrentIndex;

    /// <summary>
    /// Gets selected item before the change.
    /// </summary>
    public KeyValueListEntry? PreviousItem { get; }

    /// <summary>
    /// Gets selected item after the change.
    /// Compatibility alias for <see cref="SelectedItem" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public KeyValueListEntry? CurrentItem { get; }

    /// <summary>
    /// Gets the selected item after the change.
    /// Canonical property for selection access.
    /// </summary>
    public KeyValueListEntry? SelectedItem => CurrentItem;
}
