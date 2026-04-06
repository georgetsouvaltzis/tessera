namespace Tessera.Controls;

/// <summary>
/// Identifies the payload shape for a <see cref="JsonTreeNode" />.
/// </summary>
public enum JsonTreeNodeKind
{
    /// <summary>
    /// JSON object container.
    /// </summary>
    Object = 0,

    /// <summary>
    /// JSON array container.
    /// </summary>
    Array = 1,

    /// <summary>
    /// Scalar JSON value.
    /// </summary>
    Value = 2,
}

/// <summary>
/// Represents one node in a JSON inspection tree.
/// </summary>
public sealed class JsonTreeNode
{
    /// <summary>
    /// Initializes a JSON tree node.
    /// </summary>
    /// <param name="key">Property key or array index label.</param>
    /// <param name="displayValue">Display value for scalar nodes or container summary text.</param>
    /// <param name="kind">Node kind.</param>
    /// <param name="children">Child nodes.</param>
    public JsonTreeNode(
        string key,
        string displayValue,
        JsonTreeNodeKind kind,
        IEnumerable<JsonTreeNode>? children = null)
    {
        Key = key ?? string.Empty;
        DisplayValue = displayValue ?? string.Empty;
        Kind = kind;
        if (children is not null)
        {
            foreach (var child in children)
            {
                if (child is not null)
                {
                    Children.Add(child);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets property key or array index label.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets display value for the node.
    /// </summary>
    public string DisplayValue { get; set; }

    /// <summary>
    /// Gets or sets node kind.
    /// </summary>
    public JsonTreeNodeKind Kind { get; set; }

    /// <summary>
    /// Gets child nodes.
    /// </summary>
    public List<JsonTreeNode> Children { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether child nodes are visible.
    /// </summary>
    public bool Expanded { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether the node is a container.
    /// </summary>
    public bool IsContainer => Kind is JsonTreeNodeKind.Object or JsonTreeNodeKind.Array;
}
