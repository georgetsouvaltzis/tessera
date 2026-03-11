# TeaSharp

TeaSharp is a message-driven terminal UI library for .NET.

It keeps the default app path small:

- build apps around `Tea.NewProgram(...)`
- configure runtime behavior with `TeaProgramOptions`
- compose screens with `ScreenComposer`, `InputRouter`, and `InteractiveScreenModel`
- use the category namespaces for application-facing controls:
  - `TeaSharp.Components.Prebuilt`
  - `TeaSharp.Components.Productivity`
  - `TeaSharp.Components.UiKit`
  - `TeaSharp.Components.Advanced`
  - `TeaSharp.Components.Composition`
  - `TeaSharp.Components.Primitives`
- migration details: [docs/namespace-migration.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/namespace-migration.md)

If you want the canonical starter example for the current public app API, start with `examples/WidgetGallery`.
That sample demonstrates the intended path:

- `InteractiveScreenModel`
- `ScreenComposer`
- `Dashboard(...)`, `Form(...)`, and `MasterDetail(...)`
- `CreateDialogWorkflow(...)`
- options-first components and event-driven integration

## Quick Start

```csharp
using TeaSharp;
using TeaSharp.Core.Abstractions;

var program = Tea.NewProgram(new CounterModel(), new TeaProgramOptions
{
    MaxFps = 60,
});

await program.RunAsync();
```

## Docs

- architecture and public surface: `docs/spec.md`
- recommended app shell: `docs/app-pattern.md`
- components: `docs/components.md`
- prebuilt widgets: `docs/prebuilt-widgets.md`
- lower-level widgets: `docs/widgets.md`

## Build

Use the repo build scripts for deterministic CLI builds:

- `./scripts/build-main.sh`
- `./scripts/build-examples.sh`
- `./scripts/build-all.sh`

The solution files remain useful for IDE navigation:

- `TeaSharp.slnx`
- `TeaSharp.Examples.slnx`
