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
      ],
    },
    {
      type: 'category',
      label: 'Core Concepts',
      items: [
        {
          type: 'doc',
          id: 'controls-overview',
          label: 'Components',
        },
        {
          type: 'doc',
          id: 'app-model',
          label: 'State & effects',
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
          label: 'CLI reference',
        },
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      items: [
        {
          type: 'doc',
          id: 'custom-components',
          label: 'Custom renderers',
        },
      ],
    },
    {
      type: 'category',
      label: 'Help',
      items: ['troubleshooting'],
    },
    {
      type: 'category',
      label: 'Maintainers',
      collapsed: false,
      items: [
        {
          type: 'doc',
          id: 'alpha-release-checklist',
          label: 'Release process',
        },
      ],
    },
  ],
};

export default sidebars;
