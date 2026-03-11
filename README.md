# TeaSharp

TeaSharp is a message-driven terminal UI library for .NET.

It keeps the default app path small:

- build apps around `Tea.CreateProgram(...)`
- configure runtime behavior with `TeaProgramOptions`
- compose screens with `ScreenComposer`, `InputRouter`, and `InteractiveScreenModel`
- use `TeaSharp.Layout` when you want intent-driven layout instead of manual geometry
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
- `TeaSharp.Layout`
- `Dashboard(...)`, `Form(...)`, and `MasterDetail(...)`
- `CreateDialogWorkflow(...)`
- options-first components and event-driven integration

## Quick Start

```csharp
using TeaSharp;
using TeaSharp.Core.Abstractions;

var program = Tea.CreateProgram(new CounterModel(), new TeaProgramOptions
{
    MaxFps = 60,
});

await program.RunAsync();
```

### Centered Hello World

```csharp
using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Layout;
using TeaSharp.Styles;

var program = Tea.CreateProgram(new HelloWorldScreen());
await program.RunAsync();

internal sealed class HelloWorldScreen : IScreen
{
    public Effect? Init() => null;

    public Effect? Update(IMessage message) => null;

    public ScreenOutput Render() =>
        ScreenOutput.From(
            Center.Text("Hello World", style: TeaStyle.Empty.WithBold()));
}
```

## Docs

- architecture and public surface: `docs/spec.md`
- recommended app shell: `docs/app-pattern.md`
- components: `docs/components.md`
- layout facade: `TeaSharp.Layout` (`Split`, `Stack`, `Panel`, `Center`, `Dock`, `Overlay`, `Slot`)
- prebuilt widgets: `docs/prebuilt-widgets.md`
- lower-level widgets: `docs/widgets.md`

## Build

The solution files remain useful for IDE navigation:

- `TeaSharp.slnx`
- `TeaSharp.Examples.slnx`
