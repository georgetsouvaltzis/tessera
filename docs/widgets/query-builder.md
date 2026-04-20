---
title: "QueryBuilder"
sidebar_label: "QueryBuilder"
---

# `QueryBuilder`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `QueryBuilder` when this interaction is the best match for your screen workflow.

## When to use

- You need a `QueryBuilder`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new QueryBuilder
{
    Title = "QueryBuilder"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `QueryBuilder` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledRuleStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `ErrorRuleStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedRuleStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HasError` | `bool` |
| `HoveredRuleStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Padding` | `Thickness` |
| `PreviewStyle` | `TesseraStyle` |
| `RuleStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedMarker` | `string` |
| `SelectedRuleStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `ShowQueryPreview` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |

## Public events

| Event | Type |
| --- | --- |
| `QueryChanged` | `EventHandler<QueryChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
