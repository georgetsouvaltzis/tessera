import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import {
  ArrowRight,
  BookOpen,
  Boxes,
  Palette,
  Rocket,
  Workflow,
} from 'lucide-react';
import {Button} from '@site/src/components/ui/button';

const featureCards = [
  {
    icon: Rocket,
    title: 'Starter-first',
    text: 'Install the package, wire one UseApp lane, and keep the same authoring path when the interface turns into a real product shell.',
  },
  {
    icon: Boxes,
    title: 'Widget-rich',
    text: 'Inputs, lists, tables, trees, overlays, dashboards, and plot controls are all documented by family instead of hidden inside demos.',
  },
  {
    icon: Palette,
    title: 'Theme-first',
    text: 'Semantic tokens, control defaults, and instance overrides stay explicit in the public API instead of leaking through runtime glue.',
  },
  {
    icon: Workflow,
    title: 'Flagship proof',
    text: 'WorkspaceApp, OpsWatch, GitConsole, and DataWorkbench prove the same public path still holds when the shell gets dense fast.',
  },
  {
    icon: BookOpen,
    title: 'Docs-first',
    text: 'Architecture, recipes, widgets, troubleshooting, and reference are grouped into one path that is easier to evaluate and easier to ship from.',
  },
  {
    icon: Rocket,
    title: '+34 widgets',
    text: 'Four roadmap waves landed already, including seven plotting controls and the higher-pressure shells needed for public evaluation.',
  },
];

function HeroTerminal(): React.JSX.Element {
  return (
    <div className="lumina-home__hero-terminal-wrap">
      <div className="lumina-home__hero-terminal neon-border animate-pulse-glow">
        <div className="lumina-home__hero-terminal-bar">
          <span className="lumina-home__hero-terminal-dot lumina-home__hero-terminal-dot--red" />
          <span className="lumina-home__hero-terminal-dot lumina-home__hero-terminal-dot--yellow" />
          <span className="lumina-home__hero-terminal-dot lumina-home__hero-terminal-dot--green" />
          <span className="lumina-home__hero-terminal-path">
            ~/projects/my-app — tessera run
          </span>
        </div>
        <pre className="lumina-home__hero-terminal-code">
          <span className="lumina-home__hero-terminal-muted">$ </span>
          <span className="lumina-home__hero-terminal-strong">dotnet add package Tessera</span>
          {'\n'}
          <span className="lumina-home__hero-terminal-muted">info </span>
          <span className="lumina-home__hero-terminal-strong">Determining projects to restore...</span>
          {'\n'}
          <span className="lumina-home__hero-terminal-ok">ok   </span>
          <span className="lumina-home__hero-terminal-strong">
            PackageReference for package &apos;Tessera&apos; version &apos;1.0.0-alpha.1&apos; added.
          </span>
          {'\n'}
          <span className="lumina-home__hero-terminal-ok">ok   </span>
          <span className="lumina-home__hero-terminal-strong">Restored my-app.csproj in 1.21 sec.</span>
          {'\n\n'}
          <span className="lumina-home__hero-terminal-cyan">┌─ Ops floor ────────────────────────────────┐</span>
          {'\n'}
          <span className="lumina-home__hero-terminal-cyan">│</span>{'  '}
          <span className="lumina-home__hero-terminal-pink">●</span>{' '}
          <span className="lumina-home__hero-terminal-strong">Build status</span>{'      '}
          <span className="lumina-home__hero-terminal-ok">passing</span>{'   '}
          <span className="lumina-home__hero-terminal-cyan">│</span>
          {'\n'}
          <span className="lumina-home__hero-terminal-cyan">│</span>{'  '}
          <span className="lumina-home__hero-terminal-pink">●</span>{' '}
          <span className="lumina-home__hero-terminal-strong">Active alerts</span>{'     '}
          <span className="lumina-home__hero-terminal-strong">03</span>{'        '}
          <span className="lumina-home__hero-terminal-cyan">│</span>
          {'\n'}
          <span className="lumina-home__hero-terminal-cyan">│</span>{'  '}
          <span className="lumina-home__hero-terminal-pink">●</span>{' '}
          <span className="lumina-home__hero-terminal-strong">Latency p95</span>{'       '}
          <span className="lumina-home__hero-terminal-strong">18 ms</span>{'     '}
          <span className="lumina-home__hero-terminal-cyan">│</span>
          {'\n'}
          <span className="lumina-home__hero-terminal-cyan">└───────────────────────────────────────────┘</span>
        </pre>
      </div>
    </div>
  );
}

