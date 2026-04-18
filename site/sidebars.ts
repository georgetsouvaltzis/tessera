import type { SidebarsConfig } from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  docsSidebar: [
    {
      type: 'category',
      label: 'Getting Started',
      link: {
        type: 'doc',
        id: 'getting-started',
      },
      collapsed: false,
      items: [
        'overview',
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
        'theme-system',
        'custom-components',
      ],
    },
    {
      type: 'category',
      label: 'Architecture',
      items: [
        'architectural-review',
        'architecture-overview',
      ],
    },
    {
      type: 'category',
      label: 'Widgets',
      link: {
        type: 'doc',
        id: 'controls-overview',
      },
      items: [
        'widgets-inputs-and-forms',
        'widgets-navigation-and-workflow',
        'widgets-data-and-inspection',
        'widgets-dashboards-and-plots',
        'widgets-shells-and-overlays',
      ],
    },
    {
      type: 'category',
      label: 'Recipes',
      link: {
        type: 'doc',
        id: 'recipes',
      },
      items: [
        'recipes-app-shells',
        'recipes-effects-and-refresh',
        'recipes-data-and-workspaces',
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
