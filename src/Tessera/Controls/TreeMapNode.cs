namespace Tessera.Controls;

/// <summary>
/// Represents one node in a <see cref="TreeMapChart"/> hierarchy.
/// </summary>
public sealed class TreeMapNode
{
    /// <summary>
    /// Initializes a new treemap node.
    /// </summary>
    /// <param name="name">Display name rendered for the node.</param>
    /// <param name="value">Numeric value used for area weighting when the node has no children.</param>
    public TreeMapNode(string? name, double value = 0d)
    {
        Name = name ?? string.Empty;
        Value = value;
    }

    /// <summary>
    /// Initializes a new treemap node with child nodes.
    /// </summary>
    /// <param name="name">Display name rendered for the node.</param>
    /// <param name="children">Child nodes.</param>
    /// <param name="value">Fallback value used when all children resolve to zero weight.</param>
    public TreeMapNode(string? name, IEnumerable<TreeMapNode> children, double value = 0d)
        : this(name, value)
    {
        ArgumentNullException.ThrowIfNull(children);
        foreach (var child in children)
        {
            if (child is not null)
            {
                Children.Add(child);
            }
        }
    }

    /// <summary>
    /// Gets or sets display name rendered for this node.
    /// </summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets numeric value used for leaf weighting.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets child nodes.
    /// </summary>
    public IList<TreeMapNode> Children { get; } = new List<TreeMapNode>();

    /// <summary>
    /// Gets whether the node currently has child nodes.
    /// </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>
    /// Adds one child node.
    /// </summary>
    /// <param name="child">Node to append.</param>
    public void AddChild(TreeMapNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        Children.Add(child);
    }

    internal double ResolveWeight()
    {
        if (Children.Count == 0)
        {
            return Value > 0 ? Value : 0d;
        }

        var total = 0d;
        for (var index = 0; index < Children.Count; index++)
        {
            total += Children[index].ResolveWeight();
        }

        if (total <= 0d && Value > 0d)
        {
            return Value;
        }

        return total;
    }
}
