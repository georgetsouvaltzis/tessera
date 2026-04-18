import React, {type ReactNode} from 'react';
import Link from '@docusaurus/Link';
import useBaseUrl from '@docusaurus/useBaseUrl';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import {useThemeConfig} from '@docusaurus/theme-common';
import {splitNavbarItems, useNavbarMobileSidebar} from '@docusaurus/theme-common/internal';
import {useLocation} from '@docusaurus/router';
import NavbarMobileSidebarToggle from '@theme/Navbar/MobileSidebar/Toggle';
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

  const className = `rounded-md px-3 py-1.5 text-sm transition-colors ${
    isActive(item, pathname)
      ? 'text-[var(--tessera-text)]'
      : 'text-[var(--tessera-text-muted)] hover:bg-[rgba(255,127,197,0.08)] hover:text-[var(--tessera-text)]'
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
  const logoSrc = useBaseUrl('/img/logo.svg');
  const githubItem = rightItems.find((item) => item.href);
  const normalizedPath = pathname.startsWith(siteConfig.baseUrl)
    ? pathname.slice(siteConfig.baseUrl.length - 1)
    : pathname;

  return (
    <div className="mx-auto flex h-14 max-w-7xl items-center gap-4 px-4 sm:px-6 lg:px-8">
      {!mobileSidebar.disabled && (
        <div className="md:hidden">
          <NavbarMobileSidebarToggle />
        </div>
      )}

      <Link to="/" className="flex items-center gap-2 font-semibold tracking-tight">
        <span className="relative flex h-7 w-7 items-center justify-center overflow-hidden rounded-md bg-[var(--tessera-gradient-primary)] shadow-[0_0_18px_rgba(255,127,197,0.26)]">
          <img alt="Tessera" className="h-5 w-5" src={logoSrc} />
        </span>
        <span className="text-[var(--tessera-text)]">
          Tess<span className="text-[var(--ifm-color-primary)] [text-shadow:0_0_20px_rgba(255,127,197,0.52)]">era</span>
        </span>
      </Link>

      <nav className="hidden items-center gap-1 md:flex">
        {leftItems.map((item) => NavbarLink({item, pathname: normalizedPath}))}
      </nav>

      <div className="ml-auto flex items-center gap-2">
        <button
          type="button"
          className="group hidden h-9 w-64 items-center gap-2 rounded-lg border border-[var(--tessera-border)] bg-[rgba(18,21,28,0.76)] px-3 text-sm text-[var(--tessera-text-soft)] transition-colors hover:border-[var(--tessera-border-strong)] hover:text-[var(--tessera-text)] sm:flex">
          <Search className="h-4 w-4" />
          <span className="flex-1 text-left">Search docs...</span>
          <kbd className="rounded border border-[var(--tessera-border)] bg-[rgba(255,255,255,0.04)] px-1.5 py-0.5 font-mono text-[10px] text-[var(--tessera-text-soft)]">
            ⌘K
          </kbd>
        </button>
        {githubItem?.href ? (
          <Link
            href={githubItem.href}
            aria-label={githubItem.label ?? 'GitHub'}
            className="flex h-9 w-9 items-center justify-center rounded-lg border border-[var(--tessera-border)] bg-[rgba(18,21,28,0.76)] text-[var(--tessera-text-soft)] transition-colors hover:border-[var(--tessera-border-strong)] hover:text-[var(--tessera-text)]">
            <GitBranch className="h-4 w-4" />
          </Link>
        ) : null}
      </div>
    </div>
  );
}
