import React, { type ComponentType } from 'react';
import Layout from '@theme/Layout';

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
      <main className="container margin-vert--lg">
        <div className="row">
          <div className="col col--10 col--offset-1">
            <article className="markdown">
              <Content />
            </article>
          </div>
        </div>
      </main>
    </Layout>
  );
}
