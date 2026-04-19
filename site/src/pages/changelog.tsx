import React from 'react';
import RepoMarkdownPage from '@site/src/components/RepoMarkdownPage';
import Content from '../../../CHANGELOG.md';

export default function ChangelogPage(): React.JSX.Element {
  return (
    <RepoMarkdownPage
      title="Changelog"
      description="Release notes and documentation updates for Tessera."
      Content={Content}
      hideHero
    />
  );
}
