using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;
namespace TeaSharp.Components.UiKit.Internal;

internal sealed record SortableTableRenderState(IReadOnlyList<IReadOnlyList<string>> VisibleRows, string Title, int VisibleRowCount);
