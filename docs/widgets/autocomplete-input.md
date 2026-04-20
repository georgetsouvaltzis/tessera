---
title: "AutocompleteInput"
sidebar_label: "AutocompleteInput"
---

# `AutocompleteInput`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `AutocompleteInput` when this interaction is the best match for your screen workflow.

## When to use

- You need a `AutocompleteInput`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new AutocompleteInput
{
    Title = "AutocompleteInput"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `AutocompleteInput` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CommitMarkerStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedSuggestionStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `Glyphs` | `AutocompleteInputGlyphSet` |
| `HoveredSuggestionStyle` | `TesseraStyle` |
| `InputTextStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MaxVisibleSuggestions` | `int` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `PopupStyle` | `TesseraStyle` |
| `SelectedSuggestionIndex` | `int` |
| `SelectedSuggestionStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `SuggestionStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SuggestionCommitted` | `EventHandler<AutocompleteInputSuggestionCommittedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
