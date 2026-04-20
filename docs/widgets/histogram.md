---
title: "Histogram"
sidebar_label: "Histogram"
---

# `Histogram`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `Histogram` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Histogram`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Histogram
{
    Title = "Histogram"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Histogram` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `AxisStyle` | `TesseraStyle` |
| `BarStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `LabelStyle` | `TesseraStyle` |
| `LegendStyle` | `TesseraStyle` |
| `MaxValue` | `double?` |
| `Options` | `HistogramOptions?` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
