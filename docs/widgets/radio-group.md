---
title: "RadioGroup"
sidebar_label: "RadioGroup"
---

# `RadioGroup`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `RadioGroup` when this interaction is the best match for your screen workflow.

## When to use

- You need a `RadioGroup`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new RadioGroup
{
    Title = "RadioGroup"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `RadioGroup` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `DisabledItemStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredItemStyle` | `TesseraStyle` |
| `ItemStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedItemStyle` | `TesseraStyle` |
| `SelectedMarker` | `string` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
