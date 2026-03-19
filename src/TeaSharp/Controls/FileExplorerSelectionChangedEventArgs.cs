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
    /// Gets the current selected path, if any.
    /// </summary>
    public string? CurrentPath { get; }

    /// <summary>
    /// Gets the previously selected item, if any.
    /// </summary>
    public FileExplorerItem? PreviousItem { get; }

    /// <summary>
    /// Gets the current selected item, if any.
    /// </summary>
    public FileExplorerItem? CurrentItem { get; }
}
