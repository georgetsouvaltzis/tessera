import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';
import { themes as prismThemes } from 'prism-react-renderer';

const config: Config = {
  title: 'Tessera',
  tagline: 'Terminal UI for .NET',
  favicon: 'img/favicon.svg',
  url: 'https://georgetsouvaltzis.github.io',
  baseUrl: '/teasharp/',
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
      onBrokenMarkdownLinks: 'warn',
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
          to: '/docs',
          position: 'left',
          label: 'Docs',
          activeBaseRegex:
            '^/(?:teasharp/)?docs(?:$|/(overview|getting-started|examples|showcase|theme-system|custom-components|architecture-overview|performance|widget-roadmap|alpha-release-checklist|spec))',
        },
        {
          to: '/docs/api-reference',
          label: 'API',
          position: 'left',
          activeBaseRegex:
            '^/(?:teasharp/)?docs/(api-reference|public-api-guidelines|public-api-inventory|terminal-font-capability-matrix)(?:$|/)',
        },
        {
          to: '/docs/showcase',
          label: 'Showcase',
          position: 'left',
        },
        {
          to: '/changelog',
          label: 'Changelog',
          position: 'right',
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
              label: 'Docs Home',
              to: '/docs',
            },
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
      respectPrefersColorScheme: true,
    },
    prism: {
      theme: prismThemes.oneLight,
      darkTheme: prismThemes.oneDark,
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
