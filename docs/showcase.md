# Tessera Showcase

The public evaluation path is intentionally split:

- starter ladder for first contact
- flagship examples for product-level evaluation
- supporting demos for narrower domain exploration

## Starter ladder

<div class="grid cards" markdown>

-   :material-gesture-tap-button: **HelloWorld**

    ---

    Smallest polished starter. Centered composition, first button activation, and status text.

    ```bash
    dotnet run --project examples/HelloWorld/HelloWorld.csproj
    ```

-   :material-form-textbox: **CounterForm**

    ---

    Interactive starter for forms, choice, progress, and message-driven state transitions.

    ```bash
    dotnet run --project examples/CounterForm/CounterForm.csproj
    ```

-   :material-view-split-horizontal: **WorkspaceApp**

    ---

    Multi-pane starter for navigation, editing, preview, and action flow inside one shell.

    ```bash
    dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
    ```

</div>

## Flagship examples

=== "GitConsole"

    Use this when you want to evaluate command-heavy workflow surfaces, editing, navigation, diff review, and action history.

    ```bash
    dotnet run --project examples/GitConsole/GitConsole.csproj
    ```

=== "OpsWatch"

    Use this when you want to evaluate a dense dashboard surface with telemetry, health rails, chips, and operator actions.

    ```bash
    dotnet run --project examples/OpsWatch/OpsWatch.csproj
    ```

=== "DataWorkbench"

    Use this when you want to evaluate richer multi-pane shells, state orchestration, and pointer-ready runtime composition.

    ```bash
    dotnet run --project examples/DataWorkbench/DataWorkbench.csproj
    ```

## Supporting demos

| Example | What to look for | Run |
| --- | --- | --- |
| `DownloadCenter` | grouped jobs, throughput/status surfaces, action-heavy rails | `dotnet run --project examples/DownloadCenter/DownloadCenter.csproj` |
| `IncidentDesk` | triage and queue-like workflows | `dotnet run --project examples/IncidentDesk/IncidentDesk.csproj` |
| `MusicDeck` | media-oriented dashboard styling and browse/playback flows | `dotnet run --project examples/MusicDeck/MusicDeck.csproj` |
| `TransitBoard` | dense board-style presentation and schedule surfaces | `dotnet run --project examples/TransitBoard/TransitBoard.csproj` |

## What to evaluate

When you run the examples, check these things:

- readability under dense terminal layouts
- keyboard-first navigation and action flow
- theme coherence across panels, buttons, overlays, and data surfaces
- how the default public path feels without host-heavy setup
- whether the example shell reads like a product surface, not a widget dump

## Where to go next

- [Getting started](getting-started.md)
- [Example guide](examples.md)
- [Theme system](theme-system.md)
- [Architecture overview](architecture-overview.md)
