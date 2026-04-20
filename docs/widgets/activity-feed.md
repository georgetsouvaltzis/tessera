---
title: "ActivityFeed"
sidebar_label: "ActivityFeed"
---

# `ActivityFeed`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `ActivityFeed` when this interaction is the best match for your screen workflow.

## When to use

- You need a `ActivityFeed`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new ActivityFeed
{
    Title = "ActivityFeed"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `ActivityFeed` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `AutoFollow` | `bool` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledItemStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `ErrorItemStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedItemStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredItemStyle` | `TesseraStyle` |
| `InfoItemStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MaxItems` | `int` |
| `MutedItemStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedIndex` | `int` |
| `SelectedItemStyle` | `TesseraStyle` |
| `SelectedMarker` | `string` |
| `ShowFocusMarker` | `bool` |
| `ShowTimestamp` | `bool` |
| `SuccessItemStyle` | `TesseraStyle` |
| `TimestampFormat` | `string` |
| `TimestampStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnreadItemStyle` | `TesseraStyle` |
| `UnreadMarker` | `string` |
| `UnselectedMarker` | `string` |
| `WarningItemStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<ActivityFeedItem>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
