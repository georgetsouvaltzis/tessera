---
sidebar_label: Widget Reference
---

# Widget Reference

This page is the practical index for choosing widgets by product problem, with starter samples you can paste.

If you need a page per control (usage + public properties + events), go to [Widget Pages](/docs/widgets).

## Choose by task

| Task | Start here | Docs page |
| --- | --- | --- |
| Data entry and validation | `Button`, `TextInput`, `NumberInput`, `Choice`, `DataForm<TModel>` | [Inputs & Forms](/docs/widgets-inputs-and-forms) |
| Navigation and command flow | `Tabs`, `SideNavRail`, `CommandPalette`, `SearchBox` | [Navigation & Workflow](/docs/widgets-navigation-and-workflow) |
| Investigation and evidence | `DataGrid`, `InspectorPanel`, `DiffView`, `LogView` | [Data & Inspection](/docs/widgets-data-and-inspection) |
| Metrics and chart surfaces | `StatsCard`, `TelemetryChart`, `Sparkline`, `DashboardGrid` | [Dashboards & Plots](/docs/widgets-dashboards-and-plots) |
| Shell chrome and overlays | `StatusBar`, `Dialog`, `Modal`, `ResizablePaneGroup` | [Shells & Overlays](/docs/widgets-shells-and-overlays) |

## Sample: form lane

```csharp
using Tessera.Controls;
using Tessera.Layout;

var submit = new Button { Text = "Submit" };
var name = new TextInput { PlaceholderText = "Order name" };
var qty = new NumberInput { Value = 1 };

var form = new Form
{
    Title = "Create order",
    Fields =
    {
        FormField.Row("Name", name),
        FormField.Row("Quantity", qty),
        FormField.Row(string.Empty, submit)
    }
};

return Screen.Build(window => window.Body(body => body.Center(form, width: 52, height: 13)));
```

## Sample: data + inspector lane

```csharp
using Tessera.Controls;
using Tessera.Layout;

var grid = new DataGrid();
var inspector = new InspectorPanel();

return Screen.Build(window =>
{
    window.Body(body =>
    {
        body.Row(0.72f, grid);
        body.Row(0.28f, inspector);
    });
});
```

## Sample: shell + status lane

```csharp
using Tessera.Controls;
using Tessera.Layout;

var status = new StatusBar
{
    LeftText = "Ctrl+Q quit",
    RightText = "Ops shell"
};

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Fill(new LogView()));
});
```

## Which examples map to which widgets

- beginner forms: [Starter Examples](/docs/examples) -> `CounterForm`
- shell navigation: [Starter Examples](/docs/examples) -> `WorkspaceApp`
- dense records and inspectors: [Flagship Evaluation](/docs/showcase) -> `DataWorkbench`
- command-heavy review: [Flagship Evaluation](/docs/showcase) -> `GitConsole`
- dashboard density: [Flagship Evaluation](/docs/showcase) -> `OpsWatch`

## Next step

Use [Widget Pages](/docs/widgets) for control-by-control details, [Widgets Overview](/docs/controls-overview) for family-level capability mapping, then [Public API Inventory](/docs/public-api-inventory) for full surface auditing.
