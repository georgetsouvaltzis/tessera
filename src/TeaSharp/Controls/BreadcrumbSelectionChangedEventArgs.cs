namespace TeaSharp.Controls;

/// <summary>
/// Provides old/new selection data for <see cref="Breadcrumb"/> changes.
/// </summary>
public sealed class BreadcrumbSelectionChangedEventArgs : EventArgs
{
    public BreadcrumbSelectionChangedEventArgs(int previousIndex, int selectedIndex, BreadcrumbItem? previousItem, BreadcrumbItem? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    public int PreviousIndex { get; }

    public int SelectedIndex { get; }

    public BreadcrumbItem? PreviousItem { get; }

    public BreadcrumbItem? SelectedItem { get; }
}
