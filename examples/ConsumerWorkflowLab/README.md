# ConsumerWorkflowLab

Focused public-API consumer app for multi-step workflow/forms flows.

## Run

```bash
dotnet run --project examples/ConsumerWorkflowLab/ConsumerWorkflowLab.csproj
```

## What It Exercises

- `Stepper` + `Wizard` step synchronization and gating.
- `DataForm<WorkflowDraft>` edits with validators and commit feedback.
- `ValidationSummary` issue routing back to form/input controls.
- `Form` + `FieldSet` review/policy projections.
- `Choice` + `ComboBox` selection stress loops (`Ctrl+1`, `Ctrl+2`).
- DataForm keyed selection stress loop (`Ctrl+3`).
- Full stress pass (`Ctrl+S`) repeatedly cycling all three selection flows.
- `Dialog` submit confirmation flow (`d`).
- Theme toggle and state style overrides (`Ctrl+T`).

## Keybindings

- `Ctrl+V`: run validation
- `Ctrl+N` / `Ctrl+B`: next/back step
- `Ctrl+1`: cycle environment (`Choice` selection API path)
- `Ctrl+2`: cycle template (`ComboBox` selection API path)
- `Ctrl+3`: cycle `DataForm` field (`SelectField(string key)` path)
- `Ctrl+S`: run combined selection stress pass
- `d`: attempt submit (opens dialog when valid + on final step)
- `Ctrl+C`: quit
