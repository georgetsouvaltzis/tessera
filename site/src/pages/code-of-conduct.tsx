import React from 'react';
import RepoMarkdownPage from '@site/src/components/RepoMarkdownPage';
import Content from '../../../CODE_OF_CONDUCT.md';

export default function CodeOfConductPage(): React.JSX.Element {
  return (
    <RepoMarkdownPage
      title="Code of Conduct"
      description="Code of conduct for the Tessera community."
      Content={Content}
    />
  );
}
