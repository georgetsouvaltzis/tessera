---
title: "BarChart"
sidebar_label: "BarChart"
---

# `BarChart`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `BarChart` when this interaction is the best match for your screen workflow.

## When to use

- You need a `BarChart`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new BarChart
{
    Title = "BarChart"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `BarChart` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `LabelStyle` | `TesseraStyle` |
| `LegendStyle` | `TesseraStyle` |
| `MaxValue` | `double?` |
| `Options` | `BarChartOptions?` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
