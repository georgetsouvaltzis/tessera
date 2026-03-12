namespace TeaSharp.Controls;

public sealed class ListSelectionChangedEventArgs<T> : EventArgs
{
    public ListSelectionChangedEventArgs(int previousIndex, int selectedIndex, T? previousItem, T? selectedItem)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousItem = previousItem;
        SelectedItem = selectedItem;
    }

    public int PreviousIndex { get; }

    public int SelectedIndex { get; }

    public T? PreviousItem { get; }

    public T? SelectedItem { get; }
}
