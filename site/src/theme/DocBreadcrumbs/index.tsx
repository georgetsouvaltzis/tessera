import React from 'react';
import Link from '@docusaurus/Link';
import {useSidebarBreadcrumbs} from '@docusaurus/plugin-content-docs/client';

export default function DocBreadcrumbs(): React.JSX.Element | null {
  const breadcrumbs = useSidebarBreadcrumbs();

  if (!breadcrumbs) {
    return null;
  }

  return (
    <nav aria-label="Breadcrumbs" className="lumina-docs-breadcrumbs">
      <Link className="lumina-docs-breadcrumbs__link" to="/">
        Home
      </Link>
      <span className="lumina-docs-breadcrumbs__sep">›</span>
      <span className="lumina-docs-breadcrumbs__link">Docs</span>
      {breadcrumbs.map((item, index) => {
        const isLast = index === breadcrumbs.length - 1;
        const href = item.type === 'category' && item.linkUnlisted ? undefined : item.href;

        return (
          <React.Fragment key={`${item.label}-${index}`}>
            <span className="lumina-docs-breadcrumbs__sep">›</span>
            {href && !isLast ? (
              <Link className="lumina-docs-breadcrumbs__link" href={href}>
                {item.label}
              </Link>
            ) : (
              <span
                className={`lumina-docs-breadcrumbs__link ${
                  isLast ? 'lumina-docs-breadcrumbs__link--current' : ''
                }`}>
                {item.label}
              </span>
            )}
          </React.Fragment>
        );
      })}
    </nav>
  );
}
