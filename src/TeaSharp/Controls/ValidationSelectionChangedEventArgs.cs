namespace TeaSharp.Controls;

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
    /// Gets the current selected index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the selected index after the change.
    /// Canonical naming alias that forwards to <see cref="CurrentIndex" /> for compatibility.
    /// </summary>
    public int SelectedIndex => CurrentIndex;

    /// <summary>
    /// Gets the previously selected issue.
    /// </summary>
    public ValidationIssue? PreviousIssue { get; }

    /// <summary>
    /// Gets the currently selected issue.
    /// </summary>
    public ValidationIssue? CurrentIssue { get; }

    /// <summary>
    /// Gets the selected issue after the change.
    /// Canonical naming alias that forwards to <see cref="CurrentIssue" /> for compatibility.
    /// </summary>
    public ValidationIssue? SelectedIssue => CurrentIssue;
}
