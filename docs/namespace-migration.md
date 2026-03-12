# Namespace Migration Guide

TeaSharp no longer wants normal apps to start from the old category-heavy component surface.

## Default Imports

Prefer:

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
```

Add primitives only when needed:

```csharp
using TeaSharp.Components.Primitives;
```

## Namespace Direction

Old default mindset:

- `TeaSharp.Core.*`
- `TeaSharp.Components.Composition`
- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.UiKit`
- `TeaSharp.Components.Advanced`

New default mindset:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`

Advanced-only namespaces remain available, but they should be opt-in.

## Common Type Moves

### App Model

- `IScreen` -> `TeaApp`
- `Tea.CreateProgram(...)` -> `Tea.RunAsync(...)` or `Tea.CreateBuilder()`
- advanced program hosting moved to `TeaSharp.Hosting.TeaHost`
- `TeaProgramOptions` -> `TeaRuntimeOptions` on the default path, or `TeaSharp.Hosting.TeaProgramOptions` on the advanced hosting path
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

- `ScreenComposer`
- `ComponentComposer`
- `InteractiveScreenModel`
- `InputRouter`
- `ScreenRegionKey`
- low-level widget models in `TeaSharp.Widgets`
- advanced widgets that do not have root wrappers yet

## Migration Example

Before:

```csharp
using TeaSharp;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

internal sealed class SearchModel : InteractiveScreenModel
{
    private readonly TextInputComponent _input = new(new TextInputOptions(
        Title: "Search",
        Placeholder: "type here"));

    protected override void ComposeScreen(Rect bodyRect)
    {
        Screen.AddComponent(new ScreenRegionKey("search"), bodyRect, _input, focusable: true);
    }
}
```

After:

```csharp
using TeaSharp;
using TeaSharp.Components.Primitives;
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
        Screen.From(new WindowLayout
        {
            Body = new CenterLayout(_input, width: 48, height: 5),
        });
}
```

## Rule Of Thumb

If a normal app needs to import `TeaSharp.Core.*` or manually coordinate screen regions, the app is probably using the wrong layer.
