using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a scrollable single-selection list.
/// </summary>
/// <typeparam name="T">The item type shown by the list.</typeparam>
public sealed class ListView<T> : Control
{
    private readonly ListComponent<T> _component;

    /// <summary>
    /// Initializes a new list view.
    /// </summary>
    /// <param name="textSelector">Optional item-to-text projection.</param>
    public ListView(Func<T, string>? textSelector = null)
    {
        _component = new ListComponent<T>(Array.Empty<T>(), textSelector ?? DefaultText);
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<T>(args.PreviousIndex, args.SelectedIndex, args.PreviousItem, args.SelectedItem));
    }

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<T>>? SelectionChanged;

    /// <summary>
    /// Gets or sets the list title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the list border style.
    /// </summary>
    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    /// <summary>
    /// Gets or sets the inner padding applied to the list body.
    /// </summary>
    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    /// <summary>
    /// Gets or sets how many items fit in a page-sized view.
    /// </summary>
    public int PageSize
    {
        get => _component.PageSize;
        set => _component.PageSize = value;
    }

    /// <summary>
    /// Gets the number of currently visible items after filtering.
    /// </summary>
    public int Count => _component.Count;

    /// <summary>
    /// Gets the current selected index.
    /// </summary>
    public int SelectedIndex => _component.SelectedIndex;

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public T? SelectedItem => _component.SelectedItem;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _component.IsDisabled;
        set => _component.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _component.IsReadOnly;
        set => _component.IsReadOnly = value;
    }

    /// <summary>
    /// Replaces the items shown by the list.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<T> items) => _component.SetItems(items);

    /// <summary>
    /// Applies a filter string to the list items.
    /// </summary>
    /// <param name="filter">The filter string.</param>
    public void SetFilter(string filter) => _component.SetFilter(filter ?? string.Empty);

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return ControlForwarder.Forward(_component, message, bounds) || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }

    private static string DefaultText(T item) => item?.ToString() ?? string.Empty;
}
