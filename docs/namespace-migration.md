# Namespace Migration Guide

TeaSharp now uses domain-based component namespaces instead of the old flat `TeaSharp.Components` surface.

## Recommended Imports

Most applications should import only what they use:

- `TeaSharp.Components.Primitives`
- `TeaSharp.Components.Composition`
- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.UiKit`
- `TeaSharp.Components.Advanced`
- `TeaSharp.Components.Charting`
- `TeaSharp.Components.Dashboard`

Advanced customization namespaces still exist, but they are no longer the default path:

- `TeaSharp.Components.Styling`
- `TeaSharp.Components.Interaction`

## Before / After

Before:

```csharp
using TeaSharp.Components;
```

After:

```csharp
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
```

## Common Moves

- `Canvas`, `Rect`, `BorderStyle`, `Widgets`:
  - from `TeaSharp.Components`
  - to `TeaSharp.Components.Primitives`
- `ICanvasComponent`, `IStatefulComponent`, `IFocusableComponent`, `ScreenComposer`, `ComponentComposer`, `InputRouter`, `InteractiveScreenModel`:
  - to `TeaSharp.Components.Composition`
- `ButtonComponent`, `TextInputComponent`, `DropdownComponent`, `ComboboxComponent`, `DialogComponent`, `LayoutContainerComponent`:
  - to `TeaSharp.Components.Prebuilt`
- `MenuBarComponent`, `ContextMenuComponent`, `NumberInputComponent`, `DatePickerComponent`, `TimePickerComponent`, `MarkdownViewerComponent`:
  - to `TeaSharp.Components.Productivity`
- `TabsComponent`, `ModalComponent`, `SortableTableComponent`, `UiTheme`, `Layout`:
  - to `TeaSharp.Components.UiKit`
- `CommandPaletteComponent`, `TreeViewComponent`, `NotificationCenterComponent`, `SliderComponent`, `SpinnerComponent`, `ToggleSwitchComponent`:
  - to `TeaSharp.Components.Advanced`
- `Charts`, `LineChartComponent`, `BarChartComponent`:
  - to `TeaSharp.Components.Charting`
- `GaugeComponent`, `StatsCardComponent`, `MiniLogComponent`:
  - to `TeaSharp.Components.Dashboard`

## Styling And Interaction

`TeaSharp.Components.Styling` and `TeaSharp.Components.Interaction` remain public for customization, but they are now treated as advanced namespaces.

Typical apps should only import them when they are actively customizing palettes or mouse/hover behavior.

## Migration Pattern

1. Replace `using TeaSharp.Components;`
2. Add the specific category namespaces your file actually uses
3. Only add `Styling` or `Interaction` when customizing component visuals or pointer behavior

## Minimal Screen Example

```csharp
using TeaSharp;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;

public sealed class SearchModel : InteractiveScreenModel
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
