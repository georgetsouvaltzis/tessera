---
title: "TextInput"
sidebar_label: "TextInput"
---

# `TextInput`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `TextInput` when this interaction is the best match for your screen workflow.

## When to use

- Single-line text entry (search, names, ids, filters).
- You need submit/cancel semantics for an inline editor.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var query = new TextInput
{
    Title = "Search",
    PlaceholderText = "order id, owner, region"
};
var status = new StatusBar { LeftText = "Type and press Enter" };

query.Submitted += (_, e) => status.LeftText = $"Applied: {e.Text}";
query.Cancelled += (_, _) => status.LeftText = "Search cleared";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(query, width: 56, height: 3));
});
```

## Common pitfalls

- Use `TextArea` for multi-line text; avoid forcing line breaks into `TextInput`.
- Treat submitted text as a message into app state, not as implicit side effect.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CancelCount` | `int` |
| `ClearOnCancel` | `bool` |
| `ClearOnSubmit` | `bool` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsFocused` | `bool` |
| `LastCancelledValue` | `string` |
| `LastSubmittedValue` | `string` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `SubmitCount` | `int` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueTextStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `Cancelled` | `EventHandler<TextInputCancelledEventArgs>?` |
| `Submitted` | `EventHandler<TextInputSubmittedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
