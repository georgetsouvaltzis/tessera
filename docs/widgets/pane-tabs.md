---
title: "PaneTabs"
sidebar_label: "PaneTabs"
---

# `PaneTabs`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `PaneTabs` when this interaction is the best match for your screen workflow.

## When to use

- You need a `PaneTabs`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new PaneTabs
{
    Title = "PaneTabs"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `PaneTabs` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `DisabledTabStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `EmptyTextStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedTabStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredTabStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Padding` | `Thickness` |
| `SelectedPrefix` | `string` |
| `SelectedSuffix` | `string` |
| `SelectedTabStyle` | `TesseraStyle` |
| `Separator` | `string` |
| `SeparatorStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `TabStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<PaneTabSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
