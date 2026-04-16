import React from 'react';
import RepoMarkdownPage from '@site/src/components/RepoMarkdownPage';
import Content from '../../../SUPPORT.md';

export default function SupportPage(): React.JSX.Element {
  return (
    <RepoMarkdownPage
      title="Support"
      description="Support guidance for Tessera."
      Content={Content}
    />
  );
}
