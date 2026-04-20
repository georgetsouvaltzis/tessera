---
title: "Choice"
sidebar_label: "Choice"
---

# `Choice`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `Choice` when this interaction is the best match for your screen workflow.

## When to use

- One-of-many selection with a short option list.
- You want explicit selection state in a compact footprint.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var lane = new Choice
{
    Title = "Lane",
    Items = { "Citrine", "Cobalt", "Ember" }
};

var status = new StatusBar { LeftText = "Select lane" };
lane.SelectionChanged += (_, _) => status.LeftText = $"Lane: {lane.SelectedItem}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(lane, width: 36, height: 7));
});
```

## Common pitfalls

- Prefer `ComboBox` when users need to type and narrow options.
- Keep option labels stable to avoid confusing selection persistence.


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
| `SelectedOptionStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
