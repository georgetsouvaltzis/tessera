import React from 'react';
import Layout from '@theme/Layout';
import SearchBar from '@theme/SearchBar';

export default function SearchPage(): React.JSX.Element {
  return (
    <Layout title="Search" description="Search Tessera docs">
      <main className="repo-markdown-page">
        <div className="container">
          <div className="repo-markdown-page__shell">
            <section className="repo-markdown-page__hero">
              <h1>Search docs</h1>
              <p>Use the global docs index to find API names, guides, and examples.</p>
            </section>
            <section className="repo-markdown-page__content">
              <SearchBar />
            </section>
          </div>
        </div>
      </main>
    </Layout>
  );
}
