import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';
import luminaPrismTheme from './src/lib/luminaPrismTheme';

const config: Config = {
  title: 'Tessera',
  tagline: 'Terminal UI for serious .NET apps',
  favicon: 'img/favicon.svg',
  url: 'https://georgetsouvaltzis.github.io',
  baseUrl: '/tessera/',
  organizationName: 'georgetsouvaltzis',
  projectName: 'teasharp',
  trailingSlash: false,
  onBrokenLinks: 'throw',
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },
  markdown: {
    hooks: {
      onBrokenMarkdownLinks: 'throw',
    },
  },
  presets: [
    [
      'classic',
      {
        docs: {
          path: '../docs',
          routeBasePath: 'docs',
          sidebarPath: './sidebars.ts',
          editUrl: 'https://github.com/georgetsouvaltzis/teasharp/edit/master/docs/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],
  themeConfig: {
    navbar: {
      title: 'Tessera',
      logo: {
        alt: 'Tessera logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          to: '/docs/getting-started',
          position: 'left',
          label: 'Docs',
          activeBaseRegex:
            '^/(?:(?:teasharp|tessera)/)?docs(?:$|/(overview|getting-started|install-and-prerequisites|first-app|examples|showcase|app-model|layout-and-screen-composition|runtime-and-screen-options|controls-overview|widgets-inputs-and-forms|widgets-navigation-and-workflow|widgets-data-and-inspection|widgets-dashboards-and-plots|widgets-shells-and-overlays|recipes|recipes-app-shells|recipes-effects-and-refresh|recipes-data-and-workspaces|theme-system|custom-components|troubleshooting|faq|architectural-review|architecture-overview|performance|widget-roadmap|alpha-release-checklist|spec|api-reference|public-api-inventory|public-api-guidelines|terminal-font-capability-matrix))',
        },
        {
          to: '/changelog',
          label: 'Changelog',
          position: 'left',
        },
        {
          to: '/docs/showcase',
          label: 'Examples',
          position: 'left',
        },
        {
          href: 'https://github.com/georgetsouvaltzis/teasharp',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Getting Started',
              to: '/docs/getting-started',
            },
            {
              label: 'API Reference',
              to: '/docs/api-reference',
            },
          ],
        },
        {
          title: 'Examples',
          items: [
            {
              label: 'Starter Examples',
              to: '/docs/examples',
            },
            {
              label: 'Showcase',
              to: '/docs/showcase',
            },
            {
              label: 'GitHub',
              href: 'https://github.com/georgetsouvaltzis/teasharp',
            },
          ],
        },
        {
          title: 'Project',
          items: [
            {
              label: 'Changelog',
              to: '/changelog',
            },
            {
              label: 'Contributing',
              to: '/contributing',
            },
            {
              label: 'Security',
              to: '/security',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Tessera.`,
    },
    colorMode: {
      defaultMode: 'dark',
      disableSwitch: true,
      respectPrefersColorScheme: false,
    },
    prism: {
      theme: luminaPrismTheme,
      darkTheme: luminaPrismTheme,
      additionalLanguages: ['bash', 'csharp'],
    },
    metadata: [
      {
        name: 'keywords',
        content: 'Tessera, terminal UI, .NET, C#, TUI',
      },
    ],
  } satisfies Preset.ThemeConfig,
};

export default config;
