---
title: "Button"
sidebar_label: "Button"
---

# `Button`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `Button` when this interaction is the best match for your screen workflow.

## When to use

- A primary action should be explicit and keyboard/pointer reachable.
- You need a low-friction call-to-action in a form or shell footer.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var refresh = new Button { Text = "Refresh orders" };
var status = new StatusBar { LeftText = "Ready" };
var count = 12;

refresh.Activated += (_, _) =>
{
    count++;
    status.LeftText = $"Orders: {count}";
};

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(refresh, width: 24, height: 3));
});
```

## Common pitfalls

- Avoid using Button for passive labels; use `Label` or `Badge` instead.
- Do not hide business state changes in render-only code paths.


## Public properties

| Property | Type |
| --- | --- |
| `ActivationCount` | `int` |
| `Description` | `string?` |
| `DisabledLabelStyle` | `TesseraStyle` |
| `FocusedLabelStyle` | `TesseraStyle` |
| `FocusedSurfaceStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsPressed` | `bool` |
| `LabelPrefix` | `string` |
| `LabelStyle` | `TesseraStyle` |
| `LabelSuffix` | `string` |
| `Padding` | `Thickness` |
| `PressedLabelStyle` | `TesseraStyle` |
| `PressedSurfaceStyle` | `TesseraStyle` |
| `SurfaceStyle` | `TesseraStyle` |
| `Text` | `string` |

## Public events

| Event | Type |
| --- | --- |
| `Activated` | `EventHandler?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
