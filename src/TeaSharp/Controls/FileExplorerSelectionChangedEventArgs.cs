namespace TeaSharp.Controls;

/// <summary>
/// Provides details when a <see cref="FileExplorer"/> selection changes.
/// </summary>
public sealed class FileExplorerSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new selection payload.
    /// </summary>
    /// <param name="previousPath">The previously selected path, if any.</param>
    /// <param name="currentPath">The current selected path, if any.</param>
    /// <param name="previousItem">The previously selected item, if any.</param>
    /// <param name="currentItem">The current selected item, if any.</param>
    public FileExplorerSelectionChangedEventArgs(
        string? previousPath,
        string? currentPath,
        FileExplorerItem? previousItem,
        FileExplorerItem? currentItem)
    {
        PreviousPath = previousPath;
        CurrentPath = currentPath;
        PreviousItem = previousItem;
        CurrentItem = currentItem;
    }

    /// <summary>
    /// Gets the previously selected path, if any.
    /// </summary>
    public string? PreviousPath { get; }

    /// <summary>
    /// Gets the selected path, if any.
    /// Compatibility alias for <see cref="SelectedPath" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public string? CurrentPath { get; }

    /// <summary>
    /// Gets the selected path, if any.
    /// Canonical property for selection access.
    /// </summary>
    public string? SelectedPath => CurrentPath;

    /// <summary>
    /// Gets the previously selected item, if any.
    /// </summary>
    public FileExplorerItem? PreviousItem { get; }

    /// <summary>
    /// Gets the selected item, if any.
    /// Compatibility alias for <see cref="SelectedItem" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public FileExplorerItem? CurrentItem { get; }

    /// <summary>
    /// Gets the selected item, if any.
    /// Canonical property for selection access.
    /// </summary>
    public FileExplorerItem? SelectedItem => CurrentItem;
}
