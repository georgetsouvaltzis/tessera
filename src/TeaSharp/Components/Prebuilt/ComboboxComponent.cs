using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed partial class ComboboxComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly OptionListController _options = new();
    private readonly TextInputModel _input = new();
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private bool _fieldHovered;

    public ComboboxComponent()
    {
    }

    public ComboboxComponent(ComboboxOptions options)
    {
        Title = options.Title;
        Placeholder = options.Placeholder;
        IsFocused = options.IsFocused;
        IsDisabled = options.IsDisabled;
        IsReadOnly = options.IsReadOnly;
        Border = options.Border;
        Padding = options.Padding;
        MaxVisibleItems = options.MaxVisibleItems;
        InputKeyMap = options.InputKeyMap ?? TextInputKeyMap.Default;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
        OpenKey = options.OpenKey ?? new KeyBinding("down", "open", "down");
        CloseKey = options.CloseKey ?? new KeyBinding("esc", "close", "escape");
        NextItemKey = options.NextItemKey ?? new KeyBinding("down/j", "next item", "down", "j");
        PreviousItemKey = options.PreviousItemKey ?? new KeyBinding("up/k", "previous item", "up", "k");
        ConfirmSelectionKey = options.ConfirmSelectionKey ?? new KeyBinding("enter", "select", "enter");
        if (options.Items is { Count: > 0 } items)
        {
            SetItems(items);
        }

        if (!string.IsNullOrEmpty(options.InitialFilter))
        {
            SetFilterText(options.InitialFilter);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Combobox";

    public bool IsFocused { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsReadOnly { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette FieldStatePalette { get; } = WidgetStatePalette.CreateDefault();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette OptionStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public Func<string, int, IReadOnlyCollection<WidgetVisualState>?>? OptionStateResolver { get; set; }

    public bool IsOpen { get; private set; }

    public int SelectedIndex => _options.SelectedIndex;

    /// <summary>
    /// Raised when the selected option changes.
    /// </summary>
    public event EventHandler<OptionSelectionChangedEventArgs>? SelectionChanged;

    public int MaxVisibleItems { get; set; } = 6;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public KeyBinding OpenKey { get; set; } = new("down", "open", "down");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ConfirmSelectionKey { get; set; } = new("enter", "select", "enter");

    public string SelectedItem => _options.SelectedItem;

    public string FilterText => _input.Value;

    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value;
    }

    public void SetFilterText(string value)
    {
        _input.SetValue(value);
        _options.ApplyFilter(_input.Value);
    }

    public void SetItems(IEnumerable<string> items)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _options.SetItems(items, selectFirstItemWhenUnset: false);
        _options.ApplyFilter(_input.Value);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public bool Update(IMessage message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        if (message is KeyPressMsg key)
        {
            if (IsOpen && CloseKey.Matches(key))
            {
                IsOpen = false;
                return true;
            }

            if (IsOpen && NextItemKey.Matches(key) && _options.VisibleCount > 0)
            {
                _options.MoveNextVisible();
                return true;
            }

            if (IsOpen && PreviousItemKey.Matches(key) && _options.VisibleCount > 0)
            {
                _options.MovePreviousVisible();
                return true;
            }

            if (IsOpen && ConfirmSelectionKey.Matches(key))
            {
                return SelectHighlighted();
            }

            if (!IsOpen && OpenKey.Matches(key))
            {
                IsOpen = true;
                _options.AlignHighlightToSelectionOrStart();
                return true;
            }
        }

        var inputResult = _input.Update(message, InputKeyMap);
        if (inputResult.Changed)
        {
            _options.ApplyFilter(_input.Value);
            IsOpen = true;
            return true;
        }

        if (inputResult.Submitted && IsOpen && _options.VisibleCount > 0)
        {
            return SelectHighlighted();
        }

        return inputResult.Submitted;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly)
        {
            return false;
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(message.X, message.Y);
        var changed = false;

        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetFieldHovered(false);
                changed |= _options.SetHoveredVisibleIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && IsOpen && _options.VisibleCount > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _options.MoveNextVisible();
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _options.MovePreviousVisible();
                changed = true;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var hoveredField = message.Y == content.Y;
        var hoveredOption = RowToVisibleIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetFieldHovered(hoveredField);
            changed |= _options.SetHoveredVisibleIndex(hoveredOption);
            if (hoveredOption >= 0)
            {
                SetHighlightedVisibleIndex(hoveredOption);
            }

            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetFieldHovered(hoveredField);
                changed |= _options.SetHoveredVisibleIndex(hoveredOption);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick)
            {
                if (hoveredField)
                {
                    if (!IsOpen && InteractionProfile.OpenOnClick)
                    {
                        IsOpen = true;
                        _options.AlignHighlightToSelectionOrStart();
                        changed = true;
                    }
                    else if (IsOpen)
                    {
                        IsOpen = false;
                        changed = true;
                    }
                }
                else if (IsOpen && hoveredOption >= 0)
                {
                    changed |= SetHighlightedVisibleIndex(hoveredOption);
                    changed |= SelectHighlighted();
                }
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = ResolveRenderContentRect(canvas, clipped);
        if (content.IsEmpty)
        {
            return;
        }

        RenderField(canvas, content);
        RenderOpenOptions(canvas, content);
    }

    private bool SelectHighlighted()
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        if (!_options.TrySelectHighlighted(out var selectedIndex))
        {
            IsOpen = false;
            return true;
        }

        _input.SetValue(_options.Items[selectedIndex]);
        _options.ApplyFilter(_input.Value);
        IsOpen = false;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    private Rect ResolveContentRect(Rect bounds) =>
        FrameLayout.ResolveContentRect(bounds, Border, Padding);

    private int RowToVisibleIndex(Rect content, int y) =>
        IsOpen
            ? OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _options.VisibleCount, _options.HighlightedVisibleIndex)
            : -1;

    private bool SetFieldHovered(bool hovered)
    {
        if (_fieldHovered == hovered)
        {
            return false;
        }

        _fieldHovered = hovered;
        return true;
    }

    private bool SetHighlightedVisibleIndex(int index)
    {
        if (index < 0 || index >= _options.VisibleCount || index == _options.HighlightedVisibleIndex)
        {
            return false;
        }

        while (_options.HighlightedVisibleIndex != index)
        {
            if (_options.HighlightedVisibleIndex < index)
            {
                _options.MoveNextVisible();
            }
            else
            {
                _options.MovePreviousVisible();
            }
        }

        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, string previousItem)
    {
        if (previousIndex == SelectedIndex && string.Equals(previousItem, SelectedItem, StringComparison.Ordinal))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new OptionSelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }
}
