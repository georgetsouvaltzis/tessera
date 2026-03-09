namespace TeaSharp.Components;

internal sealed record SortableTableRenderState(IReadOnlyList<IReadOnlyList<string>> VisibleRows, string Title, int VisibleRowCount);
