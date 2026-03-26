# WidgetCoverageWorkflowLab

Realistic internal release-change intake app focused on forms/workflow/editor interactions using public TeaSharp APIs only.

## App Name

`WidgetCoverageWorkflowLab`

## Run

```bash
dotnet run --project examples/WidgetCoverageWorkflowLab/WidgetCoverageWorkflowLab.csproj
```

## What This Slice Exercises

- Workflow controls: `Stepper` + `Wizard`
- Form surface: `DataForm<ChangeDraft>`, `Form`, `FieldSet`, `ValidationSummary`, `EmptyState`
- Editor/input surface: `TextInput`, `SearchBox`, `Choice`, `ComboBox`, `TagInput`, `TokenEditor`, `AutocompleteInput`, paired `NumberInput` as rollout range flow
- Activity/ops surface: `Notifications`, `NotificationInbox`, `InspectorPanel`
- Consumer logic: template apply, validation gates, issue-to-focus routing, step synchronization, approval queueing, custom style/state overrides

## Keybindings

- `Ctrl+V`: validate
- `Ctrl+N` / `Ctrl+B`: next/back step
- `Ctrl+A`: queue approval request
- `Ctrl+1`: cycle environment
- `Ctrl+2`: cycle template
- `Ctrl+R`: route first validation issue to target control
- `Ctrl+C`: quit
