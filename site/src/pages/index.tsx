import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';

const pathCards = [
  {
    eyebrow: 'Fastest path',
    accent: 'guide',
    title: 'Getting Started',
    description: 'Take the shortest guided path from overview to starter examples and the first flagship apps.',
    href: '/docs/getting-started',
    cta: 'Follow the guide',
  },
  {
    eyebrow: 'Surface map',
    accent: 'reference',
    title: 'API Reference',
    description: 'Jump straight into runtime, controls, layout, styling, and terminal capability details.',
    href: '/docs/api-reference',
    cta: 'Browse the surface',
  },
];

const capabilityNotes = [
  {
    title: 'App shells to dense workbenches',
    description:
      'Starter flows, dashboards, workflow shells, and multi-pane investigation surfaces stay on the same public API.',
  },
  {
    title: 'Theme-first defaults',
    description:
      'Semantic tokens, shipped palettes, and override layers go from global theme to control state without hidden styling seams.',
  },
  {
    title: 'Examples with pressure',
    description:
      'The starter ladder and flagship apps show the same model holding from first screen to real software-shaped terminals.',
  },
];

const capabilityStats = [
  {
    value: '+34',
    label: 'widgets landed',
    detail: 'four roadmap waves completed',
  },
  {
    value: '7',
    label: 'plotting controls',
    detail: 'sparkline to plot panels',
  },
  {
    value: '3',
    label: 'starter apps',
    detail: 'HelloWorld to WorkspaceApp',
  },
  {
    value: '3',
    label: 'flagship apps',
    detail: 'OpsWatch, GitConsole, DataWorkbench',
  },
];

const installConsoleLines = [
  {
    kind: 'command',
    prefix: '$',
    text: 'dotnet add package Tessera',
  },
  {
    kind: 'output',
    prefix: 'info',
    text: 'Determining projects to restore...',
  },
  {
    kind: 'output',
    prefix: 'ok',
    text: "PackageReference for package 'Tessera' version '1.0.0-alpha.1' added.",
  },
  {
    kind: 'output',
    prefix: 'ok',
    text: 'Restored my-app.csproj (in 1.21 sec).',
  },
];

function HomepageHeroTerminal() {
  return (
    <div className="home-terminal" aria-label="Animated terminal setup example">
      <div className="home-terminal__chrome">
        <div className="home-terminal__dots" aria-hidden="true">
          <span />
          <span />
          <span />
        </div>
        <span className="home-terminal__path">Program.cs</span>
      </div>
      <div className="home-terminal__body">
        <div className="home-terminal__line home-terminal__line--command home-terminal__line--first">
          <span className="home-terminal__gutter">01</span>
          <span className="home-terminal__text">
            <span className="home-terminal__token home-terminal__token--keyword">using</span>{' '}
            <span className="home-terminal__token home-terminal__token--namespace">Tessera</span>
            <span className="home-terminal__token home-terminal__token--punctuation">;</span>
          </span>
        </div>
        <div className="home-terminal__line home-terminal__line--command home-terminal__line--second">
          <span className="home-terminal__gutter">02</span>
          <span className="home-terminal__text">
            <span className="home-terminal__token home-terminal__token--keyword">var</span>{' '}
            <span className="home-terminal__token home-terminal__token--identifier">app</span>{' '}
            <span className="home-terminal__token home-terminal__token--operator">=</span>{' '}
            <span className="home-terminal__token home-terminal__token--type">
              TesseraApplication
            </span>
            <span className="home-terminal__token home-terminal__token--punctuation">.</span>
            <span className="home-terminal__token home-terminal__token--method">
              CreateBuilder
            </span>
            <span className="home-terminal__token home-terminal__token--punctuation">()</span>
          </span>
        </div>
        <div className="home-terminal__line home-terminal__line--command home-terminal__line--third home-terminal__line--chained">
          <span className="home-terminal__gutter">03</span>
          <span className="home-terminal__text">
            <span className="home-terminal__indent" aria-hidden="true" />
            <span className="home-terminal__token home-terminal__token--punctuation">.</span>
            <span className="home-terminal__token home-terminal__token--method">UseApp</span>
            <span className="home-terminal__token home-terminal__token--punctuation">&lt;</span>
            <span className="home-terminal__token home-terminal__token--type">OrdersApp</span>
            <span className="home-terminal__token home-terminal__token--punctuation">&gt;()</span>
          </span>
        </div>
        <div className="home-terminal__line home-terminal__line--command home-terminal__line--fourth home-terminal__line--chained">
          <span className="home-terminal__gutter">04</span>
          <span className="home-terminal__text">
            <span className="home-terminal__indent" aria-hidden="true" />
            <span className="home-terminal__token home-terminal__token--punctuation">.</span>
            <span className="home-terminal__token home-terminal__token--method">Build</span>
            <span className="home-terminal__token home-terminal__token--punctuation">();</span>
          </span>
        </div>
        <div className="home-terminal__line home-terminal__line--command home-terminal__line--fifth">
          <span className="home-terminal__gutter">05</span>
          <span className="home-terminal__text">
            <span className="home-terminal__token home-terminal__token--keyword">await</span>{' '}
            <span className="home-terminal__token home-terminal__token--identifier">app</span>
            <span className="home-terminal__token home-terminal__token--punctuation">.</span>
            <span className="home-terminal__token home-terminal__token--method">RunAsync</span>
            <span className="home-terminal__token home-terminal__token--punctuation">();</span>
            <span className="home-terminal__cursor" aria-hidden="true" />
          </span>
        </div>
        <div className="home-terminal__status">
          <span className="home-pill home-pill--soft">UseApp sample</span>
          <strong>OrdersApp ready</strong>
          <div className="home-terminal__surface" aria-hidden="true">
            <div className="home-terminal__surface-bar">
              <span>Orders</span>
              <span>Starter theme</span>
            </div>
            <div className="home-terminal__surface-grid">
              <div className="home-terminal__surface-card">
                <span>Orders</span>
                <strong>127</strong>
              </div>
              <div className="home-terminal__surface-card">
                <span>Latency</span>
                <strong>18ms</strong>
              </div>
              <div className="home-terminal__surface-card">
                <span>Queue</span>
                <strong>03</strong>
              </div>
            </div>
          </div>
          <p>Minimal startup lane reused across the docs before you move into the denser showcase apps.</p>
        </div>
      </div>
    </div>
  );
}

