import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import { ArrowRight, BookOpen, Boxes, Palette, Rocket, Workflow } from 'lucide-react';
import { Badge } from '@site/src/components/ui/badge';
import { Button } from '@site/src/components/ui/button';
import { SurfaceCard } from '@site/src/components/ui/surface-card';

const heroStats = [
  { value: '+34', label: 'widgets landed' },
  { value: '7', label: 'plotting controls' },
  { value: '3', label: 'starter apps' },
  { value: '3', label: 'flagship apps' },
];

const featureCards = [
  {
    kicker: 'Public path',
    title: 'Start with the minimal app model',
    description:
      'Install the package, wire one UseApp lane, and keep that same authoring path when the UI turns into dashboards, workflows, and workbenches.',
    href: '/docs/first-app',
    cta: 'Read first app',
    icon: Rocket,
  },
  {
    kicker: 'Widgets',
    title: 'Ship real surfaces, not toy demos',
    description:
      'Inputs, lists, tables, trees, dashboards, overlays, and plot widgets are documented by family so teams can map the control surface quickly.',
    href: '/docs/controls-overview',
    cta: 'Browse widgets',
    icon: Boxes,
  },
  {
    kicker: 'Theming',
    title: 'Theme-first defaults and overrides',
    description:
      'Semantic tokens, control defaults, and instance overrides stay visible in the public API instead of hiding inside runtime glue.',
    href: '/docs/theme-system',
    cta: 'See theme system',
    icon: Palette,
  },
  {
    kicker: 'Examples',
    title: 'Starter ladder to flagship proof',
    description:
      'HelloWorld, CounterForm, and WorkspaceApp lead directly into DataWorkbench, OpsWatch, and GitConsole without changing the mental model.',
    href: '/docs/showcase',
    cta: 'Open showcase',
    icon: Workflow,
  },
  {
    kicker: 'Architecture',
    title: 'Clear layer ownership',
    description:
      'The docs now break down the app model, layout, runtime, widget layers, and message flow so contributors and adopters know where responsibilities live.',
    href: '/docs/architectural-review',
    cta: 'Review architecture',
    icon: BookOpen,
  },
  {
    kicker: 'Recipes',
    title: 'Common build patterns documented',
    description:
      'App shells, refresh loops, data workspaces, and effect-driven flows are documented as recipes instead of being scattered across examples alone.',
    href: '/docs/recipes',
    cta: 'Use recipes',
    icon: ArrowRight,
  },
];

const routeCards = [
  {
    kicker: 'Fastest lane',
    title: 'Getting Started',
    description:
      'Use the guided path from overview to install, first app, starter examples, and the first flagship evaluation pass.',
    href: '/docs/getting-started',
    cta: 'Read the guide',
    featured: true,
  },
  {
    kicker: 'Surface map',
    title: 'API Reference',
    description:
      'Jump directly into runtime, layout, controls, theming, terminal behavior, and the exact public surface inventory.',
    href: '/docs/api-reference',
    cta: 'Browse the API',
  },
  {
    kicker: 'Product proof',
    title: 'Showcase',
    description:
      'Open the flagship examples, evaluation notes, and concrete screenshots when you want to assess whether Tessera holds up under pressure.',
    href: '/docs/showcase',
    cta: 'See the showcases',
  },
];

function HeroTerminal(): React.JSX.Element {
  return (
    <div className="lumina-home__terminal">
      <SurfaceCard className="lumina-home__terminal-shell">
        <div className="lumina-home__terminal-chrome">
          <div className="lumina-home__terminal-dots" aria-hidden="true">
            <span />
            <span />
            <span />
          </div>
          <span className="lumina-home__terminal-path">~/my-app -- tessera run</span>
        </div>
        <div className="lumina-home__terminal-body">
          <div className="lumina-home__terminal-line">
            <span className="lumina-home__prompt">$</span>
            <span>dotnet add package Tessera</span>
          </div>
          <div className="lumina-home__terminal-line">
            <span className="lumina-home__terminal-note">info</span>
            <span>Determining projects to restore...</span>
          </div>
          <div className="lumina-home__terminal-line">
            <span className="lumina-home__terminal-ok">ok</span>
            <span>PackageReference for package &apos;Tessera&apos; version &apos;1.0.0-alpha.1&apos; added.</span>
          </div>
          <div className="lumina-home__terminal-line">
            <span className="lumina-home__terminal-ok">ok</span>
            <span>Restored my-app.csproj in 1.21 sec.</span>
          </div>
          <div className="lumina-home__terminal-preview" aria-label="Simulated Tessera surface preview">
            <pre>{`┌─ ${'Ops floor'.padEnd(34, '─')}┐
│ ${'Build status'.padEnd(18)} ${'passing'.padEnd(12)} │
│ ${'Active alerts'.padEnd(18)} ${'03'.padEnd(12)} │
│ ${'Latency p95'.padEnd(18)} ${'18 ms'.padEnd(12)} │
└${'─'.repeat(36)}┘`}</pre>
          </div>
          <p className="lumina-home__terminal-foot">
            Same public path from the first screen into dashboard panels, workflow shells, and
            multi-pane workbenches.
          </p>
        </div>
      </SurfaceCard>
    </div>
  );
}

