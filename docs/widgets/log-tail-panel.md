---
title: "LogTailPanel"
sidebar_label: "LogTailPanel"
---

# `LogTailPanel`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `LogTailPanel` when this interaction is the best match for your screen workflow.

## When to use

- You need a `LogTailPanel`-style interaction inside the data & inspection lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new LogTailPanel
{
    Title = "LogTailPanel"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `LogTailPanel` by name only; validate it against the target workflow.
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
| `CriticalEntryStyle` | `TesseraStyle` |
| `DebugEntryStyle` | `TesseraStyle` |
| `DisabledEntryStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `EntryStyle` | `TesseraStyle` |
| `ErrorEntryStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedEntryStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HasError` | `bool` |
| `HoveredEntryStyle` | `TesseraStyle` |
| `InfoEntryStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MaxEntries` | `int` |
| `MutedEntryStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedEntryStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedMarker` | `string` |
| `ShowFocusMarker` | `bool` |
| `ShowLevel` | `bool` |
| `ShowSource` | `bool` |
| `ShowTimestamp` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `TraceEntryStyle` | `TesseraStyle` |
| `UnselectedMarker` | `string` |
| `WarningEntryStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
