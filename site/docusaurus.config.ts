import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'TeaSharp',
  tagline: 'Premium terminal UI for .NET',
  favicon: 'img/favicon.svg',
  future: {
    v4: true,
  },
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
          path: 'docs',
          routeBasePath: 'docs',
          sidebarPath: './sidebars.ts',
          editUrl: 'https://github.com/georgetsouvaltzis/teasharp/tree/main/site/',
        },
        blog: {
          path: 'blog',
          routeBasePath: 'updates',
          showReadingTime: true,
          onUntruncatedBlogPosts: 'ignore',
          blogTitle: 'TeaSharp Updates',
          blogDescription: 'Release notes and documentation updates for TeaSharp.',
          editUrl: 'https://github.com/georgetsouvaltzis/teasharp/tree/main/site/',
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
      title: 'TeaSharp',
      logo: {
        alt: 'TeaSharp logo',
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
              href: 'https://github.com/georgetsouvaltzis/teasharp',
            },
            {
              label: 'Issues',
              href: 'https://github.com/georgetsouvaltzis/teasharp/issues',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} TeaSharp.`,
    },
    prism: {
      theme: {
        plain: {
          color: '#f3f4f6',
          backgroundColor: '#101725',
        },
        styles: [
          {
            types: ['comment'],
            style: { color: '#8b94a7', fontStyle: 'italic' },
          },
          {
            types: ['keyword', 'operator'],
            style: { color: '#f38ba8' },
          },
          {
            types: ['string'],
            style: { color: '#a6e3a1' },
          },
          {
            types: ['class-name', 'function'],
            style: { color: '#89b4fa' },
          },
          {
            types: ['number'],
            style: { color: '#f9e2af' },
          },
        ],
      },
      darkTheme: {
        plain: {
          color: '#f3f4f6',
          backgroundColor: '#101725',
        },
        styles: [
          {
            types: ['comment'],
            style: { color: '#8b94a7', fontStyle: 'italic' },
          },
          {
            types: ['keyword', 'operator'],
            style: { color: '#f38ba8' },
          },
          {
            types: ['string'],
            style: { color: '#a6e3a1' },
          },
          {
            types: ['class-name', 'function'],
            style: { color: '#89b4fa' },
          },
          {
            types: ['number'],
            style: { color: '#f9e2af' },
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
        content: 'TeaSharp, .NET terminal UI, C#, TUI, console UI',
      },
    ],
  } satisfies Preset.ThemeConfig,
};

export default config;
