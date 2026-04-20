---
title: "StatusBar"
sidebar_label: "StatusBar"
---

# `StatusBar`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `StatusBar` when this interaction is the best match for your screen workflow.

## When to use

- Persistent shell hints, shortcuts, and state summary.
- You need low-noise feedback without modal interruptions.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var status = new StatusBar
{
    LeftText = "Orders: 127",
    RightText = "Enter refreshes   Ctrl+Q quits"
};

var body = new Label { Text = "Workspace shell" };

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(content => content.Fill(body));
});
```

## Common pitfalls

- Keep copy short; status bars are for signals, not paragraphs.
- Avoid using status bar as a replacement for validation surfaces.


## Public properties

| Property | Type |
| --- | --- |
| `Fill` | `char` |
| `FillStyle` | `TesseraStyle` |
| `LeftText` | `string` |
| `LeftTextStyle` | `TesseraStyle` |
| `RightText` | `string` |
| `RightTextStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
