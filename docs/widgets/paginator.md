---
title: "Paginator"
sidebar_label: "Paginator"
---

# `Paginator`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `Paginator` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Paginator`-style interaction inside the navigation & workflow lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Paginator
{
    Title = "Paginator"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Paginator` by name only; validate it against the target workflow.
- Keep this control scoped to the navigation & workflow concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `ActivePageLabelStyle` | `TesseraStyle` |
| `DisabledNavigationLabelStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LabelStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `PageChanged` | `EventHandler<PageChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
