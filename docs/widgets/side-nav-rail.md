---
title: "SideNavRail"
sidebar_label: "SideNavRail"
---

# `SideNavRail`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `SideNavRail` when this interaction is the best match for your screen workflow.

## When to use

- Primary shell navigation lives in a left rail.
- You need icon/badge selection with explicit activation.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var rail = new SideNavRail();
rail.SetItems(
    new NavItem("workspace", "Workspace"),
    new NavItem("inspect", "Inspect"),
    new NavItem("actions", "Actions"));

var content = new Label { Text = "Workspace" };
rail.Activated += (_, e) => content.Text = $"Opened: {e.Item.Label}";

return Screen.Build(window =>
{
    window.Body(body =>
    {
        body.Row(0.22f, rail);
        body.Row(0.78f, content);
    });
});
```

## Common pitfalls

- Use concise labels; long labels reduce scanability.
- Keep selected and activated behaviors consistent.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledItemStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedItemStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `Glyphs` | `SideNavRailGlyphSet` |
| `HoveredItemStyle` | `TesseraStyle` |
| `IsCollapsed` | `bool` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `ItemStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `SelectedIndex` | `int` |
| `SelectedItemStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `Activated` | `EventHandler<SideNavRailActivatedEventArgs>?` |
| `SelectionChanged` | `EventHandler<SideNavRailSelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
