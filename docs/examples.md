# TeaSharp Example Guide

TeaSharp ships three example groups:

- starter examples: the main public learning path
- flagship examples: the main public evaluation path after the starter ladder
- supporting demos: narrower domain-focused showcase apps

## Starter Ladder

| Example | Run | What It Teaches |
| --- | --- | --- |
| `HelloWorld` | `dotnet run --project examples/HelloWorld/HelloWorld.csproj` | smallest centered app, button activation, status text, visual theme intent |
| `CounterForm` | `dotnet run --project examples/CounterForm/CounterForm.csproj` | inputs, choice, progress, and message-driven state changes |
| `WorkspaceApp` | `dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj` | multi-pane composition, editing, preview, and action flow in one shell |

## Flagship Examples

| Example | Run | What It Teaches |
| --- | --- | --- |
| `GitConsole` | `dotnet run --project examples/GitConsole/GitConsole.csproj` | command-driven workflow surfaces, navigation, editing, diff review, action history |
| `OpsWatch` | `dotnet run --project examples/OpsWatch/OpsWatch.csproj` | dashboard composition, telemetry cards, status surfaces, operator actions |
| `DataWorkbench` | `dotnet run --project examples/DataWorkbench/DataWorkbench.csproj` | multi-pane workbench shells, richer state orchestration, pointer-ready runtime configuration |

Recommended order:

1. `HelloWorld`
2. `CounterForm`
3. `WorkspaceApp`
4. `GitConsole`
5. `OpsWatch`
6. `DataWorkbench`

## Supporting Demos

| Example | Run | Focus |
| --- | --- | --- |
| `DownloadCenter` | `dotnet run --project examples/DownloadCenter/DownloadCenter.csproj` | transfer dashboard, grouped jobs, throughput/status surfaces |
| `IncidentDesk` | `dotnet run --project examples/IncidentDesk/IncidentDesk.csproj` | incident triage, queue-like workflows, action-heavy layouts |
| `MusicDeck` | `dotnet run --project examples/MusicDeck/MusicDeck.csproj` | media-oriented dashboard styling and browse/playback flows |
| `TransitBoard` | `dotnet run --project examples/TransitBoard/TransitBoard.csproj` | dense board-style presentation and schedule surfaces |

## Example Rules

Examples in this repository should:

- stay in `TeaSharp` namespaces on the default path
- avoid leaking `TeaSharp.Core.*` into public onboarding
- teach one clear idea per example
- remain visually intentional, not default-terminal placeholders
- update docs when the public learning path changes

If a new example becomes part of the public learning path, update:

- [README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/README.md)
- [getting-started.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/getting-started.md)
- [public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- [spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md)
- `scripts/smoke_examples_v1.sh`
