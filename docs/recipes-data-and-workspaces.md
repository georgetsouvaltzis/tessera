---
sidebar_label: "Recipes: Data & Workspaces"
---

# Recipes: Data And Workspaces

Use these patterns when the app stops being a single centered control and starts looking like real software.

## Recipe: rail + grid + inspector

```csharp
private readonly SideNavRail _sources = new()
{
    Title = "Sources"
};

private readonly DataGrid _grid = new()
{
    Title = "Records"
};

private readonly InspectorPanel _inspector = new()
{
    Title = "Inspector"
};

private readonly StatusBar _status = new();

public OrdersApp()
{
    _sources.SetItems([
        new NavItem("orders", "Orders", "O"),
        new NavItem("alerts", "Alerts", "A", "3")
    ]);

    _grid.SetColumns([
        new DataGridColumn("id", "Id"),
        new DataGridColumn("state", "State"),
        new DataGridColumn("owner", "Owner")
    ]);

    _grid.SetRows([
        ["ord-1044", "Queued", "atlas"],
        ["ord-1045", "Packed", "citrine"]
    ]);

    var section = new InspectorSection("Selected order");
    section.AddField("Id", "ord-1044");
    section.AddField("State", "Queued");
    section.AddDetail("Update this section when the grid selection changes.");
    _inspector.SetSections([section]);
}

public override Screen Build(ScreenContext context)
{
    _status.LeftText = "Orders";
    _status.RightText = "Rail / Grid / Inspector";

    return Screen.Build(window =>
    {
        window.Padding(1);
        window.Left(24, _sources);
        window.Right(32, _inspector);
        window.Body(body => body.Use(_grid));
        window.Footer(1, _status);
    });
}
```

This is the cleanest public path for record-heavy shells before you need pane docking.

## Recipe: compact metrics card

```csharp
private readonly StatsCard _metrics = new()
{
    Title = "Orders"
};

public OrdersApp()
{
    _metrics.SetValue("Queued", "12");
    _metrics.SetValue("Packed", "08");
    _metrics.SetValue("Errored", "01");
}
```

Use `StatsCard` for compact, readable metrics before you move into larger dashboard grids.

## Recipe: when to graduate to workspace widgets

Move beyond the simple rail/grid/inspector shell when you need:

- resizable panes -> `ResizablePaneGroup`
- docked workspace regions -> `DockWorkspace`
- tabbed subpanes -> `PaneTabs`
- heavier transient overlays -> `Dialog`, `Modal`, `ContextMenu`

Those widgets belong in [widgets-shells-and-overlays](/docs/widgets-shells-and-overlays).

## Related pages

- widgets: [widgets-data-and-inspection](/docs/widgets-data-and-inspection)
- widgets: [widgets-shells-and-overlays](/docs/widgets-shells-and-overlays)
- showcase: [showcase](/docs/showcase)
