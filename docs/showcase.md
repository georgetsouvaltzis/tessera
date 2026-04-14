---
title: Showcase
hide:
  - toc
---

# Showcase

<div class="page-shell">
  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Evaluation lane</span>
        <h2>From polished starters to denser product surfaces.</h2>
      </div>
      <p>The showcase is intentionally tiered so you can evaluate the framework in the same order a real team would adopt it.</p>
    </div>
    <div class="signal-row">
      <article class="signal-card">
        <strong>Starter ladder</strong>
        <span>First-contact examples for layout, controls, forms, and multi-pane composition.</span>
      </article>
      <article class="signal-card">
        <strong>Flagship apps</strong>
        <span>Richer shells that show how the public path handles dashboards, workbenches, and command-heavy workflows.</span>
      </article>
      <article class="signal-card">
        <strong>Supporting demos</strong>
        <span>Narrower domain showcases for boards, media, transfer surfaces, and incident-style layouts.</span>
      </article>
    </div>
  </section>

  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Starter ladder</span>
        <h2>Learn the framework without starting in the deep end.</h2>
      </div>
      <p>These are the right first runs when you want to understand Tessera’s public model before judging its denser surfaces.</p>
    </div>
    <div class="starter-grid">
      <article>
        <div class="starter-card__header">
          <strong class="starter-card__title">HelloWorld</strong>
          <span class="starter-card__tag">starter</span>
        </div>
        <span class="starter-card__copy">Smallest polished entry point. Centered composition, status text, first button activation.</span>
        <div class="command-line">dotnet run --project examples/HelloWorld/HelloWorld.csproj</div>
      </article>
      <article>
        <div class="starter-card__header">
          <strong class="starter-card__title">CounterForm</strong>
          <span class="starter-card__tag">interactive</span>
        </div>
        <span class="starter-card__copy">Inputs, choice, progress, and message-driven updates inside the standard public surface.</span>
        <div class="command-line">dotnet run --project examples/CounterForm/CounterForm.csproj</div>
      </article>
      <article>
        <div class="starter-card__header">
          <strong class="starter-card__title">WorkspaceApp</strong>
          <span class="starter-card__tag">multi-pane</span>
        </div>
        <span class="starter-card__copy">Preview, editing, navigation, and actions together in one shell.</span>
        <div class="command-line">dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj</div>
      </article>
    </div>
  </section>

  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Flagship examples</span>
        <h2>Evaluate how the public path behaves under real application pressure.</h2>
      </div>
      <p>Flagships are where Tessera should start to feel like product infrastructure, not just a control catalog.</p>
    </div>
    <div class="showcase-grid">
      <article>
        <div class="showcase-card__header">
          <strong class="showcase-card__title">GitConsole</strong>
          <span class="showcase-card__tag">workflow</span>
        </div>
        <span class="showcase-card__copy">Editing, navigation, diff review, command history, and action rails.</span>
        <div class="command-line">dotnet run --project examples/GitConsole/GitConsole.csproj</div>
      </article>
      <article>
        <div class="showcase-card__header">
          <strong class="showcase-card__title">OpsWatch</strong>
          <span class="showcase-card__tag">dashboard</span>
        </div>
        <span class="showcase-card__copy">Dense telemetry cards, alerts, chips, health rails, and operator actions.</span>
        <div class="command-line">dotnet run --project examples/OpsWatch/OpsWatch.csproj</div>
      </article>
      <article>
        <div class="showcase-card__header">
          <strong class="showcase-card__title">DataWorkbench</strong>
          <span class="showcase-card__tag">workbench</span>
        </div>
        <span class="showcase-card__copy">Richer state orchestration, multi-pane composition, and pointer-ready runtime configuration.</span>
        <div class="command-line">dotnet run --project examples/DataWorkbench/DataWorkbench.csproj</div>
      </article>
    </div>
  </section>

  <section class="support-band" markdown="1">
    <div class="section-head">
      <div>
        <span class="section-kicker">Supporting demos</span>
        <h2>Explore narrower domains after the flagship pass.</h2>
      </div>
      <p>These demos are useful once you already understand the main public path and want domain-specific texture.</p>
    </div>

| Example | What to look for | Run |
| --- | --- | --- |
| `DownloadCenter` | grouped jobs, throughput/status surfaces, action-heavy rails | `dotnet run --project examples/DownloadCenter/DownloadCenter.csproj` |
| `IncidentDesk` | triage and queue-like workflows | `dotnet run --project examples/IncidentDesk/IncidentDesk.csproj` |
| `MusicDeck` | media-oriented dashboard styling and browse/playback flows | `dotnet run --project examples/MusicDeck/MusicDeck.csproj` |
| `TransitBoard` | dense board-style presentation and schedule surfaces | `dotnet run --project examples/TransitBoard/TransitBoard.csproj` |
  </section>
</div>
