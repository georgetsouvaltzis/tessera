---
title: "PropertyGrid"
sidebar_label: "PropertyGrid"
---

# `PropertyGrid`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `PropertyGrid` when this interaction is the best match for your screen workflow.

## When to use

- You need a `PropertyGrid`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new PropertyGrid
{
    Title = "PropertyGrid"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `PropertyGrid` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
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
| `HeaderKeyText` | `string` |
| `HeaderStyle` | `TesseraStyle` |
| `HeaderValueText` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `KeyStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PreferredKeyColumnWidth` | `int` |
| `SelectedIndex` | `int` |
| `SelectedMarker` | `string` |
| `SelectedRowStyle` | `TesseraStyle` |
| `ShowCategoryHeaders` | `bool` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |
| `ValueStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<PropertyGridSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
