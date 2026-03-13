using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
namespace TeaSharp.Components.UiKit;

internal readonly record struct TreeNode(string Label, int Depth, bool Selected = false);
