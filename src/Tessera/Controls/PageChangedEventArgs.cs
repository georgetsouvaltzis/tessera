namespace Tessera.Controls;

/// <summary>
/// Provides details about a paginator page transition.
/// </summary>
public sealed class PageChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new page change payload.
    /// </summary>
    /// <param name="previousPageIndex">The page index before the transition.</param>
    /// <param name="newPageIndex">The page index after the transition.</param>
    public PageChangedEventArgs(int previousPageIndex, int newPageIndex)
    {
        PreviousPageIndex = previousPageIndex;
        NewPageIndex = newPageIndex;
    }

    /// <summary>
    /// Gets the page index before the change.
    /// </summary>
    public int PreviousPageIndex { get; }

    /// <summary>
    /// Gets the page index after the change.
    /// </summary>
    public int NewPageIndex { get; }
}
