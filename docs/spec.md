# Tessera Design Contract

Tessera is a `.NET 10` terminal UI framework for state-driven applications.

This document is the live product contract.
It describes what Tessera is, how the public API should feel, and what still belongs outside V1.
It should not act as a historical log or implementation diary.

## Product Position

Tessera is:

- a .NET-native framework for building state-driven terminal applications
- small on the public path
- explicit and strongly typed
- extensible without leaking internal engine details

Tessera is not:

- a Generic-Host-first framework
- a copycat framework shaped around another terminal UI library
- a port of another ecosystem's framework contract
- a nested layout DSL disguised as C#

Tessera is in public alpha.
Breaking changes are still acceptable when they simplify the long-term API and improve the default authoring path.

## V1 Design Center

V1 centers on:

- small root API
- explicit C# object model
- Tessera-owned startup
- screen/layout/control composition
- stable custom control contract
- semantic theming and override layers
- advanced runtime seams kept separate from the default path

## Non-Negotiable Rules

### Default Surface

Normal apps should primarily live in:

- `Tessera`
- `Tessera.Controls`
- `Tessera.Layout`
- `Tessera.Styles`

Advanced seams belong in opt-in lanes such as `Tessera.Hosting`.
Normal onboarding must not require `Tessera.Core.*`.

### Startup

Preferred entry points:

- `TesseraApplication.RunAsync(...)`
- `TesseraApplication.CreateBuilder()`
- `TesseraApplicationBuilder`
- `TesseraApplication`
- `TesseraRuntimeOptions`

Preferred startup forms:

- minimal: `TesseraApplication.RunAsync(new App())`
- configured: `TesseraApplication.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`

Tessera must not depend on Generic Host as the default framework identity.

### Authoring Style

Tessera should feel like idiomatic C#.

Preferred shape:

- explicit object models
- object initializers
- shallow builders when they help readability
- strong types over stringly-typed control contracts

Rejected shape:

- nested static layout mini-languages
- constructor trees that force users to parse screen structure by indentation
- bool-heavy orchestration APIs instead of typed options or smaller objects

### No Framework-Imposed App Architecture

Tessera must not require or imply:

- repository
- CQRS
- MVVM
- mediator
- unit of work

Those are application-level decisions.

### Extensibility

Custom widgets remain a core requirement.

Default path:

- built-in controls
- typed messages/effects
- screen/layout composition

Advanced path:

- custom controls/widgets
- low-level runtime seams
- advanced rendering/input behavior

## Public Architecture

### Application Model

Primary app contract:

- `TesseraApp`
- `TesseraApplication.RunAsync(...)`
- `TesseraApplication.CreateBuilder()`
- `TesseraApplicationBuilder.UseApp<TApp>()`
- `TesseraApplication`
- `TesseraRuntimeOptions`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Message`
- `TesseraEffect`
- `TesseraEffects`

Default app shape:

1. `Initialize()` may return the first effect.
2. `Update(Message)` handles typed app/runtime input.
3. `Build(ScreenContext)` returns the next screen.

Canonical learning path:

1. [overview.md](overview.md)
2. [install-and-prerequisites.md](install-and-prerequisites.md)
3. [first-app.md](first-app.md)
4. [examples.md](examples.md)
5. [showcase.md](showcase.md)

### Interaction Contract

Runtime pointer/input rules:

- `PointerEventKind.Motion` is hover-only
- `PointerActivationPolicy.DoubleClick` transfers focus on first press and activates on qualifying second press
- `PointerActivationPolicy.SingleClick` focuses and activates on first press
- runtime prefers CSI byte-stream decoding when terminal capabilities advertise pointer/focus/paste support
- `Console.ReadKey` fallback is for legacy non-CSI terminals only

### Composition Contract

Tessera uses explicit screen assembly.

Core default layout types:

- `WindowLayout`
- `RowLayout`
- `ColumnLayout`
- `PanelLayout`
- `CenterLayout`
- `LayoutSlot`
- `LayoutLength`

The default authoring model should read like explicit screen composition, not a foreign DSL.

### Control Contract

The public control surface is tracked in [public-api-inventory.md](public-api-inventory.md).

High-level rules:

- `Notifications` is the default/onboarding notification feed
- `NotificationInbox` is the advanced/dev-ops inbox surface
- `TelemetryChart` is the tiny-card telemetry control; `LinePlot` remains the larger plot surface
- `Button` must follow a terminal-equivalent box model:
  - one coherent rectangular body/background
  - fixed inner X/Y padding around content
  - centered content inside the padded rect
  - built-in defaults should own plain label chrome plus symmetric horizontal breathing room so examples do not need to restate them
  - surface styling must cover the whole allocated button body, including padded rows/columns, not just the post-padding content rect
  - focus should come from label emphasis and optional surface tint, not a second shell layer or border chrome
  - label styling and body styling are separate domains and must not bleed into one another
- `Selected*` is canonical naming
- existing `Current*` members remain compatibility aliases only where already shipped
- new bordered controls must ship border-style hooks, theme-token mapping, and regression coverage in the same slice

Custom-widget authoring remains supported through [custom-components.md](custom-components.md).

## Theme And Style Contract

V1 theming is semantic-token based with override hierarchy:

- semantic tokens for text, surface, border, state, focus, selection, and accent
- built-in palettes plus custom palette support
- precedence: global theme -> control type -> control instance -> state
- focus visuals must be theme-driven, not marker-only

Typography contract:

- ANSI emphasis intent is portable (`TesseraStyle`, `TesseraFontWeight`)
- terminal font requests are best-effort only
- terminal-specific caveats live in [terminal-font-capability-matrix.md](terminal-font-capability-matrix.md)

Detailed token and hook mapping lives in [theme-system.md](theme-system.md).

## V1 Boundaries

In scope for V1:

- public API simplification and boundary cleanup
- no-DI startup standardization
- theming/styling architecture for shipped controls
- broad built-in widget catalog
- regression tests, perf gates, docs alignment

Out of scope for V1:

- image rendering
- advanced native image modes
- anything that requires turning Tessera into a host-framework-first product
