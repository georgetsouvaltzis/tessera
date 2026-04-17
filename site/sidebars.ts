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
        'install-and-prerequisites',
        'first-app',
        'examples',
        'showcase',
      ],
    },
    {
      type: 'category',
      label: 'Core Concepts',
      items: [
        'app-model',
        'layout-and-screen-composition',
        'runtime-and-screen-options',
        'controls-overview',
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
      label: 'Help',
      items: [
        'troubleshooting',
        'faq',
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      items: [
        'spec',
        'architecture-overview',
      ],
    },
    {
      type: 'category',
      label: 'Maintainers',
      collapsed: true,
      items: [
        'performance',
        'widget-roadmap',
        'alpha-release-checklist',
      ],
    },
  ],
};

export default sidebars;
