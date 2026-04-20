---
title: "CommandOutput"
sidebar_label: "CommandOutput"
---

# `CommandOutput`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `CommandOutput` when this interaction is the best match for your screen workflow.

## When to use

- You need a `CommandOutput`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new CommandOutput
{
    Title = "CommandOutput"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `CommandOutput` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `AutoFollow` | `bool` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedLineStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredLineStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MaxLines` | `int` |
| `Padding` | `Thickness` |
| `SelectedIndex` | `int` |
| `SelectedLineStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `ShowTimestamp` | `bool` |
| `StdErrStyle` | `TesseraStyle` |
| `StdOutStyle` | `TesseraStyle` |
| `SystemStyle` | `TesseraStyle` |
| `TimestampFormat` | `string` |
| `TimestampStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<CommandOutputLine>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
