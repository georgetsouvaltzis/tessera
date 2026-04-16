import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';

const proofItems = [
  {
    title: '.NET 10',
    description: 'Small public surface. C# first.',
  },
  {
    title: 'Starter Ladder',
    description: 'HelloWorld -> CounterForm -> WorkspaceApp.',
  },
  {
    title: 'Flagship Apps',
    description: 'GitConsole, OpsWatch, DataWorkbench.',
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
    title: 'Overview',
    description: 'Framework model, boundaries, and public path.',
    href: '/docs/overview',
  },
  {
    step: '02',
    title: 'Starter apps',
    description: 'HelloWorld, CounterForm, then WorkspaceApp.',
    href: '/docs/getting-started',
  },
  {
    step: '03',
    title: 'Flagships',
    description: 'Open the denser shells and judge the same API under pressure.',
    href: '/docs/showcase',
  },
];

function HomepageHero() {
  const { siteConfig } = useDocusaurusContext();

  return (
    <section className="home-hero">
      <div className="container">
        <div className="home-hero__grid">
          <div className="home-hero__copy">
            <span className="home-badge">Public alpha • .NET 10 • C#-first</span>
            <h1 className="home-hero__title">
              C# terminal UI
              <br />
              for real product
              <br />
              shells.
            </h1>
            <p className="home-hero__subtitle">
              Build dashboards, workflows, and workbenches in C# without a host-heavy stack,
              framework glue, or a toy-widget first impression.
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
                  <span className="home-preview__label">Evaluation</span>
                  <strong>GitConsole</strong>
                  <strong>OpsWatch</strong>
                  <strong>DataWorkbench</strong>
                </aside>
                <div className="home-preview__content">
                  <div className="home-preview__toolbar">
                    <span>Workspace</span>
                    <span>Inspect</span>
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
                      <div className="home-preview__log">$ run WorkspaceApp</div>
                      <div className="home-preview__log">$ inspect OpsWatch</div>
                      <div className="home-preview__log is-accent">$ pressure-test DataWorkbench</div>
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
          <div className="home-section__header home-section__header--compact">
            <span className="home-section__eyebrow">Flagship examples</span>
            <h2>Product pressure. Same public path.</h2>
            <p>
              Rezi keeps the landing page product-led. Prisma keeps docs quiet. Tessera should do
              both: show the densest apps quickly, then move people into docs.
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

      <section className="home-eval-band">
        <div className="container">
          <div className="home-eval-band__frame">
            <div className="home-eval-band__intro">
              <span className="home-section__eyebrow">Evaluate in order</span>
              <h2>Overview. Starters. Flagships.</h2>
              <p>
                Keep the landing page short. The real evaluation path belongs here, not stretched
                across multiple teaching sections.
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
            <div className="home-eval-band__actions">
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
