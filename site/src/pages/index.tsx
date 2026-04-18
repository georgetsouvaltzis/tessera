import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import {
  ArrowRight,
  BookOpen,
  Boxes,
  Palette,
  Rocket,
  Search,
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
    <div className="mx-auto mt-16 max-w-4xl">
      <div className="overflow-hidden rounded-2xl neon-border animate-pulse-glow">
        <div className="flex items-center gap-2 border-b border-border/60 bg-secondary/40 px-4 py-2.5">
          <span className="h-3 w-3 rounded-full bg-red-500/80" />
          <span className="h-3 w-3 rounded-full bg-yellow-500/80" />
          <span className="h-3 w-3 rounded-full bg-green-500/80" />
          <span className="ml-3 font-mono text-xs text-muted-foreground">
            ~/projects/my-app — tessera run
          </span>
        </div>
        <pre className="overflow-x-auto bg-card/40 p-6 text-left font-mono text-sm leading-relaxed text-foreground/90">
          <span className="text-muted-foreground">$ </span>
          <span className="text-foreground">dotnet add package Tessera</span>
          {'\n'}
          <span className="text-muted-foreground">info </span>
          <span className="text-foreground">Determining projects to restore...</span>
          {'\n'}
          <span className="text-[var(--tessera-green)]">ok   </span>
          <span className="text-foreground">
            PackageReference for package &apos;Tessera&apos; version &apos;1.0.0-alpha.1&apos; added.
          </span>
          {'\n'}
          <span className="text-[var(--tessera-green)]">ok   </span>
          <span className="text-foreground">Restored my-app.csproj in 1.21 sec.</span>
          {'\n\n'}
          <span className="text-[var(--tessera-cyan)]">┌─ Ops floor ────────────────────────────────┐</span>
          {'\n'}
          <span className="text-[var(--tessera-cyan)]">│</span>{'  '}
          <span className="text-primary text-glow">●</span>{' '}
          <span className="text-foreground">Build status</span>{'      '}
          <span className="text-[var(--tessera-green)]">passing</span>{'   '}
          <span className="text-[var(--tessera-cyan)]">│</span>
          {'\n'}
          <span className="text-[var(--tessera-cyan)]">│</span>{'  '}
          <span className="text-primary text-glow">●</span>{' '}
          <span className="text-foreground">Active alerts</span>{'     '}
          <span className="text-foreground">03</span>{'        '}
          <span className="text-[var(--tessera-cyan)]">│</span>
          {'\n'}
          <span className="text-[var(--tessera-cyan)]">│</span>{'  '}
          <span className="text-primary text-glow">●</span>{' '}
          <span className="text-foreground">Latency p95</span>{'       '}
          <span className="text-foreground">18 ms</span>{'     '}
          <span className="text-[var(--tessera-cyan)]">│</span>
          {'\n'}
          <span className="text-[var(--tessera-cyan)]">└───────────────────────────────────────────┘</span>
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
      <main className="relative">
        <section className="relative overflow-hidden border-b border-border/60">
          <div className="absolute inset-0 -z-10" style={{background: 'var(--tessera-gradient-hero)'}} aria-hidden />
          <div className="absolute inset-0 -z-10 grid-bg" aria-hidden />

          <div className="mx-auto max-w-5xl px-4 pb-24 pt-24 text-center sm:px-6 sm:pt-32 lg:px-8">
            <span className="inline-flex items-center gap-2 rounded-full border border-primary/30 bg-card/60 px-3 py-1 text-xs text-muted-foreground backdrop-blur transition-colors">
              <span className="relative flex h-1.5 w-1.5">
                <span className="absolute inset-0 animate-ping rounded-full bg-primary opacity-75" />
                <span className="relative inline-flex h-1.5 w-1.5 rounded-full bg-primary" />
              </span>
              public alpha • .NET 10 • c#-first
            </span>

            <h1 className="mt-6 text-balance text-5xl font-bold tracking-tight sm:text-6xl lg:text-7xl">
              <span className="text-foreground">Terminal UI,</span>
              <br />
              <span className="text-gradient text-glow">but product-shaped.</span>
            </h1>

            <p className="mx-auto mt-6 max-w-3xl text-balance text-lg text-muted-foreground sm:text-xl">
              <strong className="text-foreground">Tessera</strong> is a C#-first terminal UI
              framework for dashboards, workflows, and workbenches. Start simple, get dense fast,
              keep the same authoring model.
            </p>

            <div className="mt-10 flex flex-wrap items-center justify-center gap-3">
              <Button asChild size="lg">
                <Link to="/docs/getting-started">
                  Read the docs
                  <ArrowRight className="h-4 w-4" />
                </Link>
              </Button>
              <Button asChild variant="secondary" size="lg">
                <Link to="/docs/showcase">Browse examples</Link>
              </Button>
              <button
                type="button"
                className="inline-flex items-center gap-2 rounded-lg border border-border bg-card/60 px-5 py-2.5 text-sm font-medium text-foreground backdrop-blur transition-colors hover:border-primary/40">
                <Search className="h-4 w-4" />
                Search docs
                <kbd className="rounded border border-border bg-secondary/60 px-1.5 py-0.5 font-mono text-[10px] text-muted-foreground">
                  ⌘K
                </kbd>
              </button>
            </div>

            <HeroTerminal />
          </div>
        </section>

        <section className="mx-auto max-w-6xl px-4 py-24 sm:px-6 lg:px-8">
          <div className="mb-12 text-center">
            <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
              <span className="text-gradient">Everything</span>{' '}
              <span className="text-foreground">you need to evaluate it fast.</span>
            </h2>
            <p className="mt-3 text-muted-foreground">
              Widgets, architecture, recipes, theming, and flagship proof. One docs path.
            </p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {featureCards.map((feature) => (
              <div
                key={feature.title}
                className="group relative rounded-2xl neon-border p-6 transition-transform hover:-translate-y-0.5">
                <div className="mb-4 inline-flex h-10 w-10 items-center justify-center rounded-lg bg-primary/15 text-primary transition-shadow group-hover:glow-primary-sm">
                  <feature.icon className="h-5 w-5" />
                </div>
                <h3 className="text-base font-semibold text-foreground">{feature.title}</h3>
                <p className="mt-1.5 text-sm leading-relaxed text-muted-foreground">{feature.text}</p>
              </div>
            ))}
          </div>
        </section>

        <section className="mx-auto max-w-4xl px-4 pb-32 sm:px-6 lg:px-8">
          <div className="relative overflow-hidden rounded-3xl neon-border p-10 text-center sm:p-14">
            <div
              className="absolute inset-0 -z-10 opacity-60"
              style={{background: 'var(--tessera-gradient-hero)'}}
              aria-hidden
            />
            <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
              <span className="text-foreground">Ready to </span>
              <span className="text-gradient text-glow">ship</span>
              <span className="text-foreground">?</span>
            </h2>
            <p className="mx-auto mt-3 max-w-xl text-muted-foreground">
              Start with the guided docs lane, jump straight into the widget map, or pressure-test
              the flagship examples before committing deeper.
            </p>
            <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
              <Button asChild size="lg">
                <Link to="/docs/getting-started">
                  Get started
                  <ArrowRight className="h-4 w-4" />
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
