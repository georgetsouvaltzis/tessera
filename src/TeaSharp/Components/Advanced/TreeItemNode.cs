using TeaSharp.Components.Advanced.Internal;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Advanced;

internal sealed class TreeItemNode
{
    private readonly List<TreeItemNode> _children = [];

    public TreeItemNode(string id, string label, IEnumerable<TreeItemNode>? children = null)
    {
        Id = id;
        Label = label;
        if (children is not null)
        {
            _children.AddRange(children);
        }
    }

    public string Id { get; }

    public string Label { get; set; }

    public bool Expanded { get; set; } = true;

    public List<WidgetVisualState> States { get; } = [];

    public IReadOnlyList<TreeItemNode> Children => _children;

    public void AddChild(TreeItemNode child)
    {
        _children.Add(child);
    }
}
