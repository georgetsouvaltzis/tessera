import React from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import styles from './index.module.css';

const controlHighlights = [
  'Choice',
  'DataForm<TModel>',
  'CommandPalette',
  'TokenEditor',
  'Heatmap',
  'DockWorkspace',
];

const showcaseCards = [
  {
    title: 'DeployConsole',
    body: 'Operator-style layout with search, environment switching, periodic updates, and event feeds.',
  },
  {
    title: 'NotificationInbox',
    body: 'Dense notification workflow with read, pin, delete, and state-aware styling.',
  },
  {
    title: 'TokenEditor',
    body: 'Fast structured editing for labels, tags, and workflow metadata.',
  },
];

const codeSample = `using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<CounterApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.MaxFps = 60;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Counter",
        };
    })
    .Build();

await app.RunAsync();`;

function TerminalPanel() {
  return (
    <div className={styles.terminalFrame}>
      <div className={styles.terminalBar}>
        <span />
        <span />
        <span />
      </div>
      <pre className={styles.terminalBody}>
        <code>{`$ dotnet run

┌ Deploy ──────────────────────────────────────┐
│ env: prod      status: verifying             │
│ search: billing                              │
├ Services ───────────────┬ Events ───────────┤
│ › billing-api           │ build ok          │
│   edge-gateway          │ deploy started    │
│   worker-queue          │ health restored   │
└─────────────────────────┴───────────────────┘`}</code>
      </pre>
    </div>
  );
}

export default function Home() {
  return (
    <Layout
      title="Premium terminal UI for .NET"
      description="TeaSharp is a C#-native terminal UI framework for modern, themeable, state-driven terminal apps."
    >
      <main className={styles.page}>
        <section className={styles.hero}>
          <div className={styles.heroCopy}>
            <p className={styles.kicker}>TeaSharp for .NET 10</p>
            <Heading as="h1" className={styles.heroTitle}>
              Premium terminal UI.
              <br />
              C# first. Theme first.
            </Heading>
            <p className={styles.heroBody}>
              Build modern terminal apps with a small public API, strong control surface, and an aesthetic that feels deliberate instead of retro by accident.
            </p>
            <div className={styles.heroActions}>
              <Link className="button button--primary button--lg" to="/docs/getting-started">
                Get Started
              </Link>
              <Link className="button button--secondary button--lg" to="/showcase">
                Browse Showcase
              </Link>
            </div>
            <ul className={styles.proofBar}>
              <li>.NET 10</li>
              <li>Typed messages and effects</li>
              <li>Theme tokens and overrides</li>
              <li>Dashboard-ready controls</li>
            </ul>
          </div>
          <TerminalPanel />
        </section>

        <section className={styles.section}>
          <div className={styles.sectionHeader}>
            <p className={styles.kicker}>Why TeaSharp</p>
            <Heading as="h2">A public path built for real terminal products</Heading>
          </div>
          <div className={styles.featureGrid}>
            <article>
              <Heading as="h3">Small default surface</Heading>
              <p>`TeaApp`, `Tea.RunAsync(...)`, `Tea.CreateBuilder()`, `Screen.Build(...)`, and first-class controls.</p>
            </article>
            <article>
              <Heading as="h3">Modern TUI aesthetic</Heading>
              <p>Theme tokens, focus-aware visuals, border and glyph hooks, and polished control defaults.</p>
            </article>
            <article>
              <Heading as="h3">Workflow depth</Heading>
              <p>Controls for forms, overlays, dashboards, workspaces, and operator tooling instead of prompt helpers alone.</p>
            </article>
          </div>
        </section>

        <section className={clsx(styles.section, styles.codeSection)}>
          <div>
            <p className={styles.kicker}>Starter API</p>
            <Heading as="h2">The first app stays readable</Heading>
            <p>
              TeaSharp favors explicit object models and shallow composition over nested mini-DSLs.
            </p>
          </div>
          <pre className={styles.codeBlock}>
            <code>{codeSample}</code>
          </pre>
        </section>

        <section className={styles.section}>
          <div className={styles.sectionHeader}>
            <p className={styles.kicker}>Controls</p>
            <Heading as="h2">Built for dense terminal workflows</Heading>
          </div>
          <div className={styles.pillGrid}>
            {controlHighlights.map((item) => (
              <span key={item} className={styles.pill}>
                {item}
              </span>
            ))}
          </div>
          <Link className={styles.inlineLink} to="/docs/controls">
            Explore control families
          </Link>
        </section>

        <section className={styles.section}>
          <div className={styles.sectionHeader}>
            <p className={styles.kicker}>Examples</p>
            <Heading as="h2">Show real workflows, not toy snippets</Heading>
          </div>
          <div className={styles.showcaseGrid}>
            {showcaseCards.map((card) => (
              <article key={card.title} className={styles.showcaseCard}>
                <Heading as="h3">{card.title}</Heading>
                <p>{card.body}</p>
              </article>
            ))}
          </div>
        </section>
      </main>
    </Layout>
  );
}
