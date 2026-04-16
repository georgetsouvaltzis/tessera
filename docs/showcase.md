# Showcase

The showcase is intentionally tiered so you can evaluate the framework in the same order a real team would adopt it.

## Evaluation lane

- `Starter ladder`: first-contact examples for layout, controls, forms, and multi-pane composition
- `Flagship apps`: richer shells that show how the public path handles dashboards, workbenches, and command-heavy workflows
- `Supporting demos`: narrower domain showcases for boards, media, transfer surfaces, and incident-style layouts

## Starter ladder

These are the right first runs when you want to understand Tessera’s public model before judging its denser surfaces.

| Example | What to look for | Run |
| --- | --- | --- |
| `HelloWorld` | smallest polished entry point, centered composition, status text, first button activation | `dotnet run --project examples/HelloWorld/HelloWorld.csproj` |
| `CounterForm` | inputs, choice, progress, and message-driven updates inside the standard public surface | `dotnet run --project examples/CounterForm/CounterForm.csproj` |
| `WorkspaceApp` | preview, editing, navigation, and actions together in one shell | `dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj` |

## Flagship examples

Flagships are where Tessera should start to feel like product infrastructure, not just a control catalog.

| Example | What to look for | Run |
| --- | --- | --- |
| `GitConsole` | editing, navigation, diff review, command history, and action rails | `dotnet run --project examples/GitConsole/GitConsole.csproj` |
| `OpsWatch` | dense telemetry cards, alerts, chips, health rails, and operator actions | `dotnet run --project examples/OpsWatch/OpsWatch.csproj` |
| `DataWorkbench` | richer state orchestration, multi-pane composition, and pointer-ready runtime configuration | `dotnet run --project examples/DataWorkbench/DataWorkbench.csproj` |

## Supporting demos

These demos are useful once you already understand the main public path and want domain-specific texture.

| Example | What to look for | Run |
| --- | --- | --- |
| `DownloadCenter` | grouped jobs, throughput/status surfaces, action-heavy rails | `dotnet run --project examples/DownloadCenter/DownloadCenter.csproj` |
| `IncidentDesk` | triage and queue-like workflows | `dotnet run --project examples/IncidentDesk/IncidentDesk.csproj` |
| `MusicDeck` | media-oriented dashboard styling and browse/playback flows | `dotnet run --project examples/MusicDeck/MusicDeck.csproj` |
| `TransitBoard` | dense board-style presentation and schedule surfaces | `dotnet run --project examples/TransitBoard/TransitBoard.csproj` |
