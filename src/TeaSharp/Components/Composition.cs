using TeaSharp.Core.Abstractions;

namespace TeaSharp.Components;

public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}

public interface IStatefulComponent : ICanvasComponent
{
    bool Update(IMessage message);
}

public readonly record struct ComponentSlot(ICanvasComponent Component, Rect Bounds);

public sealed class ComponentComposer
{
    private readonly List<ComponentSlot> _slots = [];

    public IReadOnlyList<ComponentSlot> Slots => _slots;

    public void Clear()
    {
        _slots.Clear();
    }

    public void Add(ICanvasComponent component, Rect bounds)
    {
        _slots.Add(new ComponentSlot(component, bounds));
    }

    public bool Update(IMessage message)
    {
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

    public void Render(Canvas canvas)
    {
        foreach (var slot in _slots)
        {
            slot.Component.Render(canvas, slot.Bounds);
        }
    }
}
