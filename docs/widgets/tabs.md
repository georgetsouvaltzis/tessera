---
title: "Tabs"
sidebar_label: "Tabs"
---

# `Tabs`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `Tabs` when this interaction is the best match for your screen workflow.

## When to use

- Users switch between a few stable views in one region.
- You need quick mode changes without route/context loss.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var tabs = new Tabs();
tabs.SetItems("Workspace", "Inspect", "Actions");
tabs.SetSelectedIndex(0);

var content = new Label { Text = "Workspace view" };
tabs.SelectionChanged += (_, _) => content.Text = $"{tabs.SelectedItem} view";

return Screen.Build(window =>
{
    window.Body(body =>
    {
        body.Row(0.14f, tabs);
        body.Row(0.86f, content);
    });
});
```

## Common pitfalls

- Keep tab count small; move large navigation trees to `SideNavRail`.
- Preserve state per tab where possible to avoid user frustration.


## Public properties

| Property | Type |
| --- | --- |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsFocused` | `bool` |
| `SelectedIndex` | `int` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<SelectionChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
