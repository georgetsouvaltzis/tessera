---
title: "EmptyState"
sidebar_label: "EmptyState"
---

# `EmptyState`

**Family:** Shells & Overlays  
**Namespace:** `Tessera.Controls`

Use `EmptyState` when this interaction is the best match for your screen workflow.

## When to use

- You need a `EmptyState`-style interaction inside the shells & overlays lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new EmptyState
{
    Title = "EmptyState"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `EmptyState` by name only; validate it against the target workflow.
- Keep this control scoped to the shells & overlays concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.


## Public properties

| Property | Type |
| --- | --- |
| `ActionStyle` | `TesseraStyle` |
| `ActionText` | `string` |
| `DefaultStyle` | `TesseraStyle` |
| `Description` | `string` |
| `DescriptionStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `FocusedActionStyle` | `TesseraStyle` |
| `FocusedStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `Hint` | `string` |
| `HintStyle` | `TesseraStyle` |
| `HoveredActionStyle` | `TesseraStyle` |
| `HoveredStyle` | `TesseraStyle` |
| `IsHovered` | `bool` |
| `ShowAction` | `bool` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `ActionInvoked` | `EventHandler?` |
| `Activated` | `EventHandler?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
