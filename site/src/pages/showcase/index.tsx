import React from 'react';
import Heading from '@theme/Heading';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import styles from './styles.module.css';

const items = [
  {
    name: 'DeployConsole',
    description: 'Multi-pane deployment workflow with search, environment selection, spinner state, and event feeds.',
    href: '/docs/examples/deploy-console',
  },
  {
    name: 'Choice',
    description: 'Focused dropdown interaction with explicit glyph and border styling.',
    href: '/docs/examples/choice',
  },
  {
    name: 'DataForm',
    description: 'Typed model editing, validation, selection, and commit flow.',
    href: '/docs/examples/data-form',
  },
  {
    name: 'TokenEditor',
    description: 'Structured token editing with keyboard and pointer selection.',
    href: '/docs/examples/token-editor',
  },
];

export default function ShowcasePage() {
  return (
    <Layout title="Showcase" description="Representative Tessera examples and workflows.">
      <main className={styles.page}>
        <div className={styles.shell}>
          <header className={styles.header}>
            <p className={styles.kicker}>Showcase</p>
            <Heading as="h1">Representative Tessera workflows</Heading>
            <p>
              The starter site keeps the showcase curated: a few examples that demonstrate layout depth, control polish, and operator-oriented UX.
            </p>
          </header>

          <section className={styles.grid}>
            {items.map((item) => (
              <article key={item.name} className={styles.card}>
                <Heading as="h2">{item.name}</Heading>
                <p>{item.description}</p>
                <Link to={item.href}>Open example page</Link>
              </article>
            ))}
          </section>
        </div>
      </main>
    </Layout>
  );
}
