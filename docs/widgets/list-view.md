---
title: "ListView<T>"
sidebar_label: "ListView<T>"
---

# `ListView<T>`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `ListView<T>` when this interaction is the best match for your screen workflow.

## When to use

- Straightforward item browsing with low complexity.
- You need selection-driven detail panels.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var list = new ListView<string>();
list.SetItems("Orders", "Incidents", "Telemetry", "Exports");

var status = new StatusBar { LeftText = "Select a section" };
list.SelectionChanged += (_, e) => status.LeftText = $"Selected: {e.SelectedItem}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Fill(list));
});
```

## Common pitfalls

- Use `VirtualizedListView<T>` for very large collections.
- Do not overload each row with excessive formatting noise.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DefaultRowStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredRowStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Padding` | `Thickness` |
| `RowMarkers` | `ListViewMarkerSet` |
| `SelectedRowStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<T>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