export default function Home(): React.JSX.Element {
  return (
    <Layout
      title="Terminal UI for serious .NET apps"
      description="Tessera is a C#-first terminal UI framework for product-grade dashboards, workflows, and workbenches.">
      <main className="lumina-home">
        <section className="lumina-home__hero">
          <div className="lumina-home__hero-inner">
            <Badge className="lumina-home__announcement">
              <span className="lumina-home__announcement-dot" aria-hidden="true" />
              Public alpha • .NET 10 • C#-first
            </Badge>
            <h1 className="lumina-home__title">
              Terminal UI, <span className="lumina-home__title-highlight">but product-shaped.</span>
            </h1>
            <p className="lumina-home__subtitle">
              Tessera is the public .NET path for dashboards, workflows, and workbenches that need
              to start simple, get dense fast, and stay on the same authoring model the whole time.
            </p>
            <div className="lumina-home__actions">
              <Button asChild size="lg">
                <Link to="/docs/getting-started">Read the docs</Link>
              </Button>
              <Button asChild variant="secondary" size="lg">
                <Link to="/docs/showcase">Browse showcases</Link>
              </Button>
              <Link className="lumina-home__ghost-link" to="/docs/api-reference">
                API reference
              </Link>
            </div>
            <div className="lumina-home__stats" aria-label="Tessera proof points">
              {heroStats.map((item) => (
                <SurfaceCard key={item.label} className="lumina-home__stat">
                  <strong>{item.value}</strong>
                  <span>{item.label}</span>
                </SurfaceCard>
              ))}
            </div>
            <HeroTerminal />
          </div>
        </section>

        <section className="lumina-home__section">
          <div className="lumina-home__section-inner">
            <header className="lumina-home__section-header">
              <Badge className="lumina-home__section-kicker">Everything you need on one path</Badge>
              <h2>From first screen to flagship shell, one public surface area.</h2>
              <p>
                Tessera keeps install, app model, widgets, theming, recipes, architecture, and
                flagship proof on the same docs path so teams can move from evaluation into real
                product work without switching frameworks or relearning the surface.
              </p>
            </header>
            <div className="lumina-home__feature-grid">
              {featureCards.map((item) => (
                <SurfaceCard key={item.title} asChild className="lumina-home__feature-card">
                  <Link to={item.href}>
                    <div className="flex items-center justify-between gap-3">
                      <span className="lumina-home__feature-kicker">{item.kicker}</span>
                      <item.icon className="h-4 w-4 text-[var(--ifm-color-primary)]" aria-hidden="true" />
                    </div>
                    <h3>{item.title}</h3>
                    <p>{item.description}</p>
                    <span className="lumina-home__feature-link">{item.cta}</span>
                  </Link>
                </SurfaceCard>
              ))}
            </div>
          </div>
        </section>

        <section className="lumina-home__cta">
          <div className="lumina-home__cta-inner">
            <SurfaceCard className="lumina-home__cta-shell">
              <div className="lumina-home__cta-copy">
                <Badge className="lumina-home__section-kicker">Choose a lane</Badge>
                <h2>Keep the pitch short. Move straight into the docs that matter.</h2>
                <p>
                  Use the guided lane when you want the shortest path to a working app, the API map
                  when you need exact type names, and the showcase when you want product-shaped
                  proof before committing deeper.
                </p>
              </div>
              <div className="lumina-home__route-grid">
                {routeCards.map((item) => (
                  <SurfaceCard
                    key={item.title}
                    className={`lumina-home__route-card ${
                      item.featured ? 'lumina-home__route-card--featured' : ''
                    }`}
                    asChild>
                    <Link to={item.href}>
                      <Badge className="lumina-home__route-kicker">{item.kicker}</Badge>
                      <h3>{item.title}</h3>
                      <p>{item.description}</p>
                      <span className="lumina-home__route-link">{item.cta}</span>
                    </Link>
                  </SurfaceCard>
                ))}
              </div>
              <p className="lumina-home__cta-foot">
                Need deeper product proof first? <Link to="/docs/showcase">Start with the full showcase.</Link>
              </p>
            </SurfaceCard>
          </div>
        </section>
      </main>
    </Layout>
  );
}
