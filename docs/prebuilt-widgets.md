# TeaSharp Controls And Advanced Widgets

TeaSharp now distinguishes between:

- root controls for the default app path
- advanced `*Component` widgets for deeper customization

## Root Controls

Preferred public catalog:

1. `Label`
2. `Button`
3. `TextInput`
4. `TextArea`
5. `Choice`
6. `Dialog`
7. `StatusBar`
8. `Tabs`
9. `ListView<T>`
10. `Table`
11. `MenuBar`

These live in `TeaSharp.Controls`.

## Advanced Widgets

The older component catalog remains available when you need functionality that has not been promoted to the root catalog yet.

Common advanced widgets:

- `ComboboxComponent`
- `ProgressBarComponent`
- `LogViewerComponent`
- `CommandPaletteComponent`
- `TreeViewComponent`
- `NotificationCenterComponent`
- `ToggleSwitchComponent`
- `SliderComponent`
- `SpinnerComponent`
- `ContextMenuComponent`
- `DatePickerComponent`
- `TimePickerComponent`
- `MarkdownViewerComponent`

These live under:

- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.Advanced`

## How They Fit Together

The new screen model accepts both:

- root controls
- advanced components

So an app can stay on `TeaApp` + `Screen` + `Layout`, while still embedding an advanced widget directly inside a `LayoutSlot`, `PanelLayout`, or `CenterLayout`.

## Example

```csharp
using TeaSharp;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

internal sealed class ComboApp : TeaApp
{
    private readonly ComboboxComponent _combo = new(new ComboboxOptions(
        Items: ["alpha", "beta", "gamma"],
        Title: "Environment",
        Border: BorderStyle.SingleLine,
        Padding: Thickness.All(1)));

    public override TeaEffect? Update(Message message)
    {
        HandleScreenInput(message);
        return null;
    }

    public override Screen Build(ScreenContext context) =>
        Screen.From(new CenterLayout(_combo, width: 48, height: 8));
}
```

## Example Apps

Current example projects:

- `examples/Showcase`
- `examples/Dropdown`
- `examples/ComboBox`
- `examples/ProductivityWidgets`
- `examples/AdvancedWidgets`
- `examples/Kanban`
- `examples/WidgetGallery`

All of these now run on the new `TeaApp` startup/composition path, even when they demonstrate advanced widgets.

## Migration Guidance

- Prefer root control names when available.
- Treat older `*Component` names as advanced or transitional.
- Keep `TeaSharp.Controls` and `TeaSharp.Layout` as the main app imports.
- Reach for advanced namespaces only when the root catalog does not cover the scenario yet.

See also:

- `docs/migration-map.md`
- `docs/namespace-migration.md`
