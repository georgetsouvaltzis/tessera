namespace Tessera.Controls;

/// <summary>
///     Provides old/new selection data for <see cref="Breadcrumb" /> changes.
/// </summary>
public sealed class BreadcrumbSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Executes breadcrumb selection changed event args.
    /// </summary>
    /// <param name="previousIndex">The previous index value.</param>
    /// <param name="selectedIndex">The selected index value.</param>
    /// <param name="previousItem">The previous item value.</param>
    /// <param name="selectedItem">The selected item value.</param>
    /// <returns>The result of breadcrumb selection changed event args.</returns>
    public BreadcrumbSelectionChangedEventArgs(int previousIndex, int selectedIndex, BreadcrumbItem? previousItem,
        BreadcrumbItem? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    /// <summary>
    ///     Gets the previous index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    ///     Gets the selected index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    ///     Gets the previous item.
    /// </summary>
    public BreadcrumbItem? PreviousItem { get; }

    /// <summary>
    ///     Gets the selected item.
    /// </summary>
    public BreadcrumbItem? SelectedItem { get; }
}
