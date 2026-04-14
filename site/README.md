# Tessera Docs Site

This directory contains the MkDocs configuration for the documentation site only.
The marketing landing page lives in `web/`.

The actual content source is the repository root:

- `README.md`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `SUPPORT.md`
- `CODE_OF_CONDUCT.md`
- `SECURITY.md`
- `docs/`

## Local docs development

```bash
cd site
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements-docs.txt
mkdocs serve -f mkdocs.yml
```

## Verification

```bash
cd site
source .venv/bin/activate
export TESSERA_SITE_DIR="${TMPDIR:-/tmp}/tessera-site-build"
mkdocs build -f mkdocs.yml
```

Build output goes to `TESSERA_SITE_DIR`.
If you do not set it, the default build output is `/tmp/tessera-site-build`.
