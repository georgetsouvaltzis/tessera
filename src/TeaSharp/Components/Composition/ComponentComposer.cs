using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public sealed class ComponentComposer
{
    private readonly List<ComponentSlot> _slots = [];

    public IReadOnlyList<ComponentSlot> Slots => _slots;

    public bool ClickToFocusEnabled { get; set; } = true;

    public bool RouteMouseWheelToFocusedSlot { get; set; } = true;

    public KeyboardRoutingMode KeyboardRoutingMode { get; set; } = KeyboardRoutingMode.FocusedOnly;

    public int FocusedSlotIndex { get; private set; } = -1;

    public void Clear()
    {
        _slots.Clear();
        FocusedSlotIndex = -1;
    }

    public void Add(ICanvasComponent component, Rect bounds)
    {
        _slots.Add(new ComponentSlot(component, bounds));
        if (component is IFocusableComponent { Focused: true })
        {
            FocusedSlotIndex = _slots.Count - 1;
        }
    }

    public bool Update(IMessage message)
    {
        if (message is MouseMsg mouse)
        {
            return UpdateMouse(mouse);
        }

        if (KeyboardRoutingMode == KeyboardRoutingMode.FocusedOnly && TryGetFocusedStateful(out var focusedStateful))
        {
            return focusedStateful.Update(message);
        }

        var changed = false;
        foreach (var slot in _slots)
        {
            if (slot.Component is IStatefulComponent stateful)
            {
                changed |= stateful.Update(message);
            }
        }

        return changed;
    }

    public bool SetFocusedSlot(int index)
    {
        if (index < 0 || index >= _slots.Count)
        {
            return false;
        }

        if (_slots[index].Component is not IFocusableComponent)
        {
            return false;
        }

        return ApplyFocus(index);
    }

    public bool ClearFocus()
    {
        var changed = false;
        foreach (var slot in _slots)
        {
            if (slot.Component is not IFocusableComponent focusable || !focusable.Focused)
            {
                continue;
            }

            focusable.Focused = false;
            changed = true;
        }

        FocusedSlotIndex = -1;
        return changed;
    }

    public void Render(Canvas canvas)
    {
        foreach (var slot in _slots)
        {
            slot.Component.Render(canvas, slot.Bounds);
        }
    }

    private bool UpdateMouse(MouseMsg mouse)
    {
        var changed = false;
        var targetIndex = FindTopMostSlot(mouse.X, mouse.Y);
        if (targetIndex < 0 && RouteMouseWheelToFocusedSlot && mouse is MouseWheelMsg && FocusedSlotIndex >= 0)
        {
            targetIndex = FocusedSlotIndex;
        }

        if (targetIndex >= 0
            && ClickToFocusEnabled
            && mouse is MouseClickMsg { Button: MouseButton.Left }
            && _slots[targetIndex].Component is IFocusableComponent)
        {
            changed |= ApplyFocus(targetIndex);
        }

        if (targetIndex < 0)
        {
            return changed;
        }

        var slot = _slots[targetIndex];
        if (slot.Component is IMouseStatefulComponent mouseStateful)
        {
            changed |= mouseStateful.UpdateMouse(mouse, slot.Bounds);
            return changed;
        }

        if (slot.Component is IStatefulComponent stateful)
        {
            changed |= stateful.Update(mouse);
        }

        return changed;
    }

    private bool TryGetFocusedStateful(out IStatefulComponent stateful)
    {
        stateful = default!;
        if (FocusedSlotIndex < 0 || FocusedSlotIndex >= _slots.Count)
        {
            return false;
        }

        if (_slots[FocusedSlotIndex].Component is not IStatefulComponent focusedStateful)
        {
            return false;
        }

        stateful = focusedStateful;
        return true;
    }

    private int FindTopMostSlot(int x, int y)
    {
        for (var i = _slots.Count - 1; i >= 0; i--)
        {
            if (_slots[i].Bounds.Contains(x, y))
            {
                return i;
            }
        }

        return -1;
    }

    private bool ApplyFocus(int index)
    {
        var changed = false;
        for (var i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Component is not IFocusableComponent focusable)
            {
                continue;
            }

            var shouldFocus = i == index;
            if (focusable.Focused == shouldFocus)
            {
                continue;
            }

            focusable.Focused = shouldFocus;
            changed = true;
        }

        FocusedSlotIndex = index;
        return changed;
    }
}
