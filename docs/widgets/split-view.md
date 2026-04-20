---
title: "SplitView"
sidebar_label: "SplitView"
---

# `SplitView`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `SplitView` when this interaction is the best match for your screen workflow.

## When to use

- First multi-pane layout before advanced docking.
- You need a stable two-region composition.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var navigator = new ListView<string>();
navigator.SetItems("Orders", "Incidents", "Exports");

var detail = new MarkdownView { Markdown = "## Detail\nSelect an item." };

var split = new SplitView
{
    Orientation = SplitViewOrientation.Horizontal,
    First = navigator,
    Second = detail,
    Ratio = 0.32f
};

return Screen.Build(window => window.Body(body => body.Fill(split)));
```

## Common pitfalls

- Switch to `ResizablePaneGroup` when users need interactive pane resizing.
- Avoid deeply nested split trees; readability drops quickly.


## Public properties

| Property | Type |
| --- | --- |
| `ActivePaneIndex` | `int` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `DividerGlyph` | `char` |
| `DividerStyle` | `TesseraStyle` |
| `First` | `Control?` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedDividerStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MinFirstSize` | `int` |
| `MinSecondSize` | `int` |
| `Orientation` | `SplitViewOrientation` |
| `Padding` | `Thickness` |
| `Second` | `Control?` |
| `ShowDivider` | `bool` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
