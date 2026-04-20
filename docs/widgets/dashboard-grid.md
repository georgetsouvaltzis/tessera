---
title: "DashboardGrid"
sidebar_label: "DashboardGrid"
---

# `DashboardGrid`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `DashboardGrid` when this interaction is the best match for your screen workflow.

## When to use

- You need a `DashboardGrid`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new DashboardGrid
{
    Title = "DashboardGrid"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `DashboardGrid` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledTileStyleText` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyleText` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredTileStyleText` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Padding` | `Thickness` |
| `SelectedTileStyleText` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `TileBorder` | `BorderStyle` |
| `TileStyleText` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyleText` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<DashboardTile>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
