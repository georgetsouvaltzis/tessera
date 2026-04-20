---
title: "TagInput"
sidebar_label: "TagInput"
---

# `TagInput`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `TagInput` when this interaction is the best match for your screen workflow.

## When to use

- You need a `TagInput`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new TagInput
{
    Title = "TagInput"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `TagInput` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CaretGlyph` | `string` |
| `CaretStyle` | `TesseraStyle` |
| `DisabledTagStyle` | `TesseraStyle` |
| `ErrorTagStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTagStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HasError` | `bool` |
| `HoveredTagStyle` | `TesseraStyle` |
| `InputPadding` | `int` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Options` | `TagInputOptions` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `SelectedTagIndex` | `int` |
| `SelectedTagStyle` | `TesseraStyle` |
| `ShowCaret` | `bool` |
| `ShowFocusMarker` | `bool` |
| `TagPadding` | `int` |
| `Tags` | `List<TagPlacement>` |
| `TagStyle` | `TesseraStyle` |
| `TextRuns` | `List<TextPlacement>` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueTextStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `TagsChanged` | `EventHandler<TagInputTagsChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
