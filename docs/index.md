---
title: Tessera
hide:
  - toc
---

<div class="hero-shell">
  <div class="hero-copy">
    <span class="hero-eyebrow">Public alpha · .NET 10 · C# first</span>
    <h1>Build terminal apps that feel like real products.</h1>
    <p class="hero-lead">
      Tessera gives you a small public app model, first-class controls and layouts, semantic theming,
      and product-grade examples for real terminal software.
    </p>
    <div class="hero-actions">
      <a class="md-button md-button--primary" href="getting-started/">Start here</a>
      <a class="md-button" href="showcase/">See the showcase</a>
    </div>
    <div class="metric-strip">
      <div>
        <strong>.NET 10</strong>
        <span>single pinned SDK</span>
      </div>
      <div>
        <strong>C# first</strong>
        <span>explicit screen composition</span>
      </div>
      <div>
        <strong>Public alpha</strong>
        <span>evaluation, contribution, iteration</span>
      </div>
    </div>
  </div>
  <div class="hero-terminal">
    <div class="hero-terminal__bar">
      <span></span>
      <span></span>
      <span></span>
    </div>
    <div class="hero-terminal__body">
      <div class="hero-terminal__line"><span class="prompt">$</span> dotnet run --project examples/WorkspaceApp</div>
      <div class="hero-terminal__line dim">Tessera WorkspaceApp // preview shell online</div>
      <div class="hero-terminal__block">
        <span class="chip chip--orchid">Focused editor</span>
        <span class="chip chip--rose">Preview rail</span>
        <span class="chip chip--gold">Action dock</span>
      </div>
      <div class="hero-terminal__status">
        <span>semantic theming</span>
        <span>built-in controls</span>
        <span>message-driven updates</span>
      </div>
    </div>
  </div>
</div>

## Start with the public path

<div class="grid cards" markdown>

-   :material-rocket-launch: **Run the starter ladder**

    ---

    Start with `HelloWorld`, `CounterForm`, and `WorkspaceApp` before opening the larger showcases.

    [Open the onboarding path](getting-started.md)

-   :material-palette-swatch: **Theme it intentionally**

    ---

    Tessera treats theme tokens and control overrides as first-class public API, not last-mile hacks.

    [Read the theme system](theme-system.md)

-   :material-view-grid-plus-outline: **Grow into richer surfaces**

    ---

    Move from the starter ladder into `GitConsole`, `OpsWatch`, and `DataWorkbench` when you need a fuller product picture.

    [Tour the showcase](showcase.md)

</div>

## Why teams evaluate Tessera

<div class="grid cards" markdown>

-   :material-language-csharp: **Small public app model**

    ---

    Derive from `TesseraApp`, return `Screen.Build(...)`, handle domain messages in `Update(...)`, and stay in ordinary C#.

-   :material-dock-window: **Controls and layouts included**

    ---

    Buttons, forms, overlays, dashboards, data surfaces, workflows, and layout primitives ship with the framework.

-   :material-brush-variant: **Semantic theming**

    ---

    Global tokens, control defaults, instance overrides, and state overrides. Product styling is part of the contract.

-   :material-console-network-outline: **Advanced hosting when needed**

    ---

    The default path stays small, while `Tessera.Hosting` and `Tessera.Core` remain available for deeper seams.

</div>

## Starter ladder

=== "HelloWorld"

    Smallest polished starter. Centered layout, first button flow, first status readout.

    ```bash
    dotnet run --project examples/HelloWorld/HelloWorld.csproj
    ```

=== "CounterForm"

    First interactive app. Inputs, choice, progress, and message-driven state updates.

    ```bash
    dotnet run --project examples/CounterForm/CounterForm.csproj
    ```

=== "WorkspaceApp"

    First multi-pane shell. Navigation, editing, preview, and actions in one surface.

    ```bash
    dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
    ```

## Flagship examples

<div class="grid cards" markdown>

-   :material-source-branch: **GitConsole**

    ---

    Command-heavy workflow surface with editing, diff review, and action history.

    `dotnet run --project examples/GitConsole/GitConsole.csproj`

-   :material-pulse: **OpsWatch**

    ---

    Dashboard-first operations shell with telemetry, alerts, and operator actions.

    `dotnet run --project examples/OpsWatch/OpsWatch.csproj`

-   :material-table-large: **DataWorkbench**

    ---

    Multi-pane workbench shell for richer state orchestration and pointer-ready surfaces.

    `dotnet run --project examples/DataWorkbench/DataWorkbench.csproj`

</div>

## Continue from here

- [Getting started](getting-started.md)
- [Example guide](examples.md)
- [Theme system](theme-system.md)
- [Architecture overview](architecture-overview.md)
- [Changelog](../CHANGELOG.md)
