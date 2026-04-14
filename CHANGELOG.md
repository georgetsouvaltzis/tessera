# Changelog

All notable changes to Tessera should be recorded here.

Tessera uses `major.minor.patch` versioning with SemVer prerelease labels during alpha:

- the public alpha line starts at `1.0.0-alpha.1`
- until Tessera is publicly live, keep all notable changes under the active public alpha heading instead of using a separate `Unreleased` bucket
- prerelease increments (`1.0.0-alpha.2`) track additional alpha cuts on the same intended stable line
- if the intended stable target changes meaningfully before release, the prerelease line can move accordingly (`1.1.0-alpha.1`)
- `1.0.0` is still the first stability line where stricter compatibility expectations apply

## [1.0.0-alpha.1] - 2026-04-07

### Added

- canonical onboarding example ladder:
  - `examples/HelloWorld`
  - `examples/CounterForm`
  - `examples/WorkspaceApp`
- public boundary coverage for onboarding examples and docs
- root-level contributor docs:
  - `CONTRIBUTING.md`
  - `CODE_OF_CONDUCT.md`
  - `SECURITY.md`
- root-level [SUPPORT.md](SUPPORT.md) for the public issue and support contract
- `examples/Tessera.Examples.slnx` for building the public examples as one separate solution
- GitHub community intake files:
  - `.github/ISSUE_TEMPLATE/bug_report.yml`
  - `.github/ISSUE_TEMPLATE/feature_request.yml`
  - `.github/pull_request_template.md`
- tracked dashboard navigation regression helper in `tests/Tessera.Tests/PublicApiDashboard`

### Changed

- project and solution branding now use `Tessera` across source, tests, examples, docs, and solution entrypoints
- public docs now teach the starter ladder before the flagship showcases
- `README.md` now acts as a stronger public-facing front page for GitHub visitors
- the docs site now uses MkDocs Material rooted on the live repo docs instead of a separate Docusaurus content tree
- button visuals now default to flat filled rectangular action surfaces on the public path
- README and contributor guidance now point to the support policy
- repo verification guidance now includes `dotnet build examples/Tessera.Examples.slnx`
- public docs and release artifacts now use repo-relative paths instead of local machine filesystem links
- public docs and release entrypoints now use simpler names without `v1` suffixes
- shell-wrapper verification scripts were removed in favor of direct `dotnet` commands
- obsolete example projects were removed from `examples/` to keep the public lineup aligned with the live solutions and docs

### Fixed

- repeated drift between docs and live example paths
- multiple shared button rendering issues around fill, spacing, and starter-example polish
- clean-clone risk from an untracked test helper required by the dashboard regression suite

### Notes

- `1.0.0-alpha.1` is the first public alpha baseline, not a stable compatibility promise
- breaking changes can still happen before `1.0.0`, but they should be reflected here
