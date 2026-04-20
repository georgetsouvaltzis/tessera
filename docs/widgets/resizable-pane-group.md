---
title: "ResizablePaneGroup"
sidebar_label: "ResizablePaneGroup"
---

# `ResizablePaneGroup`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `ResizablePaneGroup` when this interaction is the best match for your screen workflow.

## When to use

- You need a `ResizablePaneGroup`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new ResizablePaneGroup
{
    Title = "ResizablePaneGroup"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `ResizablePaneGroup` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyleText` | `TesseraStyle` |
| `DividerGlyph` | `char` |
| `DividerStyleText` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedDividerStyleText` | `TesseraStyle` |
| `FocusedTitleStyleText` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Padding` | `Thickness` |
| `PaneStyleText` | `TesseraStyle` |
| `SelectedPaneStyleText` | `TesseraStyle` |
| `ShowDividers` | `bool` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyleText` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<PaneSpec>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
