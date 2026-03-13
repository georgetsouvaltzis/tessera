using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

public sealed class Notifications : Control
{
    private readonly NotificationCenterComponent _component = new();

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    public int MaxItems
    {
        get => _component.MaxEntries;
        set => _component.MaxEntries = value;
    }

    public bool ShowTimestamp
    {
        get => _component.ShowTimestamp;
        set => _component.ShowTimestamp = value;
    }

    public int Count => _component.Entries.Count;

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

    public void Push(string message, NotificationLevel level = NotificationLevel.Info, string? id = null)
    {
        _component.Push(
            message ?? string.Empty,
            level switch
            {
                NotificationLevel.Success => NotificationSeverity.Success,
                NotificationLevel.Warning => NotificationSeverity.Warning,
                NotificationLevel.Error => NotificationSeverity.Error,
                _ => NotificationSeverity.Info,
            },
            id);
    }

    public void Clear() => _component.Clear();

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
