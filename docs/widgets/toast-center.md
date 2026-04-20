---
title: "ToastCenter"
sidebar_label: "ToastCenter"
---

# `ToastCenter`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `ToastCenter` when this interaction is the best match for your screen workflow.

## When to use

- You need a `ToastCenter`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new ToastCenter
{
    Title = "ToastCenter"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `ToastCenter` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `AutoDismissExpired` | `bool` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DefaultTimeout` | `TimeSpan?` |
| `ErrorItemStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredItemStyle` | `TesseraStyle` |
| `InfoItemStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `ItemStyle` | `TesseraStyle` |
| `MaxItems` | `int` |
| `MutedItemStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedItemStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `SuccessItemStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `VisibleCapacity` | `int` |
| `WarningItemStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
