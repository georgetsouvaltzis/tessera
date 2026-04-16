import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import CodeBlock from '@theme/CodeBlock';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';

const proofItems = [
  {
    title: '.NET 10',
    description: 'C#-first authoring with a small public surface.',
  },
  {
    title: 'Starter Ladder',
    description: 'HelloWorld -> CounterForm -> WorkspaceApp.',
  },
  {
    title: 'Flagship Apps',
    description: 'GitConsole, OpsWatch, and DataWorkbench.',
  },
];

const showcaseItems = [
  {
    title: 'GitConsole',
    eyebrow: 'Workflow shell',
    description:
      'Command-heavy review flows, diff rails, focused actions, and denser editing surfaces.',
    command: 'dotnet run --project examples/GitConsole/GitConsole.csproj',
  },
  {
    title: 'OpsWatch',
    eyebrow: 'Operational dashboard',
    description:
      'Alert-heavy telemetry panels, status cards, health rails, and operator actions in one shell.',
    command: 'dotnet run --project examples/OpsWatch/OpsWatch.csproj',
  },
  {
    title: 'DataWorkbench',
    eyebrow: 'Multi-pane workbench',
    description:
      'Investigation-focused layout with panes, inspectors, tabs, and execution lanes under pressure.',
    command: 'dotnet run --project examples/DataWorkbench/DataWorkbench.csproj',
  },
];

const evaluationSteps = [
  {
    step: '01',
    title: 'Read the overview',
    description: 'Understand the public path before touching the denser examples.',
    href: '/docs/overview',
  },
  {
    step: '02',
    title: 'Run the starters',
    description: 'Confirm the framework model with HelloWorld, CounterForm, and WorkspaceApp.',
    href: '/docs/getting-started',
  },
  {
    step: '03',
    title: 'Open the flagships',
    description: 'Judge whether the same API still holds under real application pressure.',
    href: '/docs/showcase',
  },
];

const starterSample = `using Tessera;
using Tessera.Controls;
using Tessera.Layout;

var app = TesseraApplication.CreateBuilder()
    .UseApp<WorkspaceApp>()
    .Build();

await app.RunAsync();

internal sealed class WorkspaceApp : TesseraApp
{
    public override Screen Build(ScreenContext context)
        => Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(body => body.Center(
                new Button { Text = "Launch" },
                width: 20,
                height: 3));
        });
}`;

