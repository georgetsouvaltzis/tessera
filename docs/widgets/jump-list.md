---
title: "JumpList"
sidebar_label: "JumpList"
---

# `JumpList`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `JumpList` when this interaction is the best match for your screen workflow.

## When to use

- You need a `JumpList`-style interaction inside the navigation & workflow lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new JumpList
{
    Title = "JumpList"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `JumpList` by name only; validate it against the target workflow.
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
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedItemStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `Glyphs` | `JumpListGlyphSet` |
| `HoveredItemStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `ItemStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PinnedMarkerStyle` | `TesseraStyle` |
| `RecentMarkerStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedItemStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `Activated` | `EventHandler<JumpListActivatedEventArgs>?` |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<JumpListItem>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
