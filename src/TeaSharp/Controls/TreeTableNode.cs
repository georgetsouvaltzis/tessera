namespace TeaSharp.Controls;

/// <summary>
/// Represents one hierarchical row in a <see cref="TreeTable"/>.
/// </summary>
public sealed class TreeTableNode
{
    private readonly List<TreeTableNode> _children = [];
    private readonly List<string> _values = [];

    /// <summary>
    /// Initializes a new tree-table node.
    /// </summary>
    /// <param name="id">Stable node identifier.</param>
    /// <param name="label">Primary label rendered in the tree column.</param>
    /// <param name="values">Optional per-row values for non-tree columns.</param>
    /// <param name="children">Optional child rows.</param>
    public TreeTableNode(
        string id,
        string label,
        IEnumerable<string>? values = null,
        IEnumerable<TreeTableNode>? children = null)
    {
        Id = id ?? string.Empty;
        Label = label ?? string.Empty;
        if (values is not null)
        {
            foreach (var value in values)
            {
                _values.Add(value ?? string.Empty);
            }
        }

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
    /// Gets the stable node identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets the display label for the tree column.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets a value indicating whether this node currently has child rows.
    /// </summary>
    public bool IsBranch => _children.Count > 0;

    /// <summary>
    /// Gets or sets a value indicating whether child rows are visible.
    /// </summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>
    /// Gets the non-tree column values for this row.
    /// </summary>
    public IReadOnlyList<string> Values => _values;

    /// <summary>
    /// Gets child rows.
    /// </summary>
    public IReadOnlyList<TreeTableNode> Children => _children;

    /// <summary>
    /// Adds a child row.
    /// </summary>
    /// <param name="child">Child row to add.</param>
    public void AddChild(TreeTableNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
    }

    /// <summary>
    /// Replaces all non-tree column values.
    /// </summary>
    /// <param name="values">New values.</param>
    public void SetValues(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values.Clear();
        foreach (var value in values)
        {
            _values.Add(value ?? string.Empty);
        }
    }
}
