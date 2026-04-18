import React from 'react';
import clsx from 'clsx';
import {useDocsSidebar} from '@docusaurus/plugin-content-docs/client';

export default function DocRootLayoutMain({
  hiddenSidebarContainer,
  children,
}: {
  children: React.ReactNode;
  hiddenSidebarContainer: boolean;
}): React.JSX.Element {
  const sidebar = useDocsSidebar();

  return (
    <main
      className={clsx(
        'lumina-docs-main',
        (hiddenSidebarContainer || !sidebar) && 'lumina-docs-main--enhanced',
      )}>
      <div
        className={clsx(
          'lumina-docs-main__inner',
          hiddenSidebarContainer && 'lumina-docs-main__inner--enhanced',
        )}>
        {children}
      </div>
    </main>
  );
}
