---
title: "FuzzyFinder"
sidebar_label: "FuzzyFinder"
---

# `FuzzyFinder`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `FuzzyFinder` when this interaction is the best match for your screen workflow.

## When to use

- You need a `FuzzyFinder`-style interaction inside the navigation & workflow lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new FuzzyFinder
{
    Title = "FuzzyFinder"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `FuzzyFinder` by name only; validate it against the target workflow.
- Keep this control scoped to the navigation & workflow concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredItemStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsOpen` | `bool` |
| `IsReadOnly` | `bool` |
| `LastSelectedItemId` | `string?` |
| `ListItemStyle` | `TesseraStyle` |
| `MatchHighlightStyle` | `TesseraStyle` |
| `MaxVisibleResults` | `int` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `SelectedItemStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueTextStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `ItemSelected` | `EventHandler<FuzzyFinderItemSelectedEventArgs>?` |
| `SelectionChanged` | `EventHandler<FuzzyFinderSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
