# TeaSharp Design Spec

## Overview

TeaSharp is a `.NET 10` terminal UI framework for state-driven applications.

Current design center:

- small root API
- explicit C# object model
- TeaSharp-owned startup
- screen/layout/control composition
- stable custom control contract
- advanced runtime seams kept separate from the default path

TeaSharp is pre-public. Breaking changes are allowed when they simplify the long-term API.

## Goals

- Keep the default authoring model learnable in one sitting.
- Prefer explicit object models over nested mini-DSLs.
- Let normal apps build screens without region ids, input scopes, or manual terminal bookkeeping.
- Preserve strong extensibility for custom widgets and advanced hosting.
- Keep rendering deterministic and testable.

## Non-Goals

- Generic Host as the framework identity.
- Prompt-style console helpers.
- Reproducing Terminal.Gui or Spectre.Console API shape.
- Requiring application architecture patterns such as repository, MVVM, CQRS, or mediator.

## Public Architecture

### Root Surface

Default namespaces:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`
- `TeaSharp.Styles`

Advanced namespace:

- `TeaSharp.Hosting`

The normal app path should not import `TeaSharp.Core.*`.

### Application Model

Primary app contract:

- `TeaApp`
- `Tea.RunAsync(...)`
- `Tea.CreateBuilder()`
- `TeaApplicationBuilder.UseApp<TApp>()`
- `TeaApplicationBuilder`
- `TeaApplication`
- `TeaRuntimeOptions`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Message`
- `TeaEffect`
- `TeaEffects`

App model shape:

1. `Initialize()` optionally returns the first effect.
2. `Update(Message)` handles typed input/runtime messages.
3. `Build(ScreenContext)` returns the next assembled screen.

Startup model:

- minimal path: `Tea.RunAsync(new App())`
- configured path: `Tea.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`

Canonical onboarding progression:

1. `examples/HelloWorld`: minimal startup path.
2. `examples/CounterForm`: configured startup path (`UseApp<TApp>()` + `ConfigureRuntime(...)`).
3. `examples/WorkspaceApp`: stateful multi-pane coordination with app-level messages/effects.
4. Advanced interaction lane: `examples/AdvancedWidgets` and `examples/WidgetGallery`.

Default onboarding remains in `TeaSharp`. `TeaSharp.Core` is a low-level advanced product lane.

### Theme Model

V1 theming is semantic-token based with override hierarchy:

- semantic tokens for text/surface/border/state/focus/selection/accent
- built-in palettes (Catppuccin, Rosé Pine) plus custom palette
- override precedence: global theme -> control type -> control instance -> state

Focus visuals must be theme-driven (for example focused border style/color), not limited to marker suffixes.

Image rendering is planned for V1.1.

### Composition Model

TeaSharp uses an object-based screen model.

Core default layout types:

- `WindowLayout`
- `RowLayout`
- `ColumnLayout`
- `PanelLayout`
- `CenterLayout`
- `LayoutSlot`
- `LayoutLength`

The default authoring model should read like explicit screen assembly, not nested layout-tree construction.

### Control Model

Root controls currently include:

- `Label`
- `Button`
- `Breadcrumb`
- `TextInput`
- `TextArea`
- `Choice`
- `ComboBox`
- `Dialog`
- `CommandPalette`
- `ContextMenu`
- `Notifications`
- `Toggle`
- `Slider`
- `Spinner`
- `StatusBar`
- `Tabs`
- `ListView<T>`
- `Table`
- `TreeItem`
- `TreeView`
- `MenuBar`
- `Toolbar`
- `CommandBar`
- `NumberInput`
- `DatePicker`
- `TimePicker`
- `MarkdownView`
- `MultiSelect`
- `Paginator`
- `SearchBox`
- `DiffView`
- `PropertyGrid`
- `RadioGroup`
- `ProgressBar`
- `LogView`
- `Badge`
- `Accordion`
- `Modal`
- `Gauge`
- `MiniLog`
- `StatsCard`
- `BarChart`
- `LineChart`

These types provide the default control vocabulary. Most promoted legacy `*Component` names are now internal bridges behind these controls.

### Custom Control Model

Custom widgets extend `TeaSharp.Controls.Control`.

That contract gives:

- render hook through `Render(Canvas, Rect)`
- typed message hook through `Handle(Message)`
- optional pointer-aware hook through `Handle(Message, Rect)`
- automatic bridge into the current runtime/composition engine without exposing the legacy component interfaces on the default path

The legacy component contracts still exist for advanced interop, but they are intentionally marked advanced and are no longer part of the normal custom-widget story.

Design rule:

- users should be able to write custom widgets without understanding `ScreenComposer`, routing scopes, or terminal protocol details

## Internal Architecture

### Runtime

The current runtime still uses the original core engine:

- internal `TeaRuntimeLoop`
- terminal adapters
- decoder
- renderer
- effect scheduling

Those remain the execution backend while the new root API compiles into them.

### Screen Compilation

The root screen model compiles through:

1. layout tree normalization
2. scene graph compilation
3. focus/input routing
4. canvas rendering
5. terminal output emission

The old public composition engine has been removed. Remaining advanced bridges are internal implementation details, not part of the app contract.

### Interaction

Default app code uses:

- automatic control input dispatch before `TeaApp.Update(...)`
- `TeaApp.Update(...)` for unhandled input plus runtime messages
- `RequestEffect(...)` when a control event needs to trigger runtime work
- typed key messages such as `KeyPressed`
- typed pointer messages such as `PointerInput`
- `TeaEffects` for quit/tick/sequence/batch behavior

Normal apps should not manually configure `InputRouter`, `InputScope`, or screen region chains.

## Advanced Layer

Advanced/custom-host scenarios can still reach:

- low-level renderers
- terminal capability probes
- decoder seams
- raw canvas drawing
- legacy composition helpers
- legacy `*Component` types without root wrappers yet

Most promoted legacy `*Component` families are now internal bridges behind root `TeaSharp.Controls` wrappers. The remaining public advanced layer is mainly hosting/runtime seams plus a smaller set of explicit interop contracts.

## Repo Profile

- SDK pinned: `10.0.103`
- main solution: `TeaSharp.slnx`
- test projects:
  - `tests/TeaSharp.Tests`
  - `tests/TeaSharp.IntegrationTests`

## Design Rules

- One obvious startup path.
- One obvious composition path.
- One obvious root control catalog.
- No namespace/type collisions on the public path.
- No stringly-typed routing identifiers on the normal path.
- No bool-heavy public orchestration APIs when a stronger object model is available.
- Simplicity for common apps; power for advanced users through deliberate extension points.
