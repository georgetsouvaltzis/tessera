---
title: "RichTextView"
sidebar_label: "RichTextView"
---

# `RichTextView`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `RichTextView` when this interaction is the best match for your screen workflow.

## When to use

- You need a `RichTextView`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new RichTextView
{
    Title = "RichTextView"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `RichTextView` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmphasisStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HeadingStyle` | `TesseraStyle` |
| `InlineCodeStyle` | `TesseraStyle` |
| `ListMarkerStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `QuoteMarkerStyle` | `TesseraStyle` |
| `ScrollOffset` | `int` |
| `ShowFocusMarker` | `bool` |
| `StrongStyle` | `TesseraStyle` |
| `TextStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `Wrap` | `bool` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
