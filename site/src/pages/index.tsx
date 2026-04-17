import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import useBaseUrl from '@docusaurus/useBaseUrl';

const flagshipShots = [
  {
    title: 'OpsWatch',
    alt: 'OpsWatch operator dashboard screenshot',
    label: 'Operator dashboard',
    src: 'img/home/opswatch-card.png',
    href: '/docs/showcase',
    description: 'Telemetry panels, alert pressure, and operator actions in one dense surface.',
    frameClassName: 'home-media__frame--wide',
  },
  {
    title: 'GitConsole',
    alt: 'GitConsole workflow shell screenshot',
    label: 'Workflow shell',
    src: 'img/home/gitconsole-card.png',
    href: '/docs/showcase',
    description: 'Patch review, commit flow, and worktree context without leaving the shell.',
    frameClassName: 'home-media__frame--narrow',
  },
];

const pathCards = [
  {
    title: 'Getting Started',
    description: 'Take the shortest guided path from overview to starter examples and the first flagship apps.',
    href: '/docs/getting-started',
    cta: 'Follow the guide',
  },
  {
    title: 'API Reference',
    description: 'Jump straight into runtime, controls, layout, styling, and terminal capability details.',
    href: '/docs/api-reference',
    cta: 'Browse the surface',
  },
];

type MediaFrameProps = {
  alt: string;
  href: string;
  src: string;
  frameClassName?: string;
};

function MediaFrame({ alt, href, src, frameClassName }: MediaFrameProps) {
  const shotSrc = useBaseUrl(src);
  const [missing, setMissing] = React.useState(false);

  return (
    <Link className="home-media__link" to={href} aria-label={`Open ${alt}`}>
      <div className={`home-media__frame ${frameClassName ?? ''}`}>
        {missing ? (
          <div className="home-media__placeholder" role="img" aria-label={alt}>
            <span>Screenshot unavailable</span>
          </div>
        ) : (
          <img className="home-media__image" src={shotSrc} alt={alt} onError={() => setMissing(true)} />
        )}
      </div>
    </Link>
  );
}

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
              for product
              <br />
              surfaces that
              <br />
              get dense fast.
            </h1>
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
            <p className="home-hero__proofline">
              Three starter apps. Three flagship shells. Same public path.
            </p>
          </div>
          <HomepageHeroTerminal />
        </div>
      </div>
    </section>
  );
}

function HomepageFlagships() {
  return (
    <section className="home-section home-section--flagships">
      <div className="container">
        <div className="home-section__lead">
          <div className="home-section__header home-section__header--compact">
            <span className="home-section__eyebrow">Flagship proof</span>
            <h2>See two different kinds of product pressure.</h2>
          </div>
          <p className="home-section__summary">
            OpsWatch proves dense dashboard composition. GitConsole proves workflow-heavy review
            surfaces. Both stay on the same public path.
          </p>
        </div>
        <div className="home-flagship-grid">
          {flagshipShots.map((item, index) => (
            <article
              key={item.title}
              className={`home-flagship-card${index === 0 ? ' home-flagship-card--primary' : ' home-flagship-card--secondary'}`}>
              <MediaFrame
                alt={item.alt}
                href={item.href}
                src={item.src}
                frameClassName={item.frameClassName}
              />
              <div className="home-flagship-card__copy">
                <span className="home-pill">{item.label}</span>
                <h3>{item.title}</h3>
                <p>{item.description}</p>
                <Link className="home-link" to={item.href}>
                  Open the showcase
                </Link>
              </div>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}

function HomepagePaths() {
  return (
    <section className="home-paths">
      <div className="container">
        <div className="home-paths__frame">
          <div className="home-paths__intro">
            <span className="home-section__eyebrow">Next step</span>
            <h2>Start with the guide or jump straight into the surface map.</h2>
            <p>
              The homepage should prove the product quickly. The docs should then give you a clear
              next door instead of repeating the same pitch.
            </p>
          </div>
          <div className="home-paths__grid">
            {pathCards.map((item) => (
              <Link key={item.title} className="home-path-card" to={item.href}>
                <h3>{item.title}</h3>
                <p>{item.description}</p>
                <span className="home-path-card__cta">{item.cta}</span>
              </Link>
            ))}
          </div>
          <p className="home-paths__aside">
            Need more product proof first?{' '}
            <Link className="home-link" to="/docs/showcase">
              Browse the full showcase
            </Link>
            .
          </p>
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
        <HomepageFlagships />
        <HomepagePaths />
      </main>
    </Layout>
  );
}
