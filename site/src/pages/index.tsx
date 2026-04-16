import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';

const features = [
  {
    title: 'Small Public API',
    description:
      'Start with Tessera, Tessera.Controls, Tessera.Layout, and Tessera.Styles without committing to host-heavy setup.',
  },
  {
    title: 'Docs-First Evaluation Path',
    description:
      'Run the starter ladder first, then move into the flagship examples when you want to see denser product surfaces.',
  },
  {
    title: 'Theme-Aware By Design',
    description:
      'The public path includes theme tokens, control defaults, instance overrides, and state styling from the start.',
  },
];

function HomepageHeader() {
  const { siteConfig } = useDocusaurusContext();

  return (
    <header className="hero hero--primary">
      <div className="container">
        <h1 className="hero__title">{siteConfig.title}</h1>
        <p className="hero__subtitle">
          C# terminal UI for teams shipping real product shells.
        </p>
        <div>
          <Link
            className="button button--primary button--lg margin-right--sm"
            to="/docs/getting-started">
            Get Started
          </Link>
          <Link className="button button--secondary button--lg" to="/docs/showcase">
            Browse Showcase
          </Link>
        </div>
      </div>
    </header>
  );
}

function HomepageFeatures() {
  return (
    <main>
      <section className="container margin-vert--xl">
        <div className="row">
          {features.map((feature) => (
            <div key={feature.title} className="col col--4 margin-bottom--lg">
              <div className="card">
                <div className="card__body">
                  <h2>{feature.title}</h2>
                  <p>{feature.description}</p>
                </div>
              </div>
            </div>
          ))}
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
      <HomepageHeader />
      <HomepageFeatures />
    </Layout>
  );
}
