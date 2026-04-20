---
title: "HealthBoard"
sidebar_label: "HealthBoard"
---

# `HealthBoard`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `HealthBoard` when this interaction is the best match for your screen workflow.

## When to use

- You need a `HealthBoard`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new HealthBoard
{
    Title = "HealthBoard"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `HealthBoard` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `AcknowledgedServiceStyle` | `TesseraStyle` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DegradedServiceStyle` | `TesseraStyle` |
| `DisabledServiceStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedServiceStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `Glyphs` | `HealthBoardGlyphSet` |
| `HealthyServiceStyle` | `TesseraStyle` |
| `HoveredServiceStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MutedServiceStyle` | `TesseraStyle` |
| `OutageServiceStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedIndex` | `int` |
| `SelectedServiceStyle` | `TesseraStyle` |
| `ServiceStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<HealthService>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