export default function Home(): React.JSX.Element {
  return (
    <Layout
      title="Terminal UI for serious .NET apps"
      description="Tessera is a C#-first terminal UI framework for dashboards, workflows, and workbenches that stay on one public path.">
      <main className="lumina-home">
        <section className="lumina-home__hero-shell">
          <div className="lumina-home__hero-backdrop" style={{background: 'var(--tessera-gradient-hero)'}} aria-hidden />
          <div className="lumina-home__hero-grid grid-bg" aria-hidden />

          <div className="lumina-home__hero-content">
            <span className="lumina-home__launch-pill">
              <span className="lumina-home__launch-ping-wrap">
                <span className="lumina-home__launch-ping" />
                <span className="lumina-home__launch-ping-core" />
              </span>
              public alpha • .NET 10 • c#-first
            </span>

            <h1 className="lumina-home__hero-title">
              <span className="lumina-home__hero-title-top">Terminal UI,</span>
              <br />
              <span className="lumina-home__hero-title-bottom">but product-shaped.</span>
            </h1>

            <p className="lumina-home__hero-copy">
              <strong className="lumina-home__hero-copy-strong">Tessera</strong> is a C#-first terminal UI
              framework for dashboards, workflows, and workbenches. Start simple, get dense fast,
              keep the same authoring model.
            </p>

            <div className="lumina-home__hero-actions">
              <Button asChild size="lg">
                <Link to="/docs/getting-started">
                  Read the docs
                  <ArrowRight />
                </Link>
              </Button>
            </div>

            <HeroTerminal />
          </div>
        </section>

        <section className="lumina-home__feature-section">
          <div className="lumina-home__feature-heading">
            <h2 className="lumina-home__section-title">
              <span className="lumina-home__section-title-accent">Everything</span>{' '}
              <span className="lumina-home__section-title-main">you need to evaluate it fast.</span>
            </h2>
            <p className="lumina-home__section-copy">
              Widgets, architecture, recipes, theming, and flagship proof. One docs path.
            </p>
          </div>

          <div className="lumina-home__feature-grid">
            {featureCards.map((feature) => (
              <div
                key={feature.title}
                className="lumina-home__feature-card neon-border">
                <div className="lumina-home__feature-icon">
                  <feature.icon className="lumina-home__feature-icon-glyph" />
                </div>
                <h3 className="lumina-home__feature-card-title">{feature.title}</h3>
                <p className="lumina-home__feature-card-copy">{feature.text}</p>
              </div>
            ))}
          </div>
        </section>

        <section className="lumina-home__final-cta-wrap">
          <div className="lumina-home__final-cta neon-border">
            <div
              className="lumina-home__final-cta-backdrop"
              style={{background: 'var(--tessera-gradient-hero)'}}
              aria-hidden
            />
            <h2 className="lumina-home__final-cta-title">
              <span className="lumina-home__final-cta-title-main">Ready to </span>
              <span className="lumina-home__final-cta-title-accent">ship</span>
              <span className="lumina-home__final-cta-title-main">?</span>
            </h2>
            <p className="lumina-home__final-cta-copy">
              Start with the guided docs lane, jump straight into the widget map, or pressure-test
              the flagship examples before committing deeper.
            </p>
            <div className="lumina-home__final-cta-actions">
              <Button asChild size="lg">
                <Link to="/docs/getting-started">
                  Get started
                  <ArrowRight />
                </Link>
              </Button>
              <Button asChild variant="secondary" size="lg">
                <Link to="/docs/api-reference">Browse API surface</Link>
              </Button>
            </div>
          </div>
        </section>
      </main>
    </Layout>
  );
}
