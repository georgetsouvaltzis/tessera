# Control Plane Ops Dashboard

`ControlPlaneOpsDashboard` is a realistic operations/control-plane scenario built only with the public onboarding surface:

- `Tea`
- `TeaSharp.Controls`
- `TeaSharp.Layout`
- `TeaSharp.Styles`

## Run

```bash
dotnet run --project examples/ControlPlaneOpsDashboard
```

## Scenario

The app simulates a production control plane with:

- fleet metrics + incidents
- deployment workflows
- analytics trends and endpoint distributions
- command/runbook automation

A periodic pulse (`TeaEffects.Periodic`) updates telemetry, task status, notifications, and plots.

## What It Validates

- public onboarding path only (`Tea`, `TeaSharp.Controls`, `TeaSharp.Layout`, `TeaSharp.Styles`)
- realistic multi-screen composition with navigation + workflow state
- theme switching and per-control style overrides (focus/selected/hover/error/disabled)
- keyboard + pointer interactions across lists, grids, overlays, forms, and task views
- periodic update flow with live table/list/plot refresh
- overlay workflows (`Dialog`, `QuickOpenOverlay`) and action routing from quick actions

## Key Interactions

- `1..5`: switch top views (`Overview`, `Fleet`, `Incidents`, `Analytics`, `Automation`)
- `t`: switch theme (`Catppuccin` / `Rosé Pine`)
- `Ctrl+P`: open/close quick-open overlay
- `d`: open deployment confirmation dialog
- `a`: acknowledge selected incident
- `r` (Automation view): run selected jump/runbook action
- `n`: add a manual notification
- `Ctrl+C`: quit

Pointer interactions are enabled (`MouseTrackingMode.AllMotion`) and exercised by list/grid/board/overlay controls with hover + selection styling.
