---
title: "NumberInput"
sidebar_label: "NumberInput"
---

# `NumberInput`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `NumberInput` when this interaction is the best match for your screen workflow.

## When to use

- Bounded numeric values (retries, page size, thresholds).
- You need direct numeric editing without parsing text yourself.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var pageSize = new NumberInput
{
    Title = "Page size",
    MinValue = 10,
    MaxValue = 500,
    Value = 100,
    Step = 10
};

var status = new StatusBar { LeftText = "Adjust page size" };
pageSize.Submitted += (_, e) => status.LeftText = $"Page size: {e.Value}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(pageSize, width: 42, height: 3));
});
```

## Common pitfalls

- Always set domain-valid min/max limits.
- Avoid text parsing in `Update(...)` if `NumberInput` already enforces numeric shape.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledTextStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LastSubmittedValue` | `double?` |
| `Max` | `double` |
| `Min` | `double` |
| `Padding` | `Thickness` |
| `Precision` | `int` |
| `ShowFocusMarker` | `bool` |
| `Step` | `double` |
| `SummaryTextStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `Value` | `double` |
| `ValueTextStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `Submitted` | `EventHandler<NumberInputSubmittedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
