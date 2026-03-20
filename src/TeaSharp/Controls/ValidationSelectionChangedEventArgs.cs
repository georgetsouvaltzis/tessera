namespace TeaSharp.Controls;

/// <summary>
/// Provides details when a <see cref="ValidationSummary" /> selection changes.
/// </summary>
public sealed class ValidationSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new selection payload.
    /// </summary>
    /// <param name="previousIndex">The selected index before the change.</param>
    /// <param name="currentIndex">The selected index after the change.</param>
    /// <param name="previousItem">The selected issue before the change.</param>
    /// <param name="currentItem">The selected issue after the change.</param>
    public ValidationSelectionChangedEventArgs(
        int previousIndex,
        int currentIndex,
        ValidationIssue? previousItem,
        ValidationIssue? currentItem)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousItem = previousItem;
        CurrentItem = currentItem;
    }

    /// <summary>
    /// Gets the selected index before the change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the selected index after the change.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the selected issue before the change.
    /// </summary>
    public ValidationIssue? PreviousItem { get; }

    /// <summary>
    /// Gets the selected issue after the change.
    /// </summary>
    public ValidationIssue? CurrentItem { get; }
}
