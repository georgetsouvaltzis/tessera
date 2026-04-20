---
title: "CalendarMonthView"
sidebar_label: "CalendarMonthView"
---

# `CalendarMonthView`

**Family:** Dashboards & Plots  
**Namespace:** `Tessera.Controls`

Use `CalendarMonthView` when this interaction is the best match for your screen workflow.

## When to use

- You need a `CalendarMonthView`-style interaction inside the dashboards & plots lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new CalendarMonthView
{
    Title = "CalendarMonthView"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `CalendarMonthView` by name only; validate it against the target workflow.
- Keep this control scoped to the dashboards & plots concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `DayStyle` | `TesseraStyle` |
| `DisabledDayStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `DisplayMonth` | `DateOnly` |
| `FirstDayOfWeek` | `DayOfWeek` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredDayStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MaxDate` | `DateOnly?` |
| `MinDate` | `DateOnly?` |
| `MonthHeaderStyle` | `TesseraStyle` |
| `OutsideMonthDayStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedDate` | `DateOnly` |
| `SelectedDayStyle` | `TesseraStyle` |
| `ShowAdjacentMonthDays` | `bool` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `Today` | `DateOnly` |
| `TodayDayStyle` | `TesseraStyle` |
| `WeekdayHeaderStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `DateSelected` | `EventHandler<CalendarDateSelectedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
