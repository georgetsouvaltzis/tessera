---
title: "Dialog"
sidebar_label: "Dialog"
---

# `Dialog`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `Dialog` when this interaction is the best match for your screen workflow.

## When to use

- User must confirm/cancel a disruptive action.
- You need a focused transient decision surface.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var dialog = new Dialog
{
    Title = "Delete draft?",
    Message = "This action cannot be undone.",
    IsOpen = true
};

var status = new StatusBar { LeftText = "Awaiting decision" };
dialog.Closed += (_, e) => status.LeftText = $"Dialog result: {e.Result}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(dialog, width: 54, height: 10));
});
```

## Common pitfalls

- Do not stack many dialogs; resolve one decision at a time.
- Always provide an explicit cancel path.


## Public properties

| Property | Type |
| --- | --- |
| `BodyTextStyle` | `TesseraStyle` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsFocused` | `bool` |
| `IsVisible` | `bool` |
| `LastResult` | `DialogResult` |
| `Padding` | `Thickness` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `Accepted` | `EventHandler?` |
| `Closed` | `EventHandler<DialogClosedEventArgs>?` |
| `Dismissed` | `EventHandler?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
