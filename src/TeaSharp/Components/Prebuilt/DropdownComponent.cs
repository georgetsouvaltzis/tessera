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

public sealed partial class DropdownComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly OptionListController _options = new();
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private bool _fieldHovered;

    public DropdownComponent()
    {
    }

    public DropdownComponent(DropdownOptions options)
    {
        Title = options.Title;
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        Border = options.Border;
        Padding = options.Padding;
        MaxVisibleItems = options.MaxVisibleItems;
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
        ToggleOpenKey = options.ToggleOpenKey ?? new KeyBinding("enter/space", "toggle", "enter", "space");
        OpenKey = options.OpenKey ?? new KeyBinding("down", "open", "down");
        CloseKey = options.CloseKey ?? new KeyBinding("esc", "close", "escape");
        NextItemKey = options.NextItemKey ?? new KeyBinding("down/j", "next item", "down", "j");
        PreviousItemKey = options.PreviousItemKey ?? new KeyBinding("up/k", "previous item", "up", "k");
        ConfirmSelectionKey = options.ConfirmSelectionKey ?? new KeyBinding("enter/space", "select", "enter", "space");
        if (options.Items is { Count: > 0 } items)
        {
            SetItems(items);
        }
    }

    public string Title { get; set; } = "Dropdown";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

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

    public KeyBinding ToggleOpenKey { get; set; } = new("enter/space", "toggle", "enter", "space");

    public KeyBinding OpenKey { get; set; } = new("down", "open", "down");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ConfirmSelectionKey { get; set; } = new("enter/space", "select", "enter", "space");

    public string SelectedItem => _options.SelectedItem;

    public void SetItems(IEnumerable<string> items)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _options.SetItems(items, selectFirstItemWhenUnset: true);
        _fieldHovered = false;
        if (_options.Count == 0)
        {
            IsOpen = false;
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key || _options.Count == 0)
        {
            return false;
        }

        if (!IsOpen)
        {
            if (ToggleOpenKey.Matches(key) || OpenKey.Matches(key))
            {
                IsOpen = true;
                _options.AlignHighlightToSelectionOrStart();
                return true;
            }

            return false;
        }

        if (CloseKey.Matches(key))
        {
            IsOpen = false;
            return true;
        }

        if (NextItemKey.Matches(key))
        {
            _options.MoveNextVisible();
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            _options.MovePreviousVisible();
            return true;
        }

        if (ConfirmSelectionKey.Matches(key))
        {
            var changed = SelectHighlighted();
            IsOpen = false;
            return changed || true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly || _options.Count == 0)
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

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && IsOpen)
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
        var hoveredOptionIndex = RowToVisibleIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetFieldHovered(hoveredField);
            changed |= _options.SetHoveredVisibleIndex(hoveredOptionIndex);
            if (hoveredOptionIndex >= 0)
            {
                changed |= SetHighlightedVisibleIndex(hoveredOptionIndex);
            }

            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetFieldHovered(hoveredField);
                changed |= _options.SetHoveredVisibleIndex(hoveredOptionIndex);
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
                else if (IsOpen && hoveredOptionIndex >= 0)
                {
                    changed |= SelectVisible(hoveredOptionIndex);
                    IsOpen = false;
                    changed = true;
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

    private Rect ResolveContentRect(Rect bounds) =>
        FrameLayout.ResolveContentRect(bounds, Border, Padding);

    private int RowToVisibleIndex(Rect content, int y) =>
        IsOpen
            ? OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _options.VisibleCount, _options.HighlightedVisibleIndex)
            : -1;

    private bool SelectVisible(int visibleIndex)
    {
        if (visibleIndex < 0 || visibleIndex >= _options.VisibleCount)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _options.SetSelectedIndex(_options.VisibleItemIndexAt(visibleIndex));
        if (changed)
        {
            RaiseSelectionChanged(previousIndex, previousItem);
        }

        return changed;
    }

    private bool SelectHighlighted()
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = _options.TrySelectHighlighted(out _);
        if (changed)
        {
            RaiseSelectionChanged(previousIndex, previousItem);
        }

        return changed;
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

    private bool SetFieldHovered(bool hovered)
    {
        if (_fieldHovered == hovered)
        {
            return false;
        }

        _fieldHovered = hovered;
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, string previousItem)
    {
        if (previousIndex == SelectedIndex && string.Equals(previousItem, SelectedItem, StringComparison.Ordinal))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousItem);
    }

    private void RaiseSelectionChanged(int previousIndex, string previousItem)
    {
        SelectionChanged?.Invoke(this, new OptionSelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }
}
