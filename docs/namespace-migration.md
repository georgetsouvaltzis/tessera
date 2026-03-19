# Namespace Migration Guide

TeaSharp no longer wants normal apps to start from the old category-heavy component surface.

## Default Imports

Prefer:

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
```

Add drawing primitives only when needed:

```csharp
using TeaSharp;
using TeaSharp.Components.Primitives;
```

## Namespace Direction

Old default mindset:

- `TeaSharp.Core.*`
- `TeaSharp.Components.Composition`
- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.UiKit`

New default mindset:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`

Advanced-only namespaces remain available, but they should be opt-in.

`TeaSharp.Core` stays supported as the low-level product, not the default app path.

## Common Type Moves

### App Model

- `IScreen` -> `TeaApp`
- `Tea.CreateProgram(...)` -> `Tea.RunAsync(...)` or `Tea.CreateBuilder()`
- advanced host customization moved to `TeaSharp.Hosting.TeaHost.CreateApplication(...)` / `RunAsync(...)`
- `TeaProgramOptions` -> internalized; use `TeaRuntimeOptions` and `TeaSharp.Hosting.TeaHostingOptions`
- `ScreenOutput` / `TerminalOutput` default path -> `Screen` + `ScreenOptions`

### Default Controls

- `TextBlockComponent` -> `Label`
- `ButtonComponent` -> `Button`
- `TextInputComponent` -> `TextInput`
- `TextAreaComponent` -> `TextArea`
- `DropdownComponent` -> `Choice`
- `DialogComponent` -> `Dialog`
- `StatusBarComponent` -> `StatusBar`
- `TabsComponent` -> `Tabs`
- `ListComponent<T>` -> `ListView<T>`
- `TableComponent` -> `Table`
- `MenuBarComponent` -> `MenuBar`

### Layout

- `UiKit.Layout` -> `TeaSharp.Layout.*`
- `Frame` / `Dashboard` / `Form` helpers -> `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`

## What Stayed Advanced

These still exist, but they are not the first path:

- hosting/runtime seams in `TeaSharp.Hosting`
- advanced composition interop seams (`Screen.From(LayoutNode)`, `Screen.From(ICanvasComponent)`)

## Migration Example

Before:

Historical pre-redesign example:

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Components.Primitives;

internal sealed class SearchScreen : TeaApp
{
    private readonly TextInput _input = new()
    {
        Title = "Search",
        Placeholder = "type here",
    };

    public override Screen Build(ScreenContext context)
    {
        return Screen.Build(screen =>
        {
            screen.Content(content =>
            {
                content.Add(_input);
            });
        });
    }

    public override Effect? Update(Message message) => null;
}
```

That path no longer exists in the supported implementation. The `TextInputComponent` / `TextInputOptions` pair shown above is removed; use `TeaApp` and the root `TeaSharp.Controls` / `TeaSharp.Layout` surface instead.

After:

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

internal sealed class SearchApp : TeaApp
{
    private readonly TextInput _input = new()
    {
        Title = "Search",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Placeholder = "type here",
    };

    public override TeaEffect? Update(Message message)
    {
        return null;
    }

    public override Screen Build(ScreenContext context) =>
        Screen.Build(window =>
        {
            window.Body(new CenterLayout
            {
                Content = _input,
                Width = 48,
                Height = 5,
            });
        });
}
```

## Rule Of Thumb

If a normal app needs to import `TeaSharp.Core.*` or manually coordinate screen regions, the app is probably using the wrong layer.
