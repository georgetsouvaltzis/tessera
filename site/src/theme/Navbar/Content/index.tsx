import React, {type ReactNode, useCallback, useEffect, useState} from 'react';
import Link from '@docusaurus/Link';
import useBaseUrl from '@docusaurus/useBaseUrl';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import {useThemeConfig} from '@docusaurus/theme-common';
import {splitNavbarItems, useNavbarMobileSidebar} from '@docusaurus/theme-common/internal';
import {useLocation} from '@docusaurus/router';
import NavbarMobileSidebarToggle from '@theme/Navbar/MobileSidebar/Toggle';
import SearchBar from '@theme/SearchBar';
import {GitBranch, Search} from 'lucide-react';

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
  const [searchOpen, setSearchOpen] = useState(false);
  const {siteConfig} = useDocusaurusContext();
  const logoSrc = useBaseUrl('/img/logo.svg');
  const githubItem = rightItems.find((item) => item.href);
  const normalizedPath = pathname.startsWith(siteConfig.baseUrl)
    ? pathname.slice(siteConfig.baseUrl.length - 1)
    : pathname;
  const openSearch = useCallback(() => {
    setSearchOpen(true);
  }, []);
  const closeSearch = useCallback(() => {
    setSearchOpen(false);
  }, []);

  useEffect(() => {
    const onKeyDown = (event: KeyboardEvent): void => {
      if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
        event.preventDefault();
        setSearchOpen(true);
      } else if (event.key === 'Escape') {
        setSearchOpen(false);
      }
    };

    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
  }, []);

  useEffect(() => {
    if (!searchOpen) {
      return;
    }

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    const handle = window.requestAnimationFrame(() => {
      document.querySelector<HTMLInputElement>('.lumina-search-modal .navbar__search-input')?.focus();
    });

    return () => {
      window.cancelAnimationFrame(handle);
      document.body.style.overflow = previousOverflow;
    };
  }, [searchOpen]);

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
        <button
          type="button"
          className="lumina-navbar__search"
          onClick={openSearch}
          aria-label="Open docs search">
          <Search className="lumina-navbar__search-icon" />
          <span className="lumina-navbar__search-label">Search docs...</span>
          <kbd className="lumina-navbar__search-key">
            ⌘K
          </kbd>
        </button>
        {githubItem?.href ? (
          <Link
            href={githubItem.href}
            aria-label={githubItem.label ?? 'GitHub'}
            className="lumina-navbar__icon-button">
            <GitBranch className="lumina-navbar__icon" />
          </Link>
        ) : null}
      </div>

      {searchOpen ? (
        <div
          className="lumina-search-modal"
          role="dialog"
          aria-modal="true"
          aria-label="Search docs"
          onClick={(event) => {
            if (event.target === event.currentTarget) {
              closeSearch();
            }
          }}>
          <div className="lumina-search-modal__panel">
            <button
              type="button"
              className="lumina-search-modal__close"
              aria-label="Close search"
              onClick={closeSearch}>
              ×
            </button>
            <SearchBar />
          </div>
        </div>
      ) : null}
    </div>
  );
}
