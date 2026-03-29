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

For a brand-new repository, GitHub Pages may still need one repo setting enabled before the first deploy:

1. Open `Settings -> Pages`
2. Set `Build and deployment -> Source` to `GitHub Actions`

Optional: if you add a repository secret named `PAGES_ENABLEMENT_TOKEN`, the workflow will try to enable Pages automatically. That token must be a PAT or GitHub App token with the required Pages/admin rights; `GITHUB_TOKEN` is not enough for auto-enable.
