---
title: "ProgressBar"
sidebar_label: "ProgressBar"
---

# `ProgressBar`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `ProgressBar` when this interaction is the best match for your screen workflow.

## When to use

- You need a `ProgressBar`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new ProgressBar
{
    Title = "ProgressBar"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `ProgressBar` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `FillStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LabelStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `ShowFocusMarker` | `bool` |
| `Step` | `double` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `TrackStyle` | `TesseraStyle` |
| `Value` | `double` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
