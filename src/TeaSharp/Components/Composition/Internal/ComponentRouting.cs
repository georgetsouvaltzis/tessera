using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition.Internal;

internal static class ComponentRouting
{
    public static int DetectFocusedSlotIndex(IReadOnlyList<ComponentSlot> slots)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            if (slots[i].Component is IFocusableComponent { Focused: true })
            {
                return i;
            }
        }

        return -1;
    }

    public static bool Update(
        IReadOnlyList<ComponentSlot> slots,
        IMessage message,
        bool clickToFocusEnabled,
        bool routeMouseWheelToFocusedSlot,
        KeyboardRoutingMode keyboardRoutingMode,
        ref int focusedSlotIndex)
    {
        if (message is MouseMsg mouse)
        {
            return UpdateMouse(slots, mouse, clickToFocusEnabled, routeMouseWheelToFocusedSlot, ref focusedSlotIndex);
        }

        if (keyboardRoutingMode == KeyboardRoutingMode.FocusedOnly
            && TryGetFocusedStateful(slots, focusedSlotIndex, out var focusedStateful))
        {
            return focusedStateful.Update(message);
        }

        var changed = false;
        foreach (var slot in slots)
        {
            if (slot.Component is IStatefulComponent stateful)
            {
                changed |= stateful.Update(message);
            }
        }

        return changed;
    }

    public static bool SetFocusedSlot(IReadOnlyList<ComponentSlot> slots, int index, ref int focusedSlotIndex)
    {
        if (index < 0 || index >= slots.Count)
        {
            return false;
        }

        if (slots[index].Component is not IFocusableComponent)
        {
            return false;
        }

        return ApplyFocus(slots, index, ref focusedSlotIndex);
    }

    public static bool FocusFirst(IReadOnlyList<ComponentSlot> slots, ref int focusedSlotIndex)
    {
        var targetIndex = FindFocusableSlot(slots, startIndex: -1, step: 1);
        return targetIndex >= 0 && ApplyFocus(slots, targetIndex, ref focusedSlotIndex);
    }

    public static bool FocusNext(IReadOnlyList<ComponentSlot> slots, ref int focusedSlotIndex)
    {
        var targetIndex = FindFocusableSlot(slots, focusedSlotIndex, 1);
        return targetIndex >= 0 && ApplyFocus(slots, targetIndex, ref focusedSlotIndex);
    }

    public static bool FocusPrevious(IReadOnlyList<ComponentSlot> slots, ref int focusedSlotIndex)
    {
        var startIndex = focusedSlotIndex >= 0 ? focusedSlotIndex : slots.Count;
        var targetIndex = FindFocusableSlot(slots, startIndex, -1);
        return targetIndex >= 0 && ApplyFocus(slots, targetIndex, ref focusedSlotIndex);
    }

    public static bool ClearFocus(IReadOnlyList<ComponentSlot> slots, ref int focusedSlotIndex)
    {
        var changed = false;
        foreach (var slot in slots)
        {
            if (slot.Component is not IFocusableComponent focusable || !focusable.Focused)
            {
                continue;
            }

            focusable.Focused = false;
            changed = true;
        }

        focusedSlotIndex = -1;
        return changed;
    }

    private static bool UpdateMouse(
        IReadOnlyList<ComponentSlot> slots,
        MouseMsg mouse,
        bool clickToFocusEnabled,
        bool routeMouseWheelToFocusedSlot,
        ref int focusedSlotIndex)
    {
        var changed = false;
        var targetIndex = FindTopMostSlot(slots, mouse.X, mouse.Y);
        if (targetIndex < 0 && routeMouseWheelToFocusedSlot && mouse is MouseWheelMsg && focusedSlotIndex >= 0)
        {
            targetIndex = focusedSlotIndex;
        }

        if (targetIndex >= 0
            && clickToFocusEnabled
            && mouse is MouseClickMsg { Button: MouseButton.Left }
            && slots[targetIndex].Component is IFocusableComponent)
        {
            changed |= ApplyFocus(slots, targetIndex, ref focusedSlotIndex);
        }

        if (targetIndex < 0)
        {
            return changed;
        }

        var slot = slots[targetIndex];
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

    private static int FindFocusableSlot(IReadOnlyList<ComponentSlot> slots, int startIndex, int step)
    {
        if (slots.Count == 0)
        {
            return -1;
        }

        for (var offset = 1; offset <= slots.Count; offset++)
        {
            var index = startIndex + (offset * step);
            if (index < 0)
            {
                index += slots.Count;
            }
            else if (index >= slots.Count)
            {
                index -= slots.Count;
            }

            if (slots[index].Component is IFocusableComponent)
            {
                return index;
            }
        }

        return -1;
    }

    private static bool TryGetFocusedStateful(IReadOnlyList<ComponentSlot> slots, int focusedSlotIndex, out IStatefulComponent stateful)
    {
        stateful = default!;
        if (focusedSlotIndex < 0 || focusedSlotIndex >= slots.Count)
        {
            return false;
        }

        if (slots[focusedSlotIndex].Component is not IStatefulComponent focusedStateful)
        {
            return false;
        }

        stateful = focusedStateful;
        return true;
    }

    private static int FindTopMostSlot(IReadOnlyList<ComponentSlot> slots, int x, int y)
    {
        for (var i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].Bounds.Contains(x, y))
            {
                return i;
            }
        }

        return -1;
    }

    private static bool ApplyFocus(IReadOnlyList<ComponentSlot> slots, int index, ref int focusedSlotIndex)
    {
        var changed = false;
        for (var i = 0; i < slots.Count; i++)
        {
            if (slots[i].Component is not IFocusableComponent focusable)
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

        focusedSlotIndex = index;
        return changed;
    }
}
