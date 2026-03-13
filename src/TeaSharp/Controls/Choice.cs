using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a single-choice selector backed by a dropdown interaction model.
/// </summary>
public sealed class Choice : Control
{
    private readonly DropdownComponent _component = new();

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public Choice()
    {
        _component.SelectionChanged += (_, args) =>
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(args.PreviousIndex, args.SelectedIndex, args.PreviousItem, args.SelectedItem));
    }

    /// <summary>
    /// Gets or sets the selector title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
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
    /// Gets or sets the inner padding applied to the selector.
    /// </summary>
    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    /// <summary>
    /// Gets or sets the maximum number of visible items while open.
    /// </summary>
    public int MaxVisibleItems
    {
        get => _component.MaxVisibleItems;
        set => _component.MaxVisibleItems = value;
    }

    /// <summary>
    /// Gets a value indicating whether the selector is currently open.
    /// </summary>
    public bool IsOpen => _component.IsOpen;

    /// <summary>
    /// Gets the current selected index.
    /// </summary>
    public int SelectedIndex => _component.SelectedIndex;

    /// <summary>
    /// Gets the current selected item.
    /// </summary>
    public string SelectedItem => _component.SelectedItem;

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
    /// Replaces the available choice items.
    /// </summary>
    /// <param name="items">The items to display.</param>
    public void SetItems(IEnumerable<string> items) => _component.SetItems(items);

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
