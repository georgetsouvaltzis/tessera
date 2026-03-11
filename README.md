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
