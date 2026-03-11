# TeaSharp Public API Inventory

## Purpose

This document defines the current public API tiers in TeaSharp so refactors can reduce surface area deliberately instead of by accident.

## Current Public Surface Tiers

### Tier 1: Consumer-facing APIs worth keeping stable

- `TeaSharp.Tea`
- `TeaSharp.TeaProgramOptions`
- `TeaSharp.Components.*` component types intended for application authors
- `TeaSharp.Styles.*`
- high-level model/message contracts required to build applications:
  - `IModel`
  - `IMessage`
  - `View`
  - `ViewFrame`
  - `ViewInput`
  - `ViewTerminal`
  - `Command`

### Tier 2: Advanced seams that should remain available, but not dominate the default path

- `ProgramOptions`
- `TeaProgram`
- `ComponentComposer`
- `IProgramRenderer`
- `ITerminalAdapter`
- `IEventDecoder`
- rendering/input host seams (`AnsiDiffRenderer`, `AnsiRendererOptions`, `NullRenderer`, `TerminalReader`)
- lower-level widget infrastructure (`TextInputModel`, `ViewportModel`, `ListModel<T>`, `IWidgetKeyMap`, `*KeyMap`)
- terminal capability detection types
- specialized rendering/input infrastructure for advanced host customization

### Tier 3: Engine details currently public, but candidates for narrowing or internalization

- `TerminalReader`
- `IEventDecoder` and decoding primitives
- low-level widget models and keymap types that leak through higher-level component APIs
- mutable styling/interactivity collaborators exposed directly on components
- duplicate composition infrastructure where one model can become the recommended path

## Current Pressure Points

- `TeaSharp` is a thin facade, while much of the real app contract still lives under `TeaSharp.Core`.
- `ProgramOptions` exposes too many runtime wiring seams for the default consumer experience.
- components do not follow one consistent constructor/options pattern.
- low-level widget types are visible in places where consumer-facing components should be enough.
- composition is split between `ComponentComposer` and `ScreenComposer`.
- `ScreenComposer` + `InputRouter` + `InteractiveScreenModel` is now the documented default path; `ComponentComposer` is being pushed toward lower-level subtree use.
- runtime plumbing seams (`IProgramRenderer`, `ITerminalAdapter`, `EventDecoder`, `TerminalReader`, capability detectors/profiles) are now explicitly marked `EditorBrowsable(Advanced)` so the stable host path stays centered on `Tea.NewProgram(model)` / `Tea.NewProgram(model, TeaProgramOptions)`.

## Target Public Surface

### Stable default path

- application authors should be able to stay mostly in:
  - `TeaSharp`
  - `TeaSharp.Components`
  - `TeaSharp.Styles`
- app hosting should prefer `Tea.NewProgram(model)` for defaults or `TeaProgramOptions` for stable customization, with `ProgramOptions` reserved for advanced/runtime customization.
- common component setup should flow through `*Options` records and small constructor overloads.
- examples and docs should demonstrate the stable path first.

### Advanced path

- rendering, terminal, capability, and decoding seams can stay public where they offer real extension value.
- advanced seams should move behind clearer documentation and naming so they do not look like the primary path for all consumers.
- `KeyBinding` remains discoverable for now because higher-level component customization still depends on it directly.

### Internalization targets

- internal engine helpers that are public only for historical/test reasons should be reduced over time.
- low-level types should stop leaking through higher-level component options unless that coupling is intentional.

## Immediate Follow-up Targets

1. continue adding `*Options` records to high-churn components.
2. standardize component constructor patterns and clone mutable option collaborators on ingress where appropriate.
3. decide which composition model is primary and document the non-primary path as lower-level or transitional.
4. narrow `ProgramOptions` into default consumer options vs advanced host customization seams.
