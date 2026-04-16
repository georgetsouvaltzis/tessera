import React from 'react';
import RepoMarkdownPage from '@site/src/components/RepoMarkdownPage';
import Content from '../../../SECURITY.md';

export default function SecurityPage(): React.JSX.Element {
  return (
    <RepoMarkdownPage
      title="Security"
      description="Security policy and reporting guidance for Tessera."
      Content={Content}
    />
  );
}
