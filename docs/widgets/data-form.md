---
title: "DataForm<TModel>"
sidebar_label: "DataForm<TModel>"
---

# `DataForm<TModel>`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `DataForm<TModel>` when this interaction is the best match for your screen workflow.

## When to use

- You need a `DataForm`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new DataForm<TModel>
{
    Title = "DataForm"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `DataForm` by name only; validate it against the target workflow.
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
| `EditBuffer` | `string` |
| `EmptyText` | `string` |
| `ErrorStyle` | `TesseraStyle` |
| `FieldSeparatorText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedFieldStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredFieldStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsEditing` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LabelStyle` | `TesseraStyle` |
| `LastCommitError` | `string` |
| `MaxLabelWidth` | `int` |
| `Model` | `TModel?` |
| `NoModelText` | `string` |
| `Padding` | `Thickness` |
| `PlaceholderStyle` | `TesseraStyle` |
| `ReadOnlyFieldStyle` | `TesseraStyle` |
| `SelectedFieldStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedMarker` | `string` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |
| `ValueStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `FieldCommitted` | `EventHandler<DataFormFieldCommittedEventArgs<TModel>>?` |
| `SelectionChanged` | `EventHandler<DataFormSelectionChangedEventArgs<TModel>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
