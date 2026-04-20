---
title: "DiffView"
sidebar_label: "DiffView"
---

# `DiffView`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `DiffView` when this interaction is the best match for your screen workflow.

## When to use

- You need a `DiffView`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new DiffView
{
    Title = "DiffView"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `DiffView` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `AddedLineStyle` | `TesseraStyle` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HeaderStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Mode` | `DiffViewMode` |
| `Padding` | `Thickness` |
| `RemovedLineStyle` | `TesseraStyle` |
| `SelectedLineStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnchangedLineStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
