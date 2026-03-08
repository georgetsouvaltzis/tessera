using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}

public interface IStatefulComponent : ICanvasComponent
{
    bool Update(IMessage message);
}

public interface IMouseStatefulComponent : ICanvasComponent
{
    bool UpdateMouse(MouseMsg message, Rect bounds);
}

public readonly record struct ComponentSlot(ICanvasComponent Component, Rect Bounds);

public sealed class ComponentComposer
{
    private readonly List<ComponentSlot> _slots = [];

    public IReadOnlyList<ComponentSlot> Slots => _slots;

    public bool ClickToFocusEnabled { get; set; } = true;

    public bool RouteMouseWheelToFocusedSlot { get; set; } = true;

    public int FocusedSlotIndex { get; private set; } = -1;

    public void Clear()
    {
        _slots.Clear();
        FocusedSlotIndex = -1;
    }

    public void Add(ICanvasComponent component, Rect bounds)
    {
        _slots.Add(new ComponentSlot(component, bounds));
        if (TryGetFocused(component, out var focused) && focused)
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
            && mouse is MouseClickMsg { Button: MouseButton.Left })
        {
            changed |= SetFocus(targetIndex);
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

    private bool SetFocus(int index)
    {
        var changed = false;
        var focusAssigned = false;
        for (var i = 0; i < _slots.Count; i++)
        {
            var shouldFocus = i == index;
            if (shouldFocus && TryGetFocused(_slots[i].Component, out _))
            {
                focusAssigned = true;
            }

            if (!TrySetFocused(_slots[i].Component, shouldFocus))
            {
                continue;
            }

            changed = true;
        }

        if (focusAssigned)
        {
            FocusedSlotIndex = index;
        }

        return changed;
    }

    private static bool TryGetFocused(ICanvasComponent component, out bool focused)
    {
        focused = false;
        var property = component.GetType().GetProperty("Focused");
        if (property is null || property.PropertyType != typeof(bool) || !property.CanRead)
        {
            return false;
        }

        if (property.GetValue(component) is not bool value)
        {
            return false;
        }

        focused = value;
        return true;
    }

    private static bool TrySetFocused(ICanvasComponent component, bool focused)
    {
        var property = component.GetType().GetProperty("Focused");
        if (property is null || property.PropertyType != typeof(bool) || !property.CanWrite)
        {
            return false;
        }

        if (property.GetValue(component) is bool current && current == focused)
        {
            return false;
        }

        property.SetValue(component, focused);
        return true;
    }

    public void Render(Canvas canvas)
    {
        foreach (var slot in _slots)
        {
            slot.Component.Render(canvas, slot.Bounds);
        }
    }
}
