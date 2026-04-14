# Overview

<div class="page-shell">
  <section class="overview-band">
    <div>
      <span class="section-kicker">Product-first terminal UI</span>
      <h1>Small public API. Serious application surface.</h1>
      <p>
        Tessera is a C#-first terminal UI framework for teams that want deliberate application structure,
        product-grade visuals, and built-in controls without committing to a host-heavy stack.
      </p>
      <p>
        The public path stays intentionally compact: derive from <code>TesseraApp</code>, compose screens
        with <code>Screen.Build(...)</code>, use controls from <code>Tessera.Controls</code>, and keep
        runtime seams optional until you actually need them.
      </p>
    </div>
    <div class="overview-proof">
      <article class="signal-card">
        <strong>C# object model</strong>
        <span>Explicit screens, controls, layouts, and messages. No nested layout DSL required.</span>
      </article>
      <article class="signal-card">
        <strong>Theme-driven visuals</strong>
        <span>Tokens, control defaults, instance overrides, and state styling are part of the contract.</span>
      </article>
      <article class="signal-card">
        <strong>Grow when you need to</strong>
        <span><code>Tessera.Hosting</code> and <code>Tessera.Core</code> stay available for deeper runtime seams.</span>
      </article>
    </div>
  </section>

  <section class="split-band">
    <div>
      <span class="section-kicker">Public app model</span>
      <h2>What the default path looks like</h2>
      <p>
        Normal apps should live in <code>Tessera</code>, <code>Tessera.Controls</code>,
        <code>Tessera.Layout</code>, and <code>Tessera.Styles</code>. The framework should read like explicit
        screen composition in ordinary C#.
      </p>
      <div class="workflow-list">
        <div class="workflow-step">
          <strong>1. Build the app shell</strong>
          Create a <code>TesseraApp</code> and return screens with <code>Build(ScreenContext)</code>.
        </div>
        <div class="workflow-step">
          <strong>2. Handle messages</strong>
          Apply state transitions in <code>Update(Message)</code> and return effects only when needed.
        </div>
        <div class="workflow-step">
          <strong>3. Keep the path shallow</strong>
          Use <code>TesseraApplication.RunAsync(...)</code> or <code>CreateBuilder()</code> and stay out of <code>Tessera.Core</code> unless the app truly needs it.
        </div>
      </div>
    </div>
    <div class="code-panel">
<pre><code>using Tessera;
using Tessera.Controls;
using Tessera.Layout;

var app = TesseraApplication.CreateBuilder()
    .UseApp&lt;WorkspaceApp&gt;()
    .Build();

await app.RunAsync();

internal sealed class WorkspaceApp : TesseraApp
{
    public override Screen Build(ScreenContext context)
        =&gt; Screen.Build(window =&gt;
        {
            window.Padding(1);
            window.Body(body =&gt; body.Center(
                new Button { Text = "Launch" },
                width: 20,
                height: 3));
        });
}</code></pre>
    </div>
  </section>

  <section class="section-block">
    <div class="section-head">
      <div>
        <span class="section-kicker">Why teams evaluate it</span>
        <h2>Framework ergonomics plus product-surface ambition.</h2>
      </div>
      <p>Tessera is not trying to be the smallest widget sandbox. It is trying to be the best default path for real terminal software in C#.</p>
    </div>
    <div class="signal-row">
      <article class="signal-card">
        <strong>Polished starter ladder</strong>
        <span><code>HelloWorld</code>, <code>CounterForm</code>, and <code>WorkspaceApp</code> teach the framework without dumping the whole catalog at once.</span>
      </article>
      <article class="signal-card">
        <strong>Flagship evaluation apps</strong>
        <span><code>GitConsole</code>, <code>OpsWatch</code>, and <code>DataWorkbench</code> show how the public path scales into denser surfaces.</span>
      </article>
      <article class="signal-card">
        <strong>Contributor-friendly shape</strong>
        <span>The repo is organized around public API boundaries, examples, tests, docs, and release checklists instead of hidden internal magic.</span>
      </article>
    </div>
  </section>
</div>
