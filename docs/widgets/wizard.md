---
title: "Wizard"
sidebar_label: "Wizard"
---

# `Wizard`

**Family:** Inputs & Forms  
**Namespace:** `Tessera.Controls`

Use `Wizard` when this interaction is the best match for your screen workflow.

## When to use

- You need a `Wizard`-style interaction inside the inputs & forms lane.
- A titled widget surface improves scanability in dense shells.
- You want explicit user-driven events routed into app state updates.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var widget = new Wizard
{
    Title = "Wizard"
};

return Screen.Build(window => window.Body(body => body.Center(widget, width: 44, height: 9)));
```

## Common pitfalls

- Do not choose `Wizard` by name only; validate it against the target workflow.
- Keep this control scoped to the inputs & forms concern; avoid cross-layer state coupling.
- Handle control events by posting/processing messages; avoid hidden mutation in render paths.
- Set focused/normal styles intentionally so keyboard focus remains obvious.
- Keep disabled state explicit and reversible so users understand why actions are blocked.


## Public properties

| Property | Type |
| --- | --- |
| `ActiveMarker` | `string` |
| `ActiveStepStyle` | `TesseraStyle` |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CompletedMarker` | `string` |
| `CompletedStepStyle` | `TesseraStyle` |
| `CurrentIndex` | `int` |
| `DisabledMarker` | `string` |
| `DisabledStepStyle` | `TesseraStyle` |
| `EmptyStyle` | `TesseraStyle` |
| `EmptyText` | `string` |
| `FocusedActiveStepStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HoveredStepStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `Padding` | `Thickness` |
| `PendingMarker` | `string` |
| `PendingStepStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `ShowStepNumbers` | `bool` |
| `StepStyle` | `TesseraStyle` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SelectionChanged` | `EventHandler<WizardStepChangedEventArgs>?` |
| `StepChanged` | `EventHandler<WizardStepChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
