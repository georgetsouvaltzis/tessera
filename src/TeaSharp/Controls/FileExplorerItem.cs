namespace TeaSharp.Controls;

/// <summary>
/// Represents one node in a <see cref="FileExplorer"/> tree.
/// </summary>
public sealed class FileExplorerItem
{
    private readonly List<FileExplorerItem> _children = [];

    /// <summary>
    /// Initializes a new explorer node.
    /// </summary>
    /// <param name="name">Display name.</param>
    /// <param name="isDirectory"><see langword="true"/> for directory nodes; otherwise file nodes.</param>
    /// <param name="path">Optional stable path/key. Defaults to <paramref name="name"/>.</param>
    /// <param name="children">Optional child nodes.</param>
    public FileExplorerItem(
        string name,
        bool isDirectory,
        string? path = null,
        IEnumerable<FileExplorerItem>? children = null)
    {
        Name = name ?? string.Empty;
        IsDirectory = isDirectory;
        Path = string.IsNullOrWhiteSpace(path) ? Name : path;
        if (children is not null)
        {
            foreach (var child in children)
            {
                if (child is not null)
                {
                    _children.Add(child);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets a stable path/key used for selection and lookup.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a value indicating whether this node is a directory.
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// Gets or sets a value indicating whether a directory node is expanded.
    /// </summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>
    /// Gets child nodes.
    /// </summary>
    public IReadOnlyList<FileExplorerItem> Children => _children;

    /// <summary>
    /// Adds a child node.
    /// </summary>
    /// <param name="child">The child to add.</param>
    public void AddChild(FileExplorerItem child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
    }
}
