---
title: "ComboBox"
sidebar_label: "ComboBox"
---

# `ComboBox`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `ComboBox` when this interaction is the best match for your screen workflow.

## When to use

- You need a `ComboBox`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new ComboBox
{
    Title = "ComboBox"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `ComboBox` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `Glyphs` | `DropdownGlyphSet` |
| `HoveredOptionStyle` | `TesseraStyle` |
| `HoveredValueStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsOpen` | `bool` |
| `IsReadOnly` | `bool` |
| `MaxVisibleItems` | `int` |
| `MutedStyle` | `TesseraStyle` |
| `OptionStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `SelectedOptionStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueTextStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
