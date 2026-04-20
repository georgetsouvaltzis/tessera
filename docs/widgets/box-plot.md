---
title: "BoxPlot"
sidebar_label: "BoxPlot"
---

# `BoxPlot`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `BoxPlot` when this interaction is the best match for your screen workflow.

## When to use

- You need a `BoxPlot`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new BoxPlot
{
    Title = "BoxPlot"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `BoxPlot` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledSeriesStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedSeriesStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredSeriesStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MedianGlyph` | `char` |
| `MedianStyle` | `TesseraStyle` |
| `MutedSeriesStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `QuartileGlyph` | `char` |
| `QuartileStyle` | `TesseraStyle` |
| `SelectedMarker` | `string` |
| `SelectedSeriesIndex` | `int` |
| `SelectedSeriesStyle` | `TesseraStyle` |
| `SeriesStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |
| `WhiskerCapGlyph` | `char` |
| `WhiskerGlyph` | `char` |
| `WhiskerStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<BoxPlotSeries>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
