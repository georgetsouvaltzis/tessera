---
title: "InspectorPanel"
sidebar_label: "InspectorPanel"
---

# `InspectorPanel`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `InspectorPanel` when this interaction is the best match for your screen workflow.

## When to use

- Entity details need a dedicated structured side panel.
- You need key/value inspection with section grouping.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var inspector = new InspectorPanel
{
    Title = "Record profile"
};

inspector.SetSections(
    new InspectorSection("Entity", new[]
    {
        new InspectorField("Id", "risk_10443"),
        new InspectorField("Owner", "luca ramos"),
        new InspectorField("Region", "eu-central-1")
    }));

return Screen.Build(window => window.Body(body => body.Fill(inspector)));
```

## Common pitfalls

- Do not duplicate full row content from grid; surface only decision-relevant fields.
- Keep section ordering stable to build operator muscle memory.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CollapsedMarker` | `string` |
| `DetailStyle` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `ExpandedMarker` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedRowStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `KeyStyle` | `TesseraStyle` |
| `MarkerStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PreferredKeyWidth` | `int` |
| `SectionStyle` | `TesseraStyle` |
| `SelectedRowIndex` | `int` |
| `SelectedRowStyle` | `TesseraStyle` |
| `SelectedSectionStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueStyle` | `TesseraStyle` |

## Public events

This control currently exposes no public events.


## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
