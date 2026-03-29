import type { SidebarsConfig } from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  docs: [
    'getting-started/getting-started',
    {
      type: 'category',
      label: 'Getting Started',
      items: [
        'getting-started/installation',
        'getting-started/quick-start',
      ],
    },
    {
      type: 'category',
      label: 'Core Concepts',
      items: [
        'core-concepts/app-model',
        'core-concepts/screens-layout',
        'core-concepts/messages-effects',
      ],
    },
    {
      type: 'category',
      label: 'Styling',
      items: [
        'styling/themes-and-tokens',
        'styling/control-overrides',
      ],
    },
    {
      type: 'category',
      label: 'Controls',
      items: [
        'controls/controls',
        'controls/inputs',
        'controls/forms',
        'controls/navigation',
        'controls/data-and-tables',
        'controls/overlays',
        'controls/charts',
        'controls/workspace-and-ops',
      ],
    },
    {
      type: 'category',
      label: 'Examples',
      items: [
        'examples/examples',
        'examples/choice',
        'examples/data-form',
        'examples/deploy-console',
        'examples/token-editor',
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      items: [
        'advanced/custom-controls',
        'advanced/hosting-and-runtime',
        'advanced/terminal-capabilities',
      ],
    },
    {
      type: 'category',
      label: 'Reference',
      items: [
        'reference/runtime-options',
        'reference/screen-options',
        'reference/control-index',
        'reference/migration-notes',
      ],
    },
  ],
};

export default sidebars;
