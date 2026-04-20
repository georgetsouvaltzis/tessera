---
title: "Badge"
sidebar_label: "Badge"
---

# `Badge`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `Badge` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Badge`-style interaction inside the dashboards & plots lane.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Badge
{
    Text = "Badge"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Badge` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `ErrorTextStyle` | `TesseraStyle` |
| `FocusedTextStyle` | `TesseraStyle` |
| `ShowBrackets` | `bool` |
| `SuccessTextStyle` | `TesseraStyle` |
| `Text` | `string` |
| `TextStyle` | `TesseraStyle` |
| `Tone` | `BadgeTone` |
| `WarningTextStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
