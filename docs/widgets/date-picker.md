---
title: "DatePicker"
sidebar_label: "DatePicker"
---

# `DatePicker`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `DatePicker` when this interaction is the best match for your screen workflow.

## When to use

- You need a `DatePicker`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new DatePicker
{
    Title = "DatePicker"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `DatePicker` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CurrentMonth` | `DateOnly` |
| `DayStyle` | `TesseraStyle` |
| `DisabledDayStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredDayStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LastCommittedDate` | `DateOnly?` |
| `MonthHeaderStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedDate` | `DateOnly` |
| `SelectedDayStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `WeekdayHeaderStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `DateChanged` | `EventHandler<DateChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
