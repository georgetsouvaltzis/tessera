# Changelog

All notable changes to TeaSharp should be recorded here.

TeaSharp uses `major.minor.patch` versioning with SemVer prerelease labels during alpha:

- the public alpha line starts at `1.0.0-alpha.1`
- prerelease increments (`1.0.0-alpha.2`) track additional alpha cuts on the same intended stable line
- if the intended stable target changes meaningfully before release, the prerelease line can move accordingly (`1.1.0-alpha.1`)
- `1.0.0` is still the first stability line where stricter compatibility expectations apply

## [Unreleased]

### Added

- root-level [SUPPORT.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/SUPPORT.md) for the public issue and support contract
- `examples/TeaSharp.Examples.slnx` for building the public examples as one separate solution

### Changed

- README and contributor guidance now point to the support policy
- repo verification guidance now includes `dotnet build examples/TeaSharp.Examples.slnx`

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

### Changed

- public docs now teach the starter ladder before the flagship showcases
- `README.md` now acts as a stronger public-facing front page for GitHub visitors
- button visuals now default to flat filled rectangular action surfaces on the public path

### Fixed

- repeated drift between docs and live example paths
- multiple shared button rendering issues around fill, spacing, and starter-example polish

### Notes

- `1.0.0-alpha.1` is the first public alpha baseline, not a stable compatibility promise
- breaking changes can still happen before `1.0.0`, but they should be reflected here
