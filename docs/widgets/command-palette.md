---
title: "CommandPalette"
sidebar_label: "CommandPalette"
---

# `CommandPalette`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `CommandPalette` when this interaction is the best match for your screen workflow.

## When to use

- Global command launch from keyboard-first workflows.
- Power users need fast action search and execution.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var palette = new CommandPalette
{
    Title = "Command Palette",
    IsOpen = true
};

palette.SetItems(
    new CommandPaletteItem("open-workspace", "Open workspace"),
    new CommandPaletteItem("export-csv", "Export CSV"),
    new CommandPaletteItem("restart-feed", "Restart feed"));

var status = new StatusBar { LeftText = "Type to search commands" };
palette.ItemExecuted += (_, e) => status.LeftText = $"Executed: {e.Item.Label}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(palette, width: 72, height: 16));
});
```

## Common pitfalls

- Keep command names action-first (`Open`, `Run`, `Export`).
- Do not overload palette with low-value commands; keep it task-centric.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledItemStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredItemStyle` | `TesseraStyle` |
| `IsFocused` | `bool` |
| `IsVisible` | `bool` |
| `ItemStyle` | `TesseraStyle` |
| `LastExecutedItemId` | `string?` |
| `MaxVisibleItems` | `int` |
| `MutedItemStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `QueryTextStyle` | `TesseraStyle` |
| `SelectedItemStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `ItemExecuted` | `EventHandler<CommandPaletteItemExecutedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
