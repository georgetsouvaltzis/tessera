namespace Tessera.Controls;

/// <summary>
///     Represents one node in a <see cref="TreeView" />.
/// </summary>
public sealed class TreeItem
{
    private readonly List<TreeItem> _children = [];

    /// <summary>
    ///     Executes tree item.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <param name="label">The label value.</param>
    /// <param name="children">The children value.</param>
    /// <returns>The result of tree item.</returns>
    public TreeItem(string id, string label, IEnumerable<TreeItem>? children = null)
    {
        Id = id;
        Label = label;
        if (children is not null)
        {
            _children.AddRange(children);
        }
    }

    /// <summary>
    ///     Gets the id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     Gets or sets the label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     Gets or sets the expanded.
    /// </summary>
    public bool Expanded { get; set; } = true;

    /// <summary>
    ///     Represents children.
    /// </summary>
    public IReadOnlyList<TreeItem> Children => _children;

    /// <summary>
    ///     Executes add child.
    /// </summary>
    /// <param name="child">The child value.</param>
    public void AddChild(TreeItem child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
    }
}
