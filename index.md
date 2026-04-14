---
title: Tessera
description: Build terminal software that feels like a real product from the first run.
hide:
  - navigation
  - toc
  - path
---

<div class="home-shell">
  <section class="home-hero">
    <div class="home-copy">
      <span class="home-badge">Public alpha · .NET 10 · C# first</span>
      <h1 class="home-title">Build terminal software with <span class="home-title__accent">real product taste.</span></h1>
      <p class="home-lede">
        Tessera gives you a compact public app model, first-class controls and layouts, semantic theming,
        and polished examples that already feel like tools instead of framework demos.
      </p>
      <div class="home-actions">
        <a class="md-button md-button--primary" href="docs/getting-started/">Get started</a>
        <a class="md-button" href="docs/showcase/">See the showcase</a>
      </div>
      <ul class="home-proof">
        <li>library-first startup path</li>
        <li>semantic theming built in</li>
        <li>real examples, not widget dumps</li>
      </ul>
    </div>
    <div class="home-terminal">
      <div class="home-terminal__frame">
        <div class="home-terminal__bar">
          <span></span>
          <span></span>
          <span></span>
        </div>
        <div class="home-terminal__body">
          <span class="home-terminal__eyebrow">WorkspaceApp</span>
          <h2 class="home-terminal__title">Editor. Preview. Actions. One cohesive shell.</h2>
          <p class="home-terminal__copy">
            A starter application that already reads like a shipping workflow surface.
          </p>
          <div class="home-command">dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj</div>
          <div class="home-chip-row">
            <span class="home-chip home-chip--rose">semantic theming</span>
            <span class="home-chip home-chip--gold">built-in controls</span>
            <span class="home-chip home-chip--cyan">message-driven updates</span>
          </div>
          <ul class="home-terminal__list">
            <li>focused editing zone with preview rail</li>
            <li>action dock and navigation flow already in place</li>
            <li>small public API, no host-heavy ceremony</li>
          </ul>
        </div>
      </div>
    </div>
  </section>

  <section class="home-band">
    <article>
      <strong>Small public model</strong>
      <span>Start with explicit screens, layouts, controls, and messages. No nested DSL maze.</span>
    </article>
    <article>
      <strong>Theme it intentionally</strong>
      <span>Tessera treats tokens and control overrides as real API, not last-mile hacks.</span>
    </article>
    <article>
      <strong>Scale into richer shells</strong>
      <span>Move from the starter ladder into dashboards and workbenches without leaving the same public path.</span>
    </article>
  </section>

  <section class="home-section">
    <div class="home-section__head">
      <span class="home-section__kicker">Starter ladder</span>
      <h2>Three examples. One deliberate learning curve.</h2>
      <p>
        Learn the public path in sequence, then open the denser showcases when you want the full picture.
      </p>
    </div>
    <div class="home-grid">
      <article class="home-example">
        <span class="home-example__eyebrow">starter</span>
        <h3>HelloWorld</h3>
        <p>Centered first screen, first action button, first status line, first polished terminal surface.</p>
        <div class="home-example__meta">
          <span class="home-chip home-chip--rose">layout</span>
          <span class="home-chip">button flow</span>
        </div>
        <div class="home-example__command">dotnet run --project examples/HelloWorld/HelloWorld.csproj</div>
      </article>
      <article class="home-example">
        <span class="home-example__eyebrow">interactive</span>
        <h3>CounterForm</h3>
        <p>Inputs, selection, progress, and message-driven updates without leaving the small public surface.</p>
        <div class="home-example__meta">
          <span class="home-chip home-chip--gold">state</span>
          <span class="home-chip">controls</span>
        </div>
        <div class="home-example__command">dotnet run --project examples/CounterForm/CounterForm.csproj</div>
      </article>
      <article class="home-example">
        <span class="home-example__eyebrow">multi-pane</span>
        <h3>WorkspaceApp</h3>
        <p>Navigation, preview, editing, and action rails inside one shell that already feels product-ready.</p>
        <div class="home-example__meta">
          <span class="home-chip home-chip--cyan">composition</span>
          <span class="home-chip">workflow</span>
        </div>
        <div class="home-example__command">dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj</div>
      </article>
    </div>
  </section>

  <section class="home-section">
    <div class="home-section__head">
      <span class="home-section__kicker">Flagship examples</span>
      <h2>See how the same model scales up.</h2>
      <p>
        Use the flagships to evaluate denser dashboards, command-heavy flows, and workbench-style product shells.
      </p>
    </div>
    <div class="home-grid">
      <article class="home-example">
        <span class="home-example__eyebrow">workflow</span>
        <h3>GitConsole</h3>
        <p>Command-driven workflow surface with editing, diff review, and action history.</p>
        <div class="home-example__command">dotnet run --project examples/GitConsole/GitConsole.csproj</div>
      </article>
      <article class="home-example">
        <span class="home-example__eyebrow">dashboard</span>
        <h3>OpsWatch</h3>
        <p>Telemetry rails, alerts, chips, and operator actions in a dense operations shell.</p>
        <div class="home-example__command">dotnet run --project examples/OpsWatch/OpsWatch.csproj</div>
      </article>
      <article class="home-example">
        <span class="home-example__eyebrow">workbench</span>
        <h3>DataWorkbench</h3>
        <p>Multi-pane composition, richer state orchestration, and pointer-ready runtime configuration.</p>
        <div class="home-example__command">dotnet run --project examples/DataWorkbench/DataWorkbench.csproj</div>
      </article>
    </div>
  </section>

  <section class="home-cta">
    <span class="home-section__kicker">Public path</span>
    <h2>Start with the docs. Then run something real.</h2>
    <p>
      The best way to evaluate Tessera is to open the starter ladder, then run the showcase apps and judge the shape of the product shells yourself.
    </p>
    <div class="home-actions">
      <a class="md-button md-button--primary" href="docs/getting-started/">Read the docs</a>
      <a class="md-button" href="docs/overview/">See the overview</a>
    </div>
  </section>
</div>
