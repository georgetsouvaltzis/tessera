---
title: "TerminalPanel"
sidebar_label: "TerminalPanel"
---

# `TerminalPanel`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `TerminalPanel` when this interaction is the best match for your screen workflow.

## When to use

- You need a `TerminalPanel`-style interaction inside the data & inspection lane.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new TerminalPanel
{
    // Configure properties here
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `TerminalPanel` by name only; validate it against the target workflow.
- Keep this control scoped to the data & inspection concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `CommandStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedSelectedLineStyle` | `TesseraStyle` |
| `FollowTail` | `bool` |
| `HoveredLineStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MarkerStyle` | `TesseraStyle` |
| `MaxLines` | `int` |
| `Padding` | `Thickness` |
| `SelectedIndex` | `int` |
| `SelectedLineStyle` | `TesseraStyle` |
| `ShowLineNumbers` | `bool` |
| `StandardErrorStyle` | `TesseraStyle` |
| `StandardOutputStyle` | `TesseraStyle` |
| `SystemStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<TerminalPanelLine>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
