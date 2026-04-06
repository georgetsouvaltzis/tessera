# Changelog

All notable changes to TeaSharp should be recorded here.

TeaSharp uses `major.minor.patch` versioning:

- `0.y.z` while the project is still in alpha
- patch releases (`0.1.1`) for fixes, docs corrections, and non-breaking polish
- minor releases (`0.2.0`) for meaningful feature additions or intentional public-path reshaping during alpha
- `1.0.0` is the first stability line where stricter compatibility expectations apply

## [Unreleased]

### Planned

- next public-alpha candidate tag and final release SHA
- perf-gate evidence tied to the chosen release candidate

## [0.1.0] - 2026-04-07

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

- `0.1.0` is the intended public alpha baseline, not a stable compatibility promise
- breaking changes can still happen before `1.0.0`, but they should be reflected here
