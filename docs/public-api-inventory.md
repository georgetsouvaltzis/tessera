# TeaSharp Public API Inventory

## Purpose

This document tracks the public API tiers so the pre-release redesign stays deliberate.

## Tier 1: Default Consumer Path

These are the types new applications should discover first.

- `Tea`
- `TeaApp`
- `TeaApplication`
- `TeaApplicationBuilder`
- `TeaRuntimeOptions`
- `TeaEffect`
- `TeaEffects`
- `Message` and the typed message records in `TeaSharp`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `TeaSharp.Controls.Control`

The intended beginner path is:

- build an app by deriving from `TeaApp`
- run it with `Tea.RunAsync(...)` or `TeaApplicationBuilder`
- return `Screen` from `Build(ScreenContext)`
- keep configuration in `TeaRuntimeOptions` and `ScreenOptions`

## Tier 2: Advanced But Supported

These APIs remain public because they still offer real value, but they should not dominate the default path.

- `TeaProgramOptions`
- `IScreen`
- `ProgramOptions`
- `TeaProgram`
- `TeaSharp.Components.Composition.*`
- `TeaSharp.Components.Interaction.*`
- `TeaSharp.Components.Styling.*`
- `IProgramRenderer`
- `ITerminalAdapter`
- `IEventDecoder`
- renderer, terminal, and capability-probing seams
- low-level widget infrastructure such as `TextInputModel`, `ViewportModel`, `ListModel<T>`, and `*KeyMap`

Most of these types are now marked `EditorBrowsable(Advanced)`.

## Tier 3: Candidates For Further Narrowing

These areas still expose more mechanism than the long-term public design should:

- composition types centered on explicit region routing
- low-level widget models leaking through component configuration
- runtime seams that most apps never need
- duplicate terminology between root app types and older core/runtime types

## Current Direction

TeaSharp is shifting from:

- `Tea.CreateProgram(...)`
- `TeaProgramOptions`
- `InteractiveScreenModel`
- `ScreenComposer`
- `InputRouter`

to:

- `Tea.RunAsync(...)`
- `TeaApplicationBuilder`
- `TeaApp`
- `Screen`
- `ScreenContext`
- `TeaRuntimeOptions`

The old stack remains available for now, but it is no longer the recommended starting point.

## Design Constraints

- normal apps should stay in `TeaSharp`
- normal apps should not import `TeaSharp.Core.*`
- normal apps should not manage terminal size manually
- normal apps should not manage input scopes or region routing manually
- custom widgets should remain possible through a small stable contract

## Follow-up Targets

1. keep moving control authoring toward a single obvious configuration style
2. continue pushing screen-scale routing types behind advanced discoverability
3. introduce the next app-facing composition layer without exposing engine vocabulary
4. keep custom widget extensibility stable while internal runtime details continue to shrink
