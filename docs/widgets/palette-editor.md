---
title: "PaletteEditor"
sidebar_label: "PaletteEditor"
---

# `PaletteEditor`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `PaletteEditor` when this interaction is the best match for your screen workflow.

## When to use

- You need a `PaletteEditor`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new PaletteEditor
{
    Title = "PaletteEditor"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `PaletteEditor` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `ColumnCount` | `int` |
| `DisabledSwatchStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `EmptyTextStyle` | `TesseraStyle` |
| `FocusedSelectedSwatchStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredSwatchStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MutedSwatchStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PreviewGlyph` | `string` |
| `PreviewSwatchStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedMarker` | `string` |
| `SelectedSwatchStyle` | `TesseraStyle` |
| `ShowDescription` | `bool` |
| `ShowFocusMarker` | `bool` |
| `ShowHexCode` | `bool` |
| `ShowPreviewBlock` | `bool` |
| `SwatchStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<PaletteSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
