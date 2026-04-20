---
title: "Toolbar"
sidebar_label: "Toolbar"
---

# `Toolbar`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `Toolbar` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Toolbar`-style interaction inside the navigation & workflow lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Toolbar
{
    Title = "Toolbar"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Toolbar` by name only; validate it against the target workflow.
- Keep this control scoped to the navigation & workflow concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `FocusedItemStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `ItemStyle` | `TesseraStyle` |
| `SelectedItemStyle` | `TesseraStyle` |
| `Separator` | `string` |
| `SeparatorStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ToolbarSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