function HomepageInstallConsole() {
  return (
    <article className="home-console-card home-console-card--install" aria-label="Simulated package install">
      <div className="home-console-card__chrome">
        <div className="home-console-card__dots" aria-hidden="true">
          <span />
          <span />
          <span />
        </div>
        <span className="home-console-card__path">~/my-app</span>
      </div>
      <div className="home-console-card__body">
        {installConsoleLines.map((line) => (
          <div
            key={`${line.prefix}-${line.text}`}
            className={`home-console-card__line home-console-card__line--${line.kind}`}>
            <span className="home-console-card__prefix">{line.prefix}</span>
            <span>{line.text}</span>
          </div>
        ))}
      </div>
      <div className="home-console-card__footer">
        <span className="home-pill home-pill--soft">Package install</span>
        <strong>Minimal setup, real API path.</strong>
      </div>
    </article>
  );
}

function HomepageHero() {
  return (
    <section className="home-hero">
      <div className="container">
        <div className="home-hero__grid">
          <div className="home-hero__copy">
            <span className="home-badge">Public alpha • .NET 10 • C#-first</span>
            <h1 className="home-hero__title">Terminal UI for serious product surfaces that get dense fast.</h1>
            <p className="home-hero__subtitle">
              Build dashboards, workflows, and workbenches in C# without a host-heavy stack or a
              starter path that falls apart once the UI stops being simple.
            </p>
            <div className="home-hero__actions">
              <Link className="button button--primary button--lg" to="/docs/getting-started">
                Get Started
              </Link>
              <Link className="button button--secondary button--lg" to="/docs/showcase">
                See Flagships
              </Link>
              <Link className="home-link" to="/docs/api-reference">
                API Reference
              </Link>
            </div>
            <div className="home-hero__signalbar" aria-label="Product proof points">
              {capabilityStats.slice(0, 3).map((item) => (
                <article key={item.label} className="home-hero__signal">
                  <strong>{item.value}</strong>
                  <span>{item.label}</span>
                </article>
              ))}
            </div>
            <p className="home-hero__proofline">
              Three starter apps. Three flagship shells. One public path that keeps its shape.
            </p>
          </div>
          <HomepageHeroTerminal />
        </div>
      </div>
    </section>
  );
}

function HomepageProof() {
  return (
    <section className="home-section home-section--proof">
      <div className="container">
        <div className="home-proof-shell">
          <div className="home-proof-story">
            <div className="home-section__header home-section__header--compact">
              <span className="home-section__eyebrow">Why it sells</span>
              <h2>Start on the guide. Still hold up when the terminal turns into real software.</h2>
            </div>
            <p className="home-section__summary">
              Tessera covers forms, overlays, logs, traces, plotting, dashboards, and workspace
              composition without pushing teams onto a second authoring model once the UI gets operational.
            </p>
            <div className="home-proof-stats" aria-label="Tessera selling points">
              {capabilityStats.map((item) => (
                <article key={item.label} className="home-proof-stat">
                  <span>{item.label}</span>
                  <strong>{item.value}</strong>
                  <p>{item.detail}</p>
                </article>
              ))}
            </div>
          </div>
          <div className="home-proof-stack">
            <HomepageInstallConsole />
            <div className="home-proof-notes">
              {capabilityNotes.map((item) => (
                <article key={item.title} className="home-proof-note">
                  <h3>{item.title}</h3>
                  <p>{item.description}</p>
                </article>
              ))}
            </div>
            <Link className="home-link home-proof-stack__link" to="/docs/showcase">
              Explore the showcase
            </Link>
          </div>
        </div>
      </div>
    </section>
  );
}

function HomepageRoutes() {
  return (
    <section className="home-routes">
      <div className="container">
        <div className="home-routes__frame">
          <div className="home-routes__intro">
            <span className="home-section__eyebrow">Choose a lane</span>
            <h2>Keep the pitch short. Move into docs with intent.</h2>
            <p>
              The homepage should prove the product quickly, then hand off to a clear next door
              instead of repeating the same story in a flatter layout.
            </p>
          </div>
          <div className="home-routes__grid">
            {pathCards.map((item) => (
              <Link
                key={item.title}
                className={`home-route-card home-route-card--${item.accent}`}
                to={item.href}>
                <span className="home-route-card__eyebrow">{item.eyebrow}</span>
                <h3>{item.title}</h3>
                <p>{item.description}</p>
                <span className="home-route-card__cta">{item.cta}</span>
              </Link>
            ))}
          </div>
          <div className="home-routes__aside">
            <span className="home-pill home-pill--soft">Still evaluating?</span>
            <p>
              Need more product proof first?{' '}
              <Link className="home-link" to="/docs/showcase">
                Browse the full showcase
              </Link>
              .
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}

export default function Home(): React.JSX.Element {
  return (
    <Layout
      title="Terminal UI for .NET"
      description="Tessera is a C#-first terminal UI framework for serious application surfaces.">
      <HomepageHero />
      <main>
        <HomepageProof />
        <HomepageRoutes />
      </main>
    </Layout>
  );
}
