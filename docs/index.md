---
title: Tessera
hide:
  - navigation
  - toc
  - path
---

<div class="page-shell page-shell--landing">
  <section class="hero-panel">
    <div class="hero-copy">
      <span class="eyebrow">Public alpha · .NET 10 · C# first</span>
      <h1 class="hero-title">Ship terminal software with <span class="gradient-text">product-grade taste.</span></h1>
      <p class="lede">
        Tessera gives you a compact public app model, first-class controls and layouts, semantic theming,
        and showcase apps that read like products instead of demos.
      </p>
      <div class="action-row">
        <a class="md-button md-button--primary" href="getting-started/">Start here</a>
        <a class="md-button" href="showcase/">See the showcase</a>
        <a class="md-button" href="overview/">Read the overview</a>
      </div>
      <div class="trust-grid">
        <div class="trust-card">
          <strong>.NET 10</strong>
          <span>single pinned SDK, library-first startup path</span>
        </div>
        <div class="trust-card">
          <strong>C# object model</strong>
          <span>explicit screens, layouts, controls, and messages</span>
        </div>
        <div class="trust-card">
          <strong>Public alpha</strong>
          <span>ready for evaluation, contribution, and early product work</span>
        </div>
      </div>
    </div>
    <div class="product-stage">
      <div class="stage-topline">
        <span></span>
        <span></span>
        <span></span>
      </div>
      <div class="stage-grid">
        <div class="stage-pane stage-pane--wide">
          <span class="stage-label">WorkspaceApp</span>
          <div class="stage-title">Editor · Preview · Action rail</div>
          <div class="stage-copy">A multi-pane starter that already feels like a real workflow shell.</div>
          <ul class="stage-list">
            <li>query composer and action dock</li>
            <li>preview rail and focused editing zone</li>
            <li>theme-aware controls, not ad hoc styling</li>
          </ul>
        </div>
        <div class="stage-meta">
          <div class="stage-metric">
            <strong>Starter ladder</strong>
            <span>HelloWorld → CounterForm → WorkspaceApp</span>
          </div>
          <div class="stage-metric">
            <strong>Flagships</strong>
            <span>GitConsole · OpsWatch · DataWorkbench</span>
          </div>
        </div>
        <div class="stage-actionbar">
          <span class="action-pill action-pill--pink">semantic theming</span>
          <span class="action-pill action-pill--gold">built-in controls</span>
          <span class="action-pill action-pill--cyan">message-driven updates</span>
        </div>
      </div>
    </div>
  </section>

  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Start with the public path</span>
        <h2>From first run to richer application surfaces.</h2>
      </div>
      <p>The docs site should help you evaluate the framework quickly, then deepen only where you need detail.</p>
    </div>
    <div class="signal-row">
      <article class="signal-card">
        <strong>Run the starter ladder</strong>
        <span>Start with <code>HelloWorld</code>, <code>CounterForm</code>, and <code>WorkspaceApp</code> before opening the larger showcases.</span>
      </article>
      <article class="signal-card">
        <strong>Theme it intentionally</strong>
        <span>Tessera treats tokens and control overrides as first-class public API, not last-mile hacks.</span>
      </article>
      <article class="signal-card">
        <strong>Evaluate bigger shells</strong>
        <span>Move into <code>GitConsole</code>, <code>OpsWatch</code>, and <code>DataWorkbench</code> when you want the fuller picture.</span>
      </article>
    </div>
  </section>

  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Starter ladder</span>
        <h2>Three examples. One deliberate learning curve.</h2>
      </div>
      <p>Each example is there to teach one level of app composition, not to dump the entire widget surface on first contact.</p>
    </div>
    <div class="starter-grid">
      <article>
        <div class="starter-card__header">
          <strong class="starter-card__title">HelloWorld</strong>
          <span class="starter-card__tag">starter</span>
        </div>
        <span class="starter-card__copy">Centered layout, first button activation, first status line, first polished screen.</span>
        <div class="command-line">dotnet run --project examples/HelloWorld/HelloWorld.csproj</div>
      </article>
      <article>
        <div class="starter-card__header">
          <strong class="starter-card__title">CounterForm</strong>
          <span class="starter-card__tag">interactive</span>
        </div>
        <span class="starter-card__copy">Inputs, choice, progress, and message-driven updates without leaving the small public path.</span>
        <div class="command-line">dotnet run --project examples/CounterForm/CounterForm.csproj</div>
      </article>
      <article>
        <div class="starter-card__header">
          <strong class="starter-card__title">WorkspaceApp</strong>
          <span class="starter-card__tag">multi-pane</span>
        </div>
        <span class="starter-card__copy">Navigation, preview, editing, and actions inside one cohesive shell.</span>
        <div class="command-line">dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj</div>
      </article>
    </div>
  </section>

  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Flagship examples</span>
        <h2>See how the same public model scales up.</h2>
      </div>
      <p>Flagships are the evaluation lane for denser dashboards, command workflows, and workbench-style shells.</p>
    </div>
    <div class="showcase-grid">
      <article>
        <div class="showcase-card__header">
          <strong class="showcase-card__title">GitConsole</strong>
          <span class="showcase-card__tag">workflow</span>
        </div>
        <span class="showcase-card__copy">Command-driven workflow surface with editing, diff review, and action history.</span>
        <div class="command-line">dotnet run --project examples/GitConsole/GitConsole.csproj</div>
      </article>
      <article>
        <div class="showcase-card__header">
          <strong class="showcase-card__title">OpsWatch</strong>
          <span class="showcase-card__tag">dashboard</span>
        </div>
        <span class="showcase-card__copy">Telemetry rails, alerts, chips, and operator actions in a dense operations shell.</span>
        <div class="command-line">dotnet run --project examples/OpsWatch/OpsWatch.csproj</div>
      </article>
      <article>
        <div class="showcase-card__header">
          <strong class="showcase-card__title">DataWorkbench</strong>
          <span class="showcase-card__tag">workbench</span>
        </div>
        <span class="showcase-card__copy">Multi-pane composition, richer state orchestration, and pointer-ready runtime configuration.</span>
        <div class="command-line">dotnet run --project examples/DataWorkbench/DataWorkbench.csproj</div>
      </article>
    </div>
  </section>
</div>
