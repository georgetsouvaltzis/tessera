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
          type: 'docSidebar',
          sidebarId: 'docsSidebar',
          position: 'left',
          label: 'Docs',
        },
        {
          to: '/docs/showcase',
          label: 'Showcase',
          position: 'left',
        },
        {
          to: '/changelog',
          label: 'Changelog',
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
              label: 'Overview',
              to: '/docs/overview',
            },
            {
              label: 'Getting Started',
              to: '/docs/getting-started',
            },
            {
              label: 'Showcase',
              to: '/docs/showcase',
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
        {
          title: 'More',
          items: [
            {
              label: 'Support',
              to: '/support',
            },
            {
              label: 'Code of Conduct',
              to: '/code-of-conduct',
            },
            {
              label: 'GitHub',
              href: 'https://github.com/georgetsouvaltzis/teasharp',
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
      theme: prismThemes.github,
      darkTheme: prismThemes.nightOwl,
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
