---
title: "MenuBar"
sidebar_label: "MenuBar"
---

# `MenuBar`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `MenuBar` when this interaction is the best match for your screen workflow.

## When to use

- You need a `MenuBar`-style interaction inside the navigation & workflow lane.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new MenuBar
{
    // Configure properties here
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `MenuBar` by name only; validate it against the target workflow.
- Keep this control scoped to the navigation & workflow concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledItemStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedItemStyle` | `TesseraStyle` |
| `Glyphs` | `MenuBarGlyphSet` |
| `HoveredItemStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `ItemStyle` | `TesseraStyle` |
| `LastActivatedItemId` | `string?` |
| `Padding` | `Thickness` |
| `SelectedIndex` | `int` |
| `SelectedItemStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `ItemActivated` | `EventHandler<MenuItemActivatedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
