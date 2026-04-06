# TeaSharp Design Contract

TeaSharp is a `.NET 10` terminal UI framework for state-driven applications.

This document is the live product contract.
It describes what TeaSharp is, how the public API should feel, and what still belongs outside V1.
It should not act as a historical log or implementation diary.

## Product Position

TeaSharp is:

- a .NET-native framework for building state-driven terminal applications
- small on the public path
- explicit and strongly typed
- extensible without leaking internal engine details

TeaSharp is not:

- a Generic-Host-first framework
- a Terminal.Gui clone
- a Spectre.Console clone
- a Bubble Tea port
- a nested layout DSL disguised as C#

TeaSharp is still pre-public.
Breaking changes are acceptable when they simplify the long-term API and improve the default authoring path.

## V1 Design Center

V1 centers on:

- small root API
- explicit C# object model
- TeaSharp-owned startup
- screen/layout/control composition
- stable custom control contract
- semantic theming and override layers
- advanced runtime seams kept separate from the default path

## Non-Negotiable Rules

### Default Surface

Normal apps should primarily live in:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`
- `TeaSharp.Styles`

Advanced seams belong in opt-in lanes such as `TeaSharp.Hosting`.
Normal onboarding must not require `TeaSharp.Core.*`.

### Startup

Preferred entry points:

- `Tea.RunAsync(...)`
- `Tea.CreateBuilder()`
- `TeaApplicationBuilder`
- `TeaApplication`
- `TeaRuntimeOptions`

Preferred startup forms:

- minimal: `Tea.RunAsync(new App())`
- configured: `Tea.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`

TeaSharp must not depend on Generic Host as the default framework identity.

### Authoring Style

TeaSharp should feel like idiomatic C#.

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

TeaSharp must not require or imply:

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

- `TeaApp`
- `Tea.RunAsync(...)`
- `Tea.CreateBuilder()`
- `TeaApplicationBuilder.UseApp<TApp>()`
- `TeaApplication`
- `TeaRuntimeOptions`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Message`
- `TeaEffect`
- `TeaEffects`

Default app shape:

1. `Initialize()` may return the first effect.
2. `Update(Message)` handles typed app/runtime input.
3. `Build(ScreenContext)` returns the next screen.

Canonical example progression:

1. `examples/HelloWorld`
2. `examples/CounterForm`
3. `examples/WorkspaceApp`
4. `examples/AdvancedWidgets` / `examples/WidgetGallery`

### Interaction Contract

Runtime pointer/input rules:

- `PointerEventKind.Motion` is hover-only
- `PointerActivationPolicy.DoubleClick` transfers focus on first press and activates on qualifying second press
- `PointerActivationPolicy.SingleClick` focuses and activates on first press
- runtime prefers CSI byte-stream decoding when terminal capabilities advertise pointer/focus/paste support
- `Console.ReadKey` fallback is for legacy non-CSI terminals only

### Composition Contract

TeaSharp uses explicit screen assembly.

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

The public control surface is tracked in [public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md).

High-level rules:

- `Notifications` is the default/onboarding notification feed
- `NotificationInbox` is the advanced/dev-ops inbox surface
- `TelemetryChart` is the tiny-card telemetry control; `LinePlot` remains the larger plot surface
- `Button` must follow a terminal-equivalent box model:
  - one outer border/shell
  - one coherent inner body/background
  - fixed inner X/Y padding around content
  - centered content inside the padded rect
  - rounded-border buttons should read as a single pill/button surface, not nested chips or mini-cards
  - when apps need a distinct rounded outline plus inset body, `Button.RoundedSurfaceMode = InsetBody` should reserve enough inner height for that bordered-body treatment instead of collapsing back to a 3-row pill
  - `InsetBody` should also own plain-label chrome and minimum inner X breathing room when apps leave the default `Button` chrome settings in place
  - `UnifiedShell` should reserve enough height and horizontal shoulder/cap inset so the shell reads as a rounded pill instead of a clipped octagon; label-only pills should use the taller 7-row silhouette while description-bearing action buttons can stay on the tighter 5-row contract
  - compact filled rectangular buttons should prefer `BorderStyle.Heavy`; thin `SingleLine` borders plus full-cell surface fill can read like the fill escapes past the stroke on a terminal grid
  - `BorderStyleText` owns button border color; `BorderStyle.Heavy` is the compact bordered-button option when apps need stronger button affordance without the rounded-pill tradeoff
  - focus should be ring/border-led; surface tint may increase slightly, but focus must not create inner bands or competing layers
  - label styling, body styling, and shell styling are separate domains and must not bleed into one another
- `Selected*` is canonical naming
- existing `Current*` members remain compatibility aliases only where already shipped
- new bordered controls must ship border-style hooks, theme-token mapping, and regression coverage in the same slice

Custom-widget authoring remains supported through [custom-components.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/custom-components.md).

## Theme And Style Contract

V1 theming is semantic-token based with override hierarchy:

- semantic tokens for text, surface, border, state, focus, selection, and accent
- built-in palettes plus custom palette support
- precedence: global theme -> control type -> control instance -> state
- focus visuals must be theme-driven, not marker-only

Typography contract:

- ANSI emphasis intent is portable (`TeaStyle`, `TeaFontWeight`)
- terminal font requests are best-effort only
- terminal-specific caveats live in [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md)

Detailed token and hook mapping lives in [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md).

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
- anything that requires turning TeaSharp into a host-framework-first product
