---
title: "Form"
sidebar_label: "Form"
---

# `Form`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `Form` when this interaction is the best match for your screen workflow.

## When to use

- You want explicit rows with labels and controls.
- Validation/readability matter more than dense freeform layout.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var name = new TextInput { PlaceholderText = "Order name" };
var qty = new NumberInput { Value = 1, MinValue = 1, MaxValue = 999 };
var submit = new Button { Text = "Create order" };

var form = new Form
{
    Title = "Create order",
    Fields =
    {
        FormField.Row("Name", name),
        FormField.Row("Quantity", qty),
        FormField.Row(string.Empty, submit)
    }
};

return Screen.Build(window => window.Body(body => body.Center(form, width: 56, height: 13)));
```

## Common pitfalls

- Use `DataForm<TModel>` when you need model-bound field registration.
- Avoid deeply nested forms; split into sections with `FieldSet`.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `DisabledStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedSelectedRowStyle` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredRowStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `LabelStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `RequiredMarker` | `string` |
| `RequiredMarkerStyle` | `TesseraStyle` |
| `SelectedIndex` | `int` |
| `SelectedMarker` | `string` |
| `SelectedRowStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `Title` | `string` |
| `TitlePrefix` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `TitleSuffix` | `string` |
| `UnselectedMarker` | `string` |
| `ValueStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<ListSelectionChangedEventArgs<FormField>>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
