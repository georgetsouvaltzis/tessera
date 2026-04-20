---
title: "TreeMapChart"
sidebar_label: "TreeMapChart"
---

# `TreeMapChart`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `TreeMapChart` when this interaction is the best match for your screen workflow.

## When to use

- You need a `TreeMapChart`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new TreeMapChart
{
    Title = "TreeMapChart"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `TreeMapChart` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledNodeStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedNodeStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HighGlyph` | `char` |
| `HighNodeStyle` | `TesseraStyle` |
| `HoveredNodeStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LabelStyle` | `TesseraStyle` |
| `LowGlyph` | `char` |
| `LowNodeStyle` | `TesseraStyle` |
| `MidGlyph` | `char` |
| `MidNodeStyle` | `TesseraStyle` |
| `NodeStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PeakGlyph` | `char` |
| `PeakNodeStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedNodeStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `ShowLabels` | `bool` |
| `ShowLegend` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<TreeMapNode?>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
