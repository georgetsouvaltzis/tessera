namespace Tessera.Controls;

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
    /// Gets previously selected node.
    /// </summary>
    public JsonTreeNode? PreviousNode { get; }

    /// <summary>
    /// Gets the selected node after the change.
    /// Compatibility alias for <see cref="SelectedNode" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public JsonTreeNode? CurrentNode { get; }

    /// <summary>
    /// Gets the selected node after the change.
    /// Canonical property for selection access.
    /// </summary>
    public JsonTreeNode? SelectedNode => CurrentNode;
}
