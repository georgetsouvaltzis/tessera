import React, {type ReactNode} from 'react';
import Link from '@docusaurus/Link';
import useBaseUrl from '@docusaurus/useBaseUrl';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import {useThemeConfig} from '@docusaurus/theme-common';
import {splitNavbarItems, useNavbarMobileSidebar} from '@docusaurus/theme-common/internal';
import {useLocation} from '@docusaurus/router';
import NavbarMobileSidebarToggle from '@theme/Navbar/MobileSidebar/Toggle';

type NavbarConfigItem = {
  activeBaseRegex?: string;
  href?: string;
  label?: string;
  position?: 'left' | 'right';
  to?: string;
  type?: string;
};

function useNavbarItems(): NavbarConfigItem[] {
  return (useThemeConfig().navbar.items ?? []) as NavbarConfigItem[];
}

function isActive(item: NavbarConfigItem, pathname: string): boolean {
  if (item.activeBaseRegex) {
    return new RegExp(item.activeBaseRegex).test(pathname);
  }

  if (item.to) {
    return pathname === item.to || pathname.startsWith(`${item.to}/`);
  }

  return false;
}

function NavbarLink({item, pathname}: {item: NavbarConfigItem; pathname: string}): ReactNode {
  if (!item.label) {
    return null;
  }

  const className = `lumina-navbar__link ${
    isActive(item, pathname) ? 'lumina-navbar__link--active' : ''
  }`;

  if (item.to) {
    return (
      <Link key={`${item.label}-${item.to}`} className={className} to={item.to}>
        {item.label}
      </Link>
    );
  }

  if (item.href) {
    return (
      <Link key={`${item.label}-${item.href}`} className={className} href={item.href}>
        {item.label}
      </Link>
    );
  }

  return null;
}

export default function NavbarContent(): ReactNode {
  const mobileSidebar = useNavbarMobileSidebar();
  const items = useNavbarItems().filter((item) => item.type !== 'search');
  const [leftItems, rightItems] = splitNavbarItems(items);
  const {pathname} = useLocation();
  const {siteConfig} = useDocusaurusContext();
  const logoSrc = useBaseUrl('/img/tessera-nuget-icon.png');
  const githubItem = rightItems.find((item) => item.href);
  const normalizedPath = pathname.startsWith(siteConfig.baseUrl)
    ? pathname.slice(siteConfig.baseUrl.length - 1)
    : pathname;

  return (
    <div className="lumina-navbar__inner">
      {!mobileSidebar.disabled && (
        <div className="lumina-navbar__mobile-toggle">
          <NavbarMobileSidebarToggle />
        </div>
      )}

      <Link to="/" className="lumina-navbar__brand">
        <span className="lumina-navbar__brand-mark">
          <img alt="Tessera" className="lumina-navbar__brand-icon" src={logoSrc} />
        </span>
        <span className="lumina-navbar__brand-text">
          Tess<span className="lumina-navbar__brand-text-accent">era</span>
        </span>
      </Link>

      <nav className="lumina-navbar__links">
        {leftItems.map((item) => NavbarLink({item, pathname: normalizedPath}))}
      </nav>

      <div className="lumina-navbar__actions">
        {githubItem?.href ? (
          <Link
            href={githubItem.href}
            aria-label={githubItem.label ?? 'GitHub'}
            className="lumina-navbar__icon-button">
            <svg
              className="lumina-navbar__icon"
              viewBox="0 0 24 24"
              aria-hidden="true"
              focusable="false">
              <path
                fill="currentColor"
                d="M12 2C6.477 2 2 6.59 2 12.252c0 4.528 2.865 8.37 6.839 9.724.5.096.683-.222.683-.494 0-.243-.009-.888-.014-1.742-2.782.617-3.369-1.392-3.369-1.392-.455-1.18-1.11-1.495-1.11-1.495-.908-.637.069-.624.069-.624 1.004.072 1.532 1.053 1.532 1.053.893 1.56 2.341 1.11 2.91.849.091-.664.35-1.11.636-1.365-2.22-.26-4.555-1.136-4.555-5.058 0-1.118.389-2.034 1.028-2.751-.103-.262-.446-1.317.098-2.747 0 0 .839-.276 2.75 1.051A9.337 9.337 0 0 1 12 6.79a9.31 9.31 0 0 1 2.503.349c1.911-1.327 2.748-1.051 2.748-1.051.546 1.43.203 2.485.1 2.747.64.717 1.026 1.633 1.026 2.751 0 3.932-2.339 4.796-4.566 5.052.359.317.679.943.679 1.9 0 1.372-.013 2.478-.013 2.814 0 .274.18.595.688.494C19.138 20.619 22 16.777 22 12.252 22 6.59 17.523 2 12 2Z"
              />
            </svg>
          </Link>
        ) : null}
      </div>
    </div>
  );
}
