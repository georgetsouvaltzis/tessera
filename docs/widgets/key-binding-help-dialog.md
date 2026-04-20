---
title: "KeyBindingHelpDialog"
sidebar_label: "KeyBindingHelpDialog"
---

# `KeyBindingHelpDialog`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `KeyBindingHelpDialog` when this interaction is the best match for your screen workflow.

## When to use

- You need a `KeyBindingHelpDialog`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- The control is mainly presentational or state-driven through property updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new KeyBindingHelpDialog
{
    Title = "KeyBindingHelpDialog"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `KeyBindingHelpDialog` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `DescriptionStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `EmptyTextStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `GlobalBindingStyle` | `TesseraStyle` |
| `GroupStyle` | `TesseraStyle` |
| `HoveredRowStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `IsVisible` | `bool` |
| `KeyColumnWidth` | `int` |
| `KeysStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PageSize` | `int` |
| `SelectedMarker` | `string` |
| `SelectedRowStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `ShowGroups` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
