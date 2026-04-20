---
title: "Heatmap"
sidebar_label: "Heatmap"
---

# `Heatmap`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `Heatmap` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Heatmap`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Heatmap
{
    Title = "Heatmap"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Heatmap` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CellStyle` | `TesseraStyle` |
| `ColumnCount` | `int` |
| `DisabledCellStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedCellStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HeaderStyle` | `TesseraStyle` |
| `HighCellStyle` | `TesseraStyle` |
| `HighGlyph` | `char` |
| `HoveredCellStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LegendStyle` | `TesseraStyle` |
| `LowCellStyle` | `TesseraStyle` |
| `LowGlyph` | `char` |
| `MidCellStyle` | `TesseraStyle` |
| `MidGlyph` | `char` |
| `Padding` | `Thickness` |
| `PeakCellStyle` | `TesseraStyle` |
| `PeakGlyph` | `char` |
| `RowCount` | `int` |
| `SelectedCellStyle` | `TesseraStyle` |
| `SelectedColumn` | `int` |
| `SelectedRow` | `int` |
| `ShowColumnLabels` | `bool` |
| `ShowFocusMarker` | `bool` |
| `ShowLegend` | `bool` |
| `ShowRowLabels` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<HeatmapCell?>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
