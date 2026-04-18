---
sidebar_label: "Widgets: Inputs & Forms"
---

# Widgets: Inputs And Forms

Use these widgets when the user is editing data, confirming choices, or moving through a guided form.

## Core entry widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Button` | primary actions | keyboard/pointer activation, pressed/focused states, optional description | `HelloWorld`, `WorkspaceApp` |
| `Label` | static headings or readouts | simple styled text without interaction | starter shells |
| `Badge` | compact state chips | tone-driven status or count labels | dashboards and rails |
| `TextInput` | single-line input | placeholder text, editing, submit/cancel events | `CounterForm` |
| `TextArea` | multi-line input | paragraph editing and larger text buffers | denser edit flows |
| `NumberInput` | numeric entry | bounded numeric editing and direct value setting | `CounterForm` |
| `Choice` | small one-of-many selection | compact choice list for short option sets | `CounterForm` |
| `ComboBox` | typed selection from a known list | editable text + item list | form-heavy apps |
| `AutocompleteInput` | suggestion-first text entry | free typing with commitable suggestions | search/config flows |
| `DatePicker` | date entry | calendar-style date picking | planning and scheduling |
| `TimePicker` | time entry | time-of-day editing | planning and scheduling |

## Selection and boolean widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `RadioGroup` | visible single-choice groups | explicit mutually exclusive options | forms and settings |
| `Toggle` | boolean state | on/off interaction with focused styling | settings rows |
| `Slider` | bounded range changes | scrub numeric values with keyboard/pointer | settings and tuning |
| `MultiSelect` | many-of-many selection | multiple checked options in one surface | filters and option sets |
| `TagInput` | token/tag editing | add/remove tags or short labels | query and labeling flows |

## Form containers

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Form` | explicit field/value forms | fixed form rows with helper text and required hints | admin/config screens |
| `FieldSet` | grouped rows or grouped options | framed grouping and section readability | dense forms |
| `ValidationSummary` | validation feedback | aggregated errors and quick attention guidance | submit flows |
| `DataForm<TModel>` | model-bound forms | registered field definitions, model binding, validators, commit events | advanced form flows |

## Guided workflow widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Stepper` | compact multi-step progress | current-step state and completion markers | onboarding or setup |
| `Wizard` | guided step flow | larger step descriptions and explicit progression | setup and task flows |

## When to choose `Form` vs `DataForm<TModel>`

- use `Form` when the screen is mostly explicit UI rows
- use `DataForm<TModel>` when you want a bound model and field registration to stay together

## Related pages

- recipes: [recipes-app-shells.md](recipes-app-shells.md)
- recipes: [recipes-effects-and-refresh.md](recipes-effects-and-refresh.md)
- API inventory: [public-api-inventory.md](public-api-inventory.md)
