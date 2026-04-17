import type { SidebarsConfig } from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  docsSidebar: [
    {
      type: 'category',
      label: 'Introduction',
      link: {
        type: 'doc',
        id: 'index',
      },
      collapsed: false,
      items: [
        'overview',
        'spec',
      ],
    },
    {
      type: 'category',
      label: 'Getting Started',
      link: {
        type: 'doc',
        id: 'getting-started',
      },
      collapsed: false,
      items: [
        'examples',
        'showcase',
      ],
    },
    {
      type: 'category',
      label: 'Core Concepts',
      items: [
        'theme-system',
        'custom-components',
      ],
    },
    {
      type: 'category',
      label: 'Reference',
      link: {
        type: 'doc',
        id: 'api-reference',
      },
      items: [
        'public-api-guidelines',
        'public-api-inventory',
        'terminal-font-capability-matrix',
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      items: [
        'architecture-overview',
      ],
    },
    {
      type: 'category',
      label: 'Maintainers',
      items: [
        'performance',
        'widget-roadmap',
        'alpha-release-checklist',
      ],
    },
  ],
};

export default sidebars;
