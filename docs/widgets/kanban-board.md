---
title: "KanbanBoard"
sidebar_label: "KanbanBoard"
---

# `KanbanBoard`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `KanbanBoard` when this interaction is the best match for your screen workflow.

## When to use

- You need a `KanbanBoard`-style interaction inside the navigation & workflow lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new KanbanBoard
{
    Title = "KanbanBoard"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `KanbanBoard` by name only; validate it against the target workflow.
- Keep this control scoped to the navigation & workflow concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CardStyle` | `TesseraStyle` |
| `DisabledCardStyle` | `TesseraStyle` |
| `ErrorCardStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedCardStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HasError` | `bool` |
| `HoveredCardStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LaneHeaderStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedCardIndex` | `int` |
| `SelectedCardMarker` | `string` |
| `SelectedCardStyle` | `TesseraStyle` |
| `SelectedLaneHeaderStyle` | `TesseraStyle` |
| `SelectedLaneIndex` | `int` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedCardMarker` | `string` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<KanbanSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
