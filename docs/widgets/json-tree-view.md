---
title: "JsonTreeView"
sidebar_label: "JsonTreeView"
---

# `JsonTreeView`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `JsonTreeView` when this interaction is the best match for your screen workflow.

## When to use

- You need a `JsonTreeView`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new JsonTreeView
{
    Title = "JsonTreeView"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `JsonTreeView` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CollapsedMarker` | `string` |
| `ContainerStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `ExpandedMarker` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedRowStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredRowStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MutedStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedRowStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueMarker` | `string` |
| `ValueStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<JsonTreeSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
