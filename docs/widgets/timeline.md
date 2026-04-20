---
title: "Timeline"
sidebar_label: "Timeline"
---

# `Timeline`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `Timeline` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Timeline`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Timeline
{
    Title = "Timeline"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Timeline` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `ContentStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LabelStyle` | `TesseraStyle` |
| `MutedStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PageSize` | `int` |
| `SelectedRowStyle` | `TesseraStyle` |
| `Separator` | `string` |
| `SeparatorStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `TimestampStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<TimelineSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
