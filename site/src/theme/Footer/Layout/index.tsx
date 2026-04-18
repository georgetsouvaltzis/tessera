import React from 'react';
import Link from '@docusaurus/Link';
import {useThemeConfig} from '@docusaurus/theme-common';

type FooterItem = {
  href?: string;
  label?: string;
  to?: string;
};

type FooterLinkGroup = {
  items?: FooterItem[];
};

type FooterConfig = {
  links?: FooterLinkGroup[];
};

const preferredLabels = ['GitHub', 'Changelog', 'Security'];

function pickFooterItems(config: FooterConfig): FooterItem[] {
  const flatItems = (config.links ?? []).flatMap((group) => group.items ?? []);
  const picked = preferredLabels
    .map((label) => flatItems.find((item) => item.label === label))
    .filter((item): item is FooterItem => Boolean(item));

  return picked.length > 0 ? picked : flatItems.slice(0, 3);
}

export default function FooterLayout({
  copyright,
}: {
  copyright?: React.ReactNode;
}): React.JSX.Element {
  const footer = useThemeConfig().footer as FooterConfig;
  const items = pickFooterItems(footer);

  return (
    <footer className="border-t border-[rgba(255,255,255,0.08)] py-8">
      <div className="mx-auto flex max-w-7xl flex-col items-center justify-between gap-3 px-4 text-sm text-[var(--tessera-text-soft)] sm:flex-row sm:px-6 lg:px-8">
        <div>{copyright}</div>
        <div className="flex flex-wrap items-center gap-5">
          {items.map((item) =>
            item.to ? (
              <Link
                key={`${item.label}-${item.to}`}
                className="transition-colors hover:text-[var(--tessera-text)]"
                to={item.to}>
                {item.label}
              </Link>
            ) : item.href ? (
              <Link
                key={`${item.label}-${item.href}`}
                className="transition-colors hover:text-[var(--tessera-text)]"
                href={item.href}>
                {item.label}
              </Link>
            ) : null,
          )}
        </div>
      </div>
    </footer>
  );
}
