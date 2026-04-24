---
sidebar_label: Architectural Review
---

# Architectural Review

This page explains Tessera as a product architecture, not just a namespace list.

The goal is simple:

- one public app-authoring path
- clear layer ownership
- explicit seams when the app gets dense
- clear emphasis on the primary app-authoring path before advanced runtime seams

## Layer DAG

This DAG is intentionally plain Markdown so it renders everywhere without extra plugins.

```text
App Author Code
    |
    v
TesseraApp
  Initialize()
  Update(Message)
  Build(ScreenContext)
    |
    v
Screen
  +-- Tessera.Layout
  +-- Tessera.Controls
  +-- Tessera.Styles
    |
    v
Scene Compilation + Input Routing
    |
    v
Tessera.Hosting / Runtime Loop
    |
    v
Terminal Adapter + Renderer
    |
    v
Core runtime namespaces (`Tessera.Core`)
```

## Render and message flow

```text
terminal input / timed effect / posted domain message
    -> Message
    -> Update(Message)
    -> state change
    -> Build(ScreenContext)
    -> Screen tree
    -> layout + control measurement
    -> scene compilation
    -> render diff
    -> terminal output
```

That is the core loop. Most of the public docs should stay at that level.

## What each layer is responsible for

| Layer | Owns | Should not own | Primary namespaces |
| --- | --- | --- | --- |
| App layer | state, domain messages, hotkeys, deciding which screen to render | low-level terminal I/O, renderer plumbing | `Tessera`, your app code |
| Screen/composition layer | screen regions, shell structure, layout choices | business state transitions | `Screen`, `Tessera.Layout` |
| Widget layer | interaction surfaces, selection, editing, display widgets | app-wide orchestration, long-lived domain state | `Tessera.Controls` |
| Theme layer | tokens, palettes, control defaults, state overrides | app state and runtime loop policy | `Tessera.Styles` |
| Runtime config layer | frame pacing, pointer activation, theme selection, screen options | screen composition or business logic | `TesseraRuntimeOptions`, `ScreenOptions` |
| Hosting layer | renderer, terminal adapters, event decoding, alternate host seams | beginner onboarding path | `Tessera.Hosting` |
| Core runtime layer | engine-adjacent transport, rendering internals, terminal plumbing | public onboarding examples and normal app code | `Tessera.Core` (in `Tessera` package) |

## Public path versus advanced path

The default public path is:

1. derive from `TesseraApp`
2. use `Update(Message)` for state transitions
3. use `Build(ScreenContext)` for rendering
4. compose with `Screen.Build(...)`
5. use widgets from `Tessera.Controls`
6. configure startup with `TesseraApplication.RunAsync(...)` or `CreateBuilder()`

The advanced path exists when you need:

- custom hosting
- renderer substitution
- terminal capability probing
- lower-level runtime control

That is where `Tessera.Hosting` and advanced `Tessera.Core` namespaces matter.

## Responsibility map by question

When you ask:

- `Where should my domain state live?`
  - in your `TesseraApp`
- `Where should global hotkeys and background refresh live?`
  - in `Update(Message)` and effects
- `Where should shell structure live?`
  - in `Screen.Build(...)` and `Tessera.Layout`
- `Where should interaction details live?`
  - in the widget instance and widget events
- `Where should color, borders, and focus treatment live?`
  - in `TesseraTheme`, theme defaults, and overrides
- `Where should alternate screen and mouse tracking live?`
  - in `ScreenOptions`
- `Where should frame pacing and pointer activation live?`
  - in `TesseraRuntimeOptions`

## Recommended ownership boundaries

Keep these boundaries sharp:

- app code owns domain state
- widgets own local interaction mechanics
- layout objects own spatial composition
- theme objects own visual defaults and override layers
- runtime options own loop/terminal policy
- hosting/core own transport and rendering internals

When those boundaries blur, the docs get hard to teach and the API gets harder to keep deliberate.

## Example-to-layer map

Use the examples as architecture checkpoints:

- `HelloWorld`
  - proves the smallest app/runtime/composition loop
- `CounterForm`
  - proves input widgets, form state, and simple `Update(...)`
- `WorkspaceApp`
  - proves denser composition without changing the app model
- `GitConsole`
  - proves workflow-heavy surfaces on the same public path
- `OpsWatch`
  - proves dashboard density and timed refresh
- `DataWorkbench`
  - proves multi-pane workbench structure, inspectors, and denser record surfaces

## Common architectural mistakes

- introducing advanced runtime seams before the primary app model is clear
- letting widgets become app-state containers
- mixing terminal configuration into screen composition code
- solving dense screens by inventing a second framework story
- treating theme overrides as one-off hardcoded visual hacks

## Review checklist

When you review a feature or doc change, ask:

1. does it stay on the default public path?
2. does it make layer ownership clearer or blurrier?
3. does it belong in app code, widget code, layout code, theme code, runtime config, or hosting/core?
4. would a new user know which layer to touch after reading the docs?

## Related pages

- architecture map for contributors: [architecture-overview](/docs/architecture-overview)
- lifecycle and message flow: [app-model](/docs/app-model)
- composition model: [layout-and-screen-composition](/docs/layout-and-screen-composition)
- runtime knobs: [runtime-and-screen-options](/docs/runtime-and-screen-options)
- widget families: [controls-overview](/docs/controls-overview)
