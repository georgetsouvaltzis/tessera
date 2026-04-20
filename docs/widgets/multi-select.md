---
title: "MultiSelect"
sidebar_label: "MultiSelect"
---

# `MultiSelect`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `MultiSelect` when this interaction is the best match for your screen workflow.

## When to use

- You need a `MultiSelect`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new MultiSelect
{
    Title = "MultiSelect"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `MultiSelect` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `CheckedItemStyle` | `TesseraStyle` |
| `CheckedMarker` | `string` |
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
| `UncheckedMarker` | `string` |
| `UnselectedMarker` | `string` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
