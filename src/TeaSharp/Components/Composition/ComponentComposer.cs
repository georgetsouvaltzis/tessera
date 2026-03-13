using TeaSharp.Components.Primitives;
using TeaSharp.Components.Composition.Internal;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ComponentComposer
{
    private readonly List<ComponentSlot> _slots = [];
    private int _focusedSlotIndex = -1;

    public IReadOnlyList<ComponentSlot> Slots => _slots;

    public bool ClickToFocusEnabled { get; set; } = true;

    public bool RouteMouseWheelToFocusedSlot { get; set; } = true;

    public KeyboardRoutingMode KeyboardRoutingMode { get; set; } = KeyboardRoutingMode.FocusedOnly;

    public int FocusedSlotIndex => _focusedSlotIndex;

    public void Clear()
    {
        _slots.Clear();
        _focusedSlotIndex = -1;
    }

    public void Add(ICanvasComponent component, Rect bounds)
    {
        _slots.Add(new ComponentSlot(component, bounds));
        if (component is IFocusableComponent { IsFocused: true })
        {
            _focusedSlotIndex = _slots.Count - 1;
        }
    }

    public bool Update(IMessage message)
    {
        return ComponentRouting.Update(
            _slots,
            message,
            ClickToFocusEnabled,
            RouteMouseWheelToFocusedSlot,
            KeyboardRoutingMode,
            ref _focusedSlotIndex);
    }

    public bool SetFocusedSlot(int index)
    {
        return ComponentRouting.SetFocusedSlot(_slots, index, ref _focusedSlotIndex);
    }

    public bool FocusFirst()
    {
        return ComponentRouting.FocusFirst(_slots, ref _focusedSlotIndex);
    }

    public bool FocusNext()
    {
        return ComponentRouting.FocusNext(_slots, ref _focusedSlotIndex);
    }

    public bool FocusPrevious()
    {
        return ComponentRouting.FocusPrevious(_slots, ref _focusedSlotIndex);
    }

    public bool ClearFocus()
    {
        return ComponentRouting.ClearFocus(_slots, ref _focusedSlotIndex);
    }

    public void Render(Canvas canvas)
    {
        foreach (var slot in _slots)
        {
            slot.Component.Render(canvas, slot.Bounds);
        }
    }
}
