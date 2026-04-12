using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class FileExplorer
{
    private bool TryFindNode(string path, out FileExplorerItem node)
    {
        var stack = new Stack<FileExplorerItem>(_roots);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (string.Equals(current.Path, path, StringComparison.Ordinal))
            {
                node = current;
                return true;
            }

            for (var index = current.Children.Count - 1; index >= 0; index--)
            {
                stack.Push(current.Children[index]);
            }
        }

        node = null!;
        return false;
    }

    private bool TryFindPath(string path, out List<FileExplorerItem> chain)
    {
        chain = [];
        for (var index = 0; index < _roots.Count; index++)
        {
            if (TryFindPathRecursive(_roots[index], path, chain))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryFindPathRecursive(FileExplorerItem current, string path, List<FileExplorerItem> chain)
    {
        chain.Add(current);
        if (string.Equals(current.Path, path, StringComparison.Ordinal))
        {
            return true;
        }

        for (var i = 0; i < current.Children.Count; i++)
        {
            if (TryFindPathRecursive(current.Children[i], path, chain))
            {
                return true;
            }
        }

        chain.RemoveAt(chain.Count - 1);
        return false;
    }

    private void RaiseSelectionChangedIfNeeded(string? previousPath, FileExplorerItem? previousItem)
    {
        if (string.Equals(previousPath, SelectedPath, StringComparison.Ordinal)
            && ReferenceEquals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new FileExplorerSelectionChangedEventArgs(previousPath, SelectedPath, previousItem, SelectedItem));
    }

    private static FileExplorerItem Clone(FileExplorerItem source)
    {
        var clone = new FileExplorerItem(source.Name, source.IsDirectory, source.Path)
        {
            IsExpanded = source.IsExpanded,
        };
        for (var i = 0; i < source.Children.Count; i++)
        {
            clone.AddChild(Clone(source.Children[i]));
        }

        return clone;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }

    private readonly record struct VisibleEntry(FileExplorerItem Item, int Depth, int? ParentVisibleIndex);
}
