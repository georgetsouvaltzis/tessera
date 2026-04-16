import type { SidebarsConfig } from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  docsSidebar: [
    'overview',
    {
      type: 'category',
      label: 'Start Here',
      items: [
        'getting-started',
        'examples',
        'showcase',
      ],
    },
    {
      type: 'category',
      label: 'Guides',
      items: [
        'theme-system',
        'custom-components',
        'architecture-overview',
      ],
    },
    {
      type: 'category',
      label: 'Reference',
      items: [
        'public-api-guidelines',
        'public-api-inventory',
        'spec',
        'terminal-font-capability-matrix',
      ],
    },
    {
      type: 'category',
      label: 'Project',
      items: [
        'performance',
        'widget-roadmap',
        'alpha-release-checklist',
      ],
    },
  ],
};

export default sidebars;
