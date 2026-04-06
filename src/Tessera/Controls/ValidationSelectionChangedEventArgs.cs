namespace Tessera.Controls;

/// <summary>
/// Provides data for validation selection change events.
/// </summary>
public sealed class ValidationSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationSelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="previousIndex">The previous selected index.</param>
    /// <param name="currentIndex">The current selected index.</param>
    /// <param name="previousIssue">The previously selected issue.</param>
    /// <param name="currentIssue">The currently selected issue.</param>
    public ValidationSelectionChangedEventArgs(
        int previousIndex,
        int currentIndex,
        ValidationIssue? previousIssue,
        ValidationIssue? currentIssue)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousIssue = previousIssue;
        CurrentIssue = currentIssue;
    }

    /// <summary>
    /// Gets the previous selected index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the selected index after the change.
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
    /// Gets the previously selected issue.
    /// </summary>
    public ValidationIssue? PreviousIssue { get; }

    /// <summary>
    /// Gets the selected issue after the change.
    /// Compatibility alias for <see cref="SelectedIssue" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public ValidationIssue? CurrentIssue { get; }

    /// <summary>
    /// Gets the selected issue after the change.
    /// Canonical property for selection access.
    /// </summary>
    public ValidationIssue? SelectedIssue => CurrentIssue;
}
