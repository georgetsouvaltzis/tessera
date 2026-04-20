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
        {
          type: 'doc',
          id: 'overview',
          label: 'Introduction',
        },
        {
          type: 'doc',
          id: 'install-and-prerequisites',
          label: 'Installation',
        },
        {
          type: 'doc',
          id: 'quickstart-new-app',
          label: 'Quickstart (New App)',
        },
        {
          type: 'doc',
          id: 'quickstart-existing-app',
          label: 'Quickstart (Existing App)',
        },
        {
          type: 'doc',
          id: 'first-app',
          label: 'Your First App',
        },
        {
          type: 'doc',
          id: 'beginner-track',
          label: 'Beginner Track',
        },
        {
          type: 'doc',
          id: 'examples',
          label: 'Starter Examples',
        },
        {
          type: 'doc',
          id: 'showcase',
          label: 'Flagship Evaluation',
        },
      ],
    },
    {
      type: 'category',
      label: 'Core Concepts',
      items: [
        {
          type: 'doc',
          id: 'app-model',
          label: 'App Model',
        },
        {
          type: 'doc',
          id: 'layout-and-screen-composition',
          label: 'Screen & Layout',
        },
        {
          type: 'doc',
          id: 'runtime-and-screen-options',
          label: 'Runtime & Screen Options',
        },
        {
          type: 'doc',
          id: 'theme-system',
          label: 'Theme System',
        },
      ],
    },
    {
      type: 'category',
      label: 'Widgets',
      items: [
        {
          type: 'doc',
          id: 'widget-reference',
          label: 'Widget Reference',
        },
        {
          type: 'doc',
          id: 'widgets/index',
          label: 'Widget Pages',
        },
        {
          type: 'doc',
          id: 'controls-overview',
          label: 'Widgets Overview',
        },
        {
          type: 'doc',
          id: 'widgets-inputs-and-forms',
          label: 'Inputs & Forms',
        },
        {
          type: 'doc',
          id: 'widgets-navigation-and-workflow',
          label: 'Navigation & Workflow',
        },
        {
          type: 'doc',
          id: 'widgets-data-and-inspection',
          label: 'Data & Inspection',
        },
        {
          type: 'doc',
          id: 'widgets-dashboards-and-plots',
          label: 'Dashboards & Plots',
        },
        {
          type: 'doc',
          id: 'widgets-shells-and-overlays',
          label: 'Shells & Overlays',
        },
      ],
    },
    {
      type: 'category',
      label: 'Recipes',
      items: [
        {
          type: 'doc',
          id: 'recipes',
          label: 'Recipes Overview',
        },
        {
          type: 'doc',
          id: 'recipes-app-shells',
          label: 'App Shell Recipes',
        },
        {
          type: 'doc',
          id: 'recipes-effects-and-refresh',
          label: 'Effects & Refresh',
        },
        {
          type: 'doc',
          id: 'recipes-data-and-workspaces',
          label: 'Data & Workspaces',
        },
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      items: [
        {
          type: 'doc',
          id: 'advanced-track',
          label: 'Advanced Track',
        },
        {
          type: 'doc',
          id: 'custom-components',
          label: 'Custom Components',
        },
        {
          type: 'doc',
          id: 'architectural-review',
          label: 'Architectural Review',
        },
        {
          type: 'doc',
          id: 'architecture-overview',
          label: 'Architecture Overview',
        },
        {
          type: 'doc',
          id: 'performance',
          label: 'Performance',
        },
      ],
    },
    {
      type: 'category',
      label: 'Reference',
      items: [
        {
          type: 'doc',
          id: 'api-reference',
          label: 'API Reference',
        },
        {
          type: 'doc',
          id: 'public-api-inventory',
          label: 'Public API Inventory',
        },
        {
          type: 'doc',
          id: 'public-api-guidelines',
          label: 'API Guidelines',
        },
        {
          type: 'doc',
          id: 'terminal-font-capability-matrix',
          label: 'Terminal Capability Matrix',
        },
      ],
    },
    {
      type: 'category',
      label: 'Help',
      items: [
        {
          type: 'doc',
          id: 'troubleshooting',
          label: 'Troubleshooting',
        },
        {
          type: 'doc',
          id: 'faq',
          label: 'FAQ',
        },
      ],
    },
    {
      type: 'category',
      label: 'Maintainers',
      collapsed: true,
      items: [
        {
          type: 'doc',
          id: 'alpha-release-checklist',
          label: 'Release process',
        },
        {
          type: 'doc',
          id: 'widget-roadmap',
          label: 'Widget roadmap',
        },
      ],
    },
  ],
};

export default sidebars;
