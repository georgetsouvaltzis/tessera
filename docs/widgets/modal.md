---
title: "Modal"
sidebar_label: "Modal"
---

# `Modal`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `Modal` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Modal`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Modal
{
    Title = "Modal"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Modal` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `BackdropFill` | `char` |
| `BodyTextStyle` | `TesseraStyle` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsVisible` | `bool` |
| `Padding` | `Thickness` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
