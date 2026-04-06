namespace Tessera.Controls;

/// <summary>
/// Represents one node in a <see cref="TreeView"/>.
/// </summary>
public sealed class TreeItem
{
    private readonly List<TreeItem> _children = [];

    public TreeItem(string id, string label, IEnumerable<TreeItem>? children = null)
    {
        Id = id ?? string.Empty;
        Label = label ?? string.Empty;
        if (children is not null)
        {
            _children.AddRange(children);
        }
    }

    public string Id { get; }

    public string Label { get; set; }

    public bool Expanded { get; set; } = true;

    public IReadOnlyList<TreeItem> Children => _children;

    public void AddChild(TreeItem child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
    }
}
