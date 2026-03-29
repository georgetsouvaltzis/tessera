# TeaSharp Site

This directory contains the Docusaurus-based public docs and marketing site for TeaSharp.

## Local development

```bash
cd site
npm install
npm run start
```

## Verification

```bash
cd site
npm run typecheck
npm run build
```

## Deployment

The repository is configured for GitHub Pages via GitHub Actions.

- source: `main`
- site root: `site/`
- output: `site/build/`
- Pages source: `GitHub Actions`

The current config assumes project-pages hosting at:

- `https://georgetsouvaltzis.github.io/teasharp/`
