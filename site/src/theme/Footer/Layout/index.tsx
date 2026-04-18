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
    <footer className="lumina-footer">
      <div className="lumina-footer__inner">
        <div className="lumina-footer__copyright">{copyright}</div>
        <div className="lumina-footer__links">
          {items.map((item) =>
            item.to ? (
              <Link
                key={`${item.label}-${item.to}`}
                className="lumina-footer__link"
                to={item.to}>
                {item.label}
              </Link>
            ) : item.href ? (
              <Link
                key={`${item.label}-${item.href}`}
                className="lumina-footer__link"
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
