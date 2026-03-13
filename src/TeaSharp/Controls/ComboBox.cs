using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a filterable single-selection control.
/// </summary>
/// <remarks>
/// Use this when the option list is too large for a simple choice control and users benefit from inline filtering.
/// </remarks>
public sealed class ComboBox : Control
{
    private readonly ComboboxComponent _component = new();

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public ComboBox()
    {
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(args.PreviousIndex, args.SelectedIndex, args.PreviousItem, args.SelectedItem));
    }

    /// <summary>
    /// Gets or sets the field title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the placeholder shown when no filter text is present.
    /// </summary>
    public string Placeholder
    {
        get => _component.Placeholder;
        set => _component.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the active filter text.
    /// </summary>
    public string FilterText => _component.FilterText;

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public string SelectedItem => _component.SelectedItem;

    /// <summary>
    /// Gets a value indicating whether the drop-down list is open.
    /// </summary>
    public bool IsOpen => _component.IsOpen;

    /// <summary>
    /// Gets or sets the maximum number of visible items while the list is open.
    /// </summary>
    public int MaxVisibleItems
    {
        get => _component.MaxVisibleItems;
        set => _component.MaxVisibleItems = value;
    }

    /// <summary>
    /// Gets or sets the field border style.
    /// </summary>
    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    /// <summary>
    /// Gets or sets the inner padding applied to the field body.
    /// </summary>
    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

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
    /// Replaces the available selection items.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<string> items) => _component.SetItems(items);

    /// <summary>
    /// Replaces the current filter text.
    /// </summary>
    /// <param name="value">The filter text to apply.</param>
    public void SetFilterText(string value) => _component.SetFilterText(value ?? string.Empty);

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
}