function HomepageHero() {
  const { siteConfig } = useDocusaurusContext();

  return (
    <section className="home-hero">
      <div className="container">
        <div className="home-hero__grid">
          <div className="home-hero__copy">
            <span className="home-badge">Public alpha • .NET 10 • C#-first</span>
            <h1 className="home-hero__title">
              Terminal UI for
              <br />
              serious .NET apps.
            </h1>
            <p className="home-hero__subtitle">
              Build dashboards, workflows, and workbenches in C# without a host-heavy stack or
              a toy-widget first impression.
            </p>
            <div className="home-hero__actions">
              <Link className="button button--primary button--lg" to="/docs/getting-started">
                Get Started
              </Link>
              <Link className="button button--secondary button--lg" to="/docs/showcase">
                View Examples
              </Link>
            </div>
            <div className="home-proof-strip">
              {proofItems.map((item) => (
                <div key={item.title} className="home-proof-strip__item">
                  <strong>{item.title}</strong>
                  <span>{item.description}</span>
                </div>
              ))}
            </div>
          </div>
          <div className="home-preview">
            <div className="home-preview__frame">
              <div className="home-preview__bar">
                <span />
                <span />
                <span />
                <em>{siteConfig.title} Preview</em>
              </div>
              <div className="home-preview__body">
                <aside className="home-preview__nav">
                  <span className="home-preview__label">Starter path</span>
                  <strong>HelloWorld</strong>
                  <strong>CounterForm</strong>
                  <strong className="is-active">WorkspaceApp</strong>
                  <span className="home-preview__label">Flagships</span>
                  <strong>GitConsole</strong>
                  <strong>OpsWatch</strong>
                  <strong>DataWorkbench</strong>
                </aside>
                <div className="home-preview__content">
                  <div className="home-preview__toolbar">
                    <span>Workspace</span>
                    <span>Preview</span>
                    <span>Actions</span>
                  </div>
                  <div className="home-preview__panes">
                    <div className="home-preview__pane home-preview__pane--primary">
                      <div className="home-preview__card">
                        <b>Orders</b>
                        <span>127 open</span>
                      </div>
                      <div className="home-preview__card">
                        <b>Latency</b>
                        <span>p95 18ms</span>
                      </div>
                      <div className="home-preview__card">
                        <b>Incidents</b>
                        <span>2 active</span>
                      </div>
                    </div>
                    <div className="home-preview__pane">
                      <div className="home-preview__log">$ run starter ladder</div>
                      <div className="home-preview__log">$ open DataWorkbench</div>
                      <div className="home-preview__log is-accent">$ evaluate flagship shell</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function HomepageContent() {
  return (
    <main>
      <section className="home-section">
        <div className="container">
          <div className="home-section__header">
            <span className="home-section__eyebrow">Flagship examples</span>
            <h2>See the framework under real layout pressure.</h2>
            <p>
              The homepage should show more than generic feature claims. These are the interfaces
              that answer whether Tessera feels like product infrastructure or just a widget set.
            </p>
          </div>
          <div className="home-showcase-grid">
            {showcaseItems.map((item) => (
              <article key={item.title} className="home-showcase-card">
                <span className="home-showcase-card__eyebrow">{item.eyebrow}</span>
                <h3>{item.title}</h3>
                <p>{item.description}</p>
                <code>{item.command}</code>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="home-section home-section--split">
        <div className="container">
          <div className="home-split">
            <div className="home-split__copy">
              <span className="home-section__eyebrow">Starter API</span>
              <h2>Readable first app. No hidden ceremony.</h2>
              <p>
                Prisma’s docs work because the structure is calm. Rezi works because the landing
                page shows the product quickly. For Tessera, the first sample should do the same:
                obvious builder, obvious app shell, obvious screen composition.
              </p>
              <ul className="home-copy-list">
                <li>derive from `TesseraApp`</li>
                <li>compose with `Screen.Build(...)`</li>
                <li>drop into runtime seams only when needed</li>
              </ul>
            </div>
            <div className="home-split__code">
              <CodeBlock language="csharp" title="starter.cs">
                {starterSample}
              </CodeBlock>
            </div>
          </div>
        </div>
      </section>

      <section className="home-section">
        <div className="container">
          <div className="home-section__header">
            <span className="home-section__eyebrow">Evaluation path</span>
            <h2>Three steps. Fast signal.</h2>
            <p>
              Keep the homepage honest: show the path a real evaluator should take, instead of
              burying them in equal-weight feature blocks.
            </p>
          </div>
          <div className="home-steps">
            {evaluationSteps.map((item) => (
              <article key={item.step} className="home-step">
                <span className="home-step__index">{item.step}</span>
                <h3>{item.title}</h3>
                <p>{item.description}</p>
                <Link to={item.href}>Open</Link>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="home-cta-band">
        <div className="container">
          <div className="home-cta-band__inner">
            <div>
              <span className="home-section__eyebrow">Ready to evaluate?</span>
              <h2>Start with the guide, then open the flagship apps.</h2>
            </div>
            <div className="home-hero__actions">
              <Link className="button button--primary button--lg" to="/docs/getting-started">
                Get Started
              </Link>
              <Link className="button button--secondary button--lg" to="/docs/showcase">
                Browse Showcase
              </Link>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}

export default function Home(): React.JSX.Element {
  return (
    <Layout
      title="Terminal UI for .NET"
      description="Tessera is a C#-first terminal UI framework for serious application surfaces.">
      <HomepageHero />
      <HomepageContent />
    </Layout>
  );
}
