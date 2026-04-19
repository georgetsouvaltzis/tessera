---
sidebar_label: "Recipes: Effects & Refresh"
---

# Recipes: Effects And Refresh

Use these patterns when the screen needs timed updates, notifications, or explicit message flow.

## Recipe: button event -> domain message -> `Update(...)`

```csharp
private sealed record RefreshRequested : Message;

internal sealed class OrdersApp : TesseraApp
{
    private readonly Button _refresh = new() { Text = "Refresh" };

    public OrdersApp()
    {
        _refresh.Activated += (_, _) => Post(new RefreshRequested());
    }

    public override TesseraEffect? Update(Message message)
    {
        return message switch
        {
            RefreshRequested => RefreshOrders(),
            KeyPressed key when key.IsCharacter('q', ModifierKeys.Ctrl) => TesseraEffects.Quit,
            _ => null
        };
    }

    public override Screen Build(ScreenContext context) => Screen.From(_refresh);

    private TesseraEffect? RefreshOrders()
    {
        // Apply state changes here.
        return null;
    }
}
```

Use this once widget events start changing real domain state.

## Recipe: periodic refresh without self-rescheduling noise

```csharp
private sealed record TickMessage : Message;

public override TesseraEffect? Initialize()
    => TesseraEffects.Periodic(TimeSpan.FromSeconds(5), _ => new TickMessage());

public override TesseraEffect? Update(Message message)
{
    return message switch
    {
        TickMessage => RefreshOrders(),
        _ => null
    };
}
```

`TesseraEffects.Periodic(...)` is the cleanest public path when the app needs a recurring tick.

## Recipe: add a notification feed

```csharp
private readonly Notifications _notifications = new()
{
    Title = "Activity"
};

private TesseraEffect? RefreshOrders()
{
    _notifications.Push("Orders refreshed");
    return null;
}
```

Use `Notifications` for the default visible feed. Use `NotificationInbox` later when the app needs a denser persistent inbox workflow.

## Recipe: put the notification feed into a real shell

```csharp
public override Screen Build(ScreenContext context)
{
    return Screen.Build(window =>
    {
        window.Padding(1);
        window.Right(36, _notifications);
        window.Body(body => body.Use(_refresh));
    });
}
```

## Related pages

- app model: [app-model](/docs/app-model)
- widgets overview: [controls-overview](/docs/controls-overview)
- shells and overlays widgets: [widgets-shells-and-overlays](/docs/widgets-shells-and-overlays)
