# Tessera Landing

This directory contains the public landing page for Tessera.

- `web/` handles the premium marketing homepage and any future product-facing pages.
- `site/` and `docs/` still handle the documentation site via MkDocs.

## Local development

Landing page:

```bash
cd web
npm install
npm run dev
```

Documentation:

```bash
cd site
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements-docs.txt
mkdocs serve -f mkdocs.yml
```

## GitHub Pages build shape

1. Next.js builds to `web/out`
2. MkDocs builds to a temporary directory
3. Docs are copied into `web/out/docs`
4. GitHub Pages deploys `web/out`
