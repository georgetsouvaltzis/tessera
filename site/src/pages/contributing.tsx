import React from 'react';
import RepoMarkdownPage from '@site/src/components/RepoMarkdownPage';
import Content from '../../../CONTRIBUTING.md';

export default function ContributingPage(): React.JSX.Element {
  return (
    <RepoMarkdownPage
      title="Contributing"
      description="Contribution guidelines for Tessera."
      Content={Content}
    />
  );
}
