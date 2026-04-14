import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'Tessera',
  tagline: 'Premium terminal UI for .NET',
  favicon: 'img/favicon.svg',
  future: {
    v4: true,
  },
  url: 'https://georgetsouvaltzis.github.io',
  baseUrl: '/tessera/',
  organizationName: 'georgetsouvaltzis',
  projectName: 'tessera',
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
          path: 'docs',
          routeBasePath: 'docs',
          sidebarPath: './sidebars.ts',
          editUrl: 'https://github.com/georgetsouvaltzis/tessera/tree/main/site/',
        },
        blog: {
          path: 'blog',
          routeBasePath: 'updates',
          showReadingTime: true,
          onUntruncatedBlogPosts: 'ignore',
          blogTitle: 'Tessera Updates',
          blogDescription: 'Release notes and documentation updates for Tessera.',
          editUrl: 'https://github.com/georgetsouvaltzis/tessera/tree/main/site/',
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],
  themeConfig: {
    image: 'img/social-card.svg',
    navbar: {
      title: 'Tessera',
      logo: {
        alt: 'Tessera logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          to: '/docs/getting-started',
          label: 'Docs',
          position: 'left',
          activeBaseRegex: '^/docs',
        },
        {
          to: '/docs/controls',
          label: 'Controls',
          position: 'left',
        },
        {
          to: '/docs/examples',
          label: 'Examples',
          position: 'left',
        },
        {
          to: '/showcase',
          label: 'Showcase',
          position: 'left',
        },
        {
          to: '/updates',
          label: 'Updates',
          position: 'left',
        },
        {
          href: 'https://github.com/georgetsouvaltzis/tessera',
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
              label: 'Styling',
              to: '/docs/styling/themes-and-tokens',
            },
            {
              label: 'Controls',
              to: '/docs/controls',
            },
          ],
        },
        {
          title: 'Examples',
          items: [
            {
              label: 'Examples Overview',
              to: '/docs/examples',
            },
            {
              label: 'Showcase',
              to: '/showcase',
            },
            {
              label: 'Updates',
              to: '/updates',
            },
          ],
        },
        {
          title: 'Project',
          items: [
            {
              label: 'Repository',
              href: 'https://github.com/georgetsouvaltzis/tessera',
            },
            {
              label: 'Issues',
              href: 'https://github.com/georgetsouvaltzis/tessera/issues',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Tessera.`,
    },
    prism: {
      theme: {
        plain: {
          color: '#f4f7ff',
          backgroundColor: '#0d1324',
        },
        styles: [
          {
            types: ['comment'],
            style: { color: '#a8b4d6', fontStyle: 'italic' },
          },
          {
            types: ['keyword', 'operator'],
            style: { color: '#ff4fd8' },
          },
          {
            types: ['string'],
            style: { color: '#4fffe0' },
          },
          {
            types: ['class-name', 'function'],
            style: { color: '#59b7ff' },
          },
          {
            types: ['number'],
            style: { color: '#ffc857' },
          },
        ],
      },
      darkTheme: {
        plain: {
          color: '#f4f7ff',
          backgroundColor: '#0d1324',
        },
        styles: [
          {
            types: ['comment'],
            style: { color: '#a8b4d6', fontStyle: 'italic' },
          },
          {
            types: ['keyword', 'operator'],
            style: { color: '#ff4fd8' },
          },
          {
            types: ['string'],
            style: { color: '#4fffe0' },
          },
          {
            types: ['class-name', 'function'],
            style: { color: '#59b7ff' },
          },
          {
            types: ['number'],
            style: { color: '#ffc857' },
          },
        ],
      },
      additionalLanguages: ['bash', 'csharp'],
    },
    colorMode: {
      defaultMode: 'dark',
      disableSwitch: false,
      respectPrefersColorScheme: true,
    },
    metadata: [
      {
        name: 'keywords',
        content: 'Tessera, .NET terminal UI, C#, TUI, console UI',
      },
    ],
  } satisfies Preset.ThemeConfig,
};

export default config;
