import React from 'react';
import Link from '@docusaurus/Link';
import {translate} from '@docusaurus/Translate';
import type {
  PropSidebar,
  PropSidebarItem,
  PropSidebarItemCategory,
} from '@docusaurus/plugin-content-docs';
import type {Props} from '@theme/DocSidebar/Desktop/Content';

type SidebarLinkItem = PropSidebarItem & {
  href?: string;
  label: string;
};

function isCategory(item: PropSidebarItem): item is PropSidebarItemCategory {
  return item.type === 'category';
}

function isLinkItem(item: PropSidebarItem): item is SidebarLinkItem {
  return item.type !== 'html' && item.type !== 'category' && 'label' in item;
}

function isActive(href: string | undefined, pathname: string): boolean {
  if (!href) {
    return false;
  }

  return pathname === href || pathname.startsWith(`${href}/`);
}

function RenderLink({
  href,
  isCurrent,
  label,
}: {
  href?: string;
  isCurrent: boolean;
  label: string;
}): React.JSX.Element {
  const className = `lumina-docs-sidebar__link ${
    isCurrent ? 'lumina-docs-sidebar__link--active' : ''
  }`;

  if (!href) {
    return <span className={className}>{label}</span>;
  }

  return (
    <Link className={className} to={href}>
      {label}
    </Link>
  );
}

function RenderItems({
  items,
  pathname,
}: {
  items: PropSidebarItem[];
  pathname: string;
}): React.JSX.Element {
  return (
    <ul className="lumina-docs-sidebar__list">
      {items.map((item) => {
        if (isCategory(item)) {
          const categoryActive = isActive(item.href, pathname);

          return (
            <li key={`${item.label}-${item.href ?? 'category'}`} className="lumina-docs-sidebar__item">
              <div className="lumina-docs-sidebar__subsection">
                <RenderLink href={item.href} isCurrent={categoryActive} label={item.label} />
              </div>
              {item.items?.length ? (
                <RenderItems items={item.items} pathname={pathname} />
              ) : null}
            </li>
          );
        }

        if (!isLinkItem(item)) {
          return null;
        }

        return (
          <li key={`${item.label}-${item.href ?? 'link'}`} className="lumina-docs-sidebar__item">
            <RenderLink
              href={item.href}
              isCurrent={isActive(item.href, pathname)}
              label={item.label}
            />
          </li>
        );
      })}
    </ul>
  );
}

export default function DocSidebarDesktopContent({
  path,
  sidebar,
}: Props): React.JSX.Element | null {
  if (!sidebar) {
    return null;
  }

  return (
    <nav
      aria-label={translate({
        id: 'theme.docs.sidebar.navAriaLabel',
        message: 'Docs sidebar',
        description: 'The ARIA label for the sidebar navigation',
      })}
      className="lumina-docs-sidebar">
      {sidebar.map((item) => {
        if (isCategory(item)) {
          const categoryActive = isActive(item.href, path);

          return (
            <section
              key={`${item.label}-${item.href ?? 'section'}`}
              className="lumina-docs-sidebar__section">
              <div className="lumina-docs-sidebar__section-title">
                <RenderLink
                  href={item.href}
                  isCurrent={categoryActive}
                  label={item.label}
                />
              </div>
              {item.items?.length ? (
                <RenderItems items={item.items} pathname={path} />
              ) : null}
            </section>
          );
        }

        if (!isLinkItem(item)) {
          return null;
        }

        return (
          <section
            key={`${item.label}-${item.href ?? 'section-link'}`}
            className="lumina-docs-sidebar__section">
            <RenderLink
              href={item.href}
              isCurrent={isActive(item.href, path)}
              label={item.label}
            />
          </section>
        );
      })}
    </nav>
  );
}
