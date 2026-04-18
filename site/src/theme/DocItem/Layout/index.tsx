import React from 'react';
import {useDoc} from '@docusaurus/plugin-content-docs/client';
import ContentVisibility from '@theme/ContentVisibility';
import DocBreadcrumbs from '@theme/DocBreadcrumbs';
import DocItemContent from '@theme/DocItem/Content';
import DocItemFooter from '@theme/DocItem/Footer';
import DocItemPaginator from '@theme/DocItem/Paginator';
import DocVersionBadge from '@theme/DocVersionBadge';
import DocVersionBanner from '@theme/DocVersionBanner';

export default function DocItemLayout({
  children,
}: {
  children: React.ReactNode;
}): React.JSX.Element {
  const {metadata} = useDoc();

  return (
    <div className="lumina-docs-page">
      <ContentVisibility metadata={metadata} />
      <DocVersionBanner />
      <article className="lumina-docs-article">
        <DocBreadcrumbs />
        <DocVersionBadge />
        <DocItemContent>{children}</DocItemContent>
        <DocItemFooter />
      </article>
      <DocItemPaginator />
    </div>
  );
}
