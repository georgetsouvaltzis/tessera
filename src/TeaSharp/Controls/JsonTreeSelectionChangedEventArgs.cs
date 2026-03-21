namespace TeaSharp.Controls;

/// <summary>
/// Provides details when <see cref="JsonTreeView" /> selection changes.
/// </summary>
public sealed class JsonTreeSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes selection change details.
    /// </summary>
    /// <param name="previousIndex">Previous selected index.</param>
    /// <param name="currentIndex">Current selected index.</param>
    /// <param name="previousNode">Previous selected node.</param>
    /// <param name="currentNode">Current selected node.</param>
    public JsonTreeSelectionChangedEventArgs(
        int previousIndex,
        int currentIndex,
        JsonTreeNode? previousNode,
        JsonTreeNode? currentNode)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousNode = previousNode;
        CurrentNode = currentNode;
    }

    /// <summary>
    /// Gets previous selected index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets current selected index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets previously selected node.
    /// </summary>
    public JsonTreeNode? PreviousNode { get; }

    /// <summary>
    /// Gets currently selected node.
    /// </summary>
    public JsonTreeNode? CurrentNode { get; }
}
