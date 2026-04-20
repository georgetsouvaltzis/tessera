---
title: "LogView"
sidebar_label: "LogView"
---

# `LogView`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `LogView` when this interaction is the best match for your screen workflow.

## When to use

- You need a `LogView`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new LogView
{
    Title = "LogView"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `LogView` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `AutoScroll` | `bool` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `EntryStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsFocused` | `bool` |
| `IsPaused` | `bool` |
| `Padding` | `Thickness` |
| `PausedTitleStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
