import React, { type ComponentType } from 'react';
import Layout from '@theme/Layout';
import { Badge } from '@site/src/components/ui/badge';
import { SurfaceCard } from '@site/src/components/ui/surface-card';

type RepoMarkdownPageProps = {
  title: string;
  description?: string;
  Content: ComponentType;
};

export default function RepoMarkdownPage({
  title,
  description,
  Content,
}: RepoMarkdownPageProps): React.JSX.Element {
  return (
    <Layout title={title} description={description}>
      <main className="repo-markdown-page">
        <div className="container">
          <div className="repo-markdown-page__shell">
            <SurfaceCard className="repo-markdown-page__hero">
              <Badge className="repo-markdown-page__eyebrow">Project page</Badge>
              <h1>{title}</h1>
              {description ? <p>{description}</p> : null}
            </SurfaceCard>
            <SurfaceCard asChild className="repo-markdown-page__content">
              <article className="markdown">
                <Content />
              </article>
            </SurfaceCard>
          </div>
        </div>
      </main>
    </Layout>
  );
}
