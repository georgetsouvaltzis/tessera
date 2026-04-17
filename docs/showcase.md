---
sidebar_label: Showcase
---

# Showcase

Use the showcase after the starter ladder. This is where Tessera should start to feel like product infrastructure, not just a control catalog.

## Flagship Evaluation Order

1. `GitConsole`
2. `OpsWatch`
3. `DataWorkbench`

## `GitConsole`

Best when you want to judge command-heavy workflow surfaces.

- patch deck and diff review
- worktree and commit flow
- denser navigation and action rails

```bash
dotnet run --project examples/GitConsole/GitConsole.csproj
```

## `OpsWatch`

Best when you want to judge dashboard composition and operator-facing density.

- telemetry cards and charts
- alert feed and pressure indicators
- operator actions inside one surface

```bash
dotnet run --project examples/OpsWatch/OpsWatch.csproj
```

## `DataWorkbench`

Best when you want the richest workbench-style evaluation.

- multi-pane investigation layout
- tabs, result grids, and inspector flows
- pointer-ready runtime configuration and denser state orchestration

```bash
dotnet run --project examples/DataWorkbench/DataWorkbench.csproj
```

## Supporting Demos

Use these once the flagship path is already clear:

- `DownloadCenter`: grouped jobs and transfer-heavy surfaces
  - `dotnet run --project examples/DownloadCenter/DownloadCenter.csproj`
- `IncidentDesk`: triage and queue-oriented workflows
  - `dotnet run --project examples/IncidentDesk/IncidentDesk.csproj`
- `MusicDeck`: media-oriented dashboard styling and playback flows
  - `dotnet run --project examples/MusicDeck/MusicDeck.csproj`
- `TransitBoard`: dense board-style presentation and schedule surfaces
  - `dotnet run --project examples/TransitBoard/TransitBoard.csproj`
