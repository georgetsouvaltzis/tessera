namespace TeaSharp.Components;

public sealed class FocusGroup<TFocus>
    where TFocus : struct, Enum
{
    private readonly List<(TFocus Key, IFocusableComponent Component)> _registrations = [];

    public FocusGroup<TFocus> Register(TFocus key, IFocusableComponent component)
    {
        _registrations.Add((key, component));
        return this;
    }

    public FocusGroup<TFocus> Clear()
    {
        _registrations.Clear();
        return this;
    }

    public bool Apply(TFocus active)
    {
        var changed = false;
        foreach (var registration in _registrations)
        {
            var shouldFocus = EqualityComparer<TFocus>.Default.Equals(registration.Key, active);
            if (registration.Component.Focused == shouldFocus)
            {
                continue;
            }

            registration.Component.Focused = shouldFocus;
            changed = true;
        }

        return changed;
    }

    public bool UnfocusAll()
    {
        var changed = false;
        foreach (var registration in _registrations)
        {
            if (!registration.Component.Focused)
            {
                continue;
            }

            registration.Component.Focused = false;
            changed = true;
        }

        return changed;
    }
}
