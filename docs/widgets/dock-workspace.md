---
title: "DockWorkspace"
sidebar_label: "DockWorkspace"
---

# `DockWorkspace`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `DockWorkspace` when this interaction is the best match for your screen workflow.

## When to use

- You need a `DockWorkspace`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new DockWorkspace
{
    Title = "DockWorkspace"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `DockWorkspace` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledPaneStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `EmptyTextStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedPaneBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedPaneTitleStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredPaneStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MutedPaneStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PaneBodyStyle` | `TesseraStyle` |
| `PaneBorder` | `BorderStyle` |
| `PaneBorderStyleText` | `TesseraStyle` |
| `PaneEmptyText` | `string` |
| `PanePadding` | `Thickness` |
| `PaneTitleStyle` | `TesseraStyle` |
| `SelectedPaneBodyStyle` | `TesseraStyle` |
| `SelectedPaneMarker` | `string` |
| `SelectedPaneTitleStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<DockPane>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
