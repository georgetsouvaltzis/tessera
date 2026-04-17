import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import useBaseUrl from '@docusaurus/useBaseUrl';

const proofItems = ['.NET 10', 'C#-first API', 'Starter ladder', 'Flagship apps'];

const heroShot = {
  title: 'DataWorkbench',
  alt: 'DataWorkbench flagship workbench screenshot',
  role: 'Hero screenshot',
  src: 'img/home/dataworkbench-hero.png',
  href: '/docs/showcase',
  description: 'Investigation workspace with rails, tabs, result grid, and inspector.',
  command: 'dotnet run --project examples/DataWorkbench/DataWorkbench.csproj --no-build',
  capture: '176x48 terminal • 16:10 crop',
};

const flagshipShots = [
  {
    title: 'OpsWatch',
    alt: 'OpsWatch operator dashboard screenshot',
    role: 'Operator dashboard',
    src: 'img/home/opswatch-card.png',
    href: '/docs/showcase',
    description: 'Telemetry cards, charts, and incident feed in one operator surface.',
    command: 'dotnet run --project examples/OpsWatch/OpsWatch.csproj --no-build',
    capture: '176x48 terminal • 16:10 crop',
  },
  {
    title: 'GitConsole',
    alt: 'GitConsole workflow shell screenshot',
    role: 'Workflow shell',
    src: 'img/home/gitconsole-card.png',
    href: '/docs/showcase',
    description: 'Patch deck, commit flow, and worktree review in a denser command surface.',
    command: 'dotnet run --project examples/GitConsole/GitConsole.csproj --no-build',
    capture: '176x48 terminal • 4:3 crop',
  },
];

const evaluationSteps = [
  {
    step: '01',
    title: 'Read the overview',
    description: 'Boundaries, app model, and the public path.',
    href: '/docs/overview',
  },
  {
    step: '02',
    title: 'Run the starters',
    description: 'HelloWorld, CounterForm, then WorkspaceApp.',
    href: '/docs/getting-started',
  },
  {
    step: '03',
    title: 'Pressure-test the flagships',
    description: 'Open DataWorkbench, OpsWatch, and GitConsole on the same API model.',
    href: '/docs/showcase',
  },
];

type ScreenshotSlotProps = {
  title: string;
  alt: string;
  role: string;
  src: string;
  href: string;
  description: string;
  command: string;
  capture: string;
  aspectClassName?: string;
  compact?: boolean;
};

function ScreenshotSlot({
  title,
  alt,
  role,
  src,
  href,
  description,
  command,
  capture,
  aspectClassName,
  compact = false,
}: ScreenshotSlotProps) {
  const [missing, setMissing] = React.useState(false);
  const shotSrc = useBaseUrl(src);

  return (
    <article className={`home-shot${compact ? ' home-shot--compact' : ''}`}>
      <Link className="home-shot__frame-link" to={href} aria-label={`Open ${title} showcase page`}>
        <div className={`home-shot__frame ${aspectClassName ?? ''}`}>
          {missing ? (
            <div className="home-shot__placeholder" role="img" aria-label={alt}>
              <span className="home-shot__placeholder-badge">{role}</span>
              <strong>{title}</strong>
              <p>{description}</p>
              <code>{capture}</code>
            </div>
          ) : (
            <img className="home-shot__image" src={shotSrc} alt={alt} onError={() => setMissing(true)} />
          )}
        </div>
      </Link>
      <div className="home-shot__meta">
        <div className="home-shot__copy">
          <span className="home-shot__eyebrow">{role}</span>
          <h3>{title}</h3>
          <p>{description}</p>
        </div>
        <div className="home-shot__foot">
          <code>{command}</code>
          <span>{capture}</span>
        </div>
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
            <h1 className="home-hero__title">
              Terminal UI
              <br />
              for dashboards,
              <br />
              workflows, and
              <br />
              workbenches.
            </h1>
            <p className="home-hero__subtitle">
              Build product-grade terminal surfaces in C# without a host-heavy stack, placeholder
              chrome, or a starter path that collapses under real complexity.
            </p>
            <div className="home-hero__actions">
              <Link className="button button--primary button--lg" to="/docs/getting-started">
                Get Started
              </Link>
              <Link className="button button--secondary button--lg" to="/docs/showcase">
                Browse Showcase
              </Link>
            </div>
            <p className="home-hero__proofline">
              Small public surface. Real seeded apps. Same mental model from first screen to
              flagship shell.
            </p>
          </div>
          <ScreenshotSlot {...heroShot} aspectClassName="home-shot__frame--hero" />
        </div>
        <div className="home-proof-rail" aria-label="Homepage proof points">
          {proofItems.map((item) => (
            <span key={item}>{item}</span>
          ))}
        </div>
      </div>
    </section>
  );
}

function HomepageContent() {
  return (
    <main>
      <section className="home-section home-section--tight">
        <div className="container">
          <div className="home-section__lead">
            <div className="home-section__header home-section__header--compact">
              <span className="home-section__eyebrow">Flagship surfaces</span>
              <h2>Open the dense apps early.</h2>
            </div>
            <p className="home-section__summary">
              The landing page should prove the flagship shells fast. Docs can explain the path
              afterward.
            </p>
          </div>
          <div className="home-flagship-grid">
            {flagshipShots.map((item) => (
              <ScreenshotSlot key={item.title} {...item} compact aspectClassName="home-shot__frame--card" />
            ))}
          </div>
        </div>
      </section>

      <section className="home-eval-band">
        <div className="container">
          <div className="home-eval-band__frame">
            <div className="home-eval-band__header">
              <div className="home-eval-band__intro">
                <span className="home-section__eyebrow">Evaluate in order</span>
                <h2>Overview. Starters. Flagships.</h2>
                <p>
                  Keep the homepage short. Read the contract, run the starter ladder, then decide
                  on the flagship apps.
                </p>
              </div>
              <div className="home-eval-band__actions">
                <Link className="button button--primary button--lg" to="/docs/getting-started">
                  Get Started
                </Link>
                <Link className="button button--secondary button--lg" to="/docs/showcase">
                  View Showcase
                </Link>
              </div>
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
