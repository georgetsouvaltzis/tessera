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
6. `ComboBox`
7. `Dialog`
8. `ProgressBar`
9. `LogView`
10. `Notifications`
11. `Toggle`
12. `Slider`
13. `Spinner`
14. `StatusBar`
15. `Tabs`
16. `ListView<T>`
17. `Table`
18. `TreeItem`
19. `TreeView`
20. `MenuBar`
21. `NumberInput`
22. `DatePicker`
23. `TimePicker`
24. `MarkdownView`
25. `MultiSelect`
26. `RadioGroup`

These live in `TeaSharp.Controls`.

## Advanced Widgets

The older component catalog remains available when you need functionality that has not been promoted to the root catalog yet.

Common advanced widgets:

- `CommandPaletteComponent`
- `ContextMenuComponent`
- `BadgeComponent`
- `ModalComponent`
- `AccordionComponent`

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
using TeaSharp.Controls;
using TeaSharp.Layout;

internal sealed class ComboApp : TeaApp
{
    private readonly ComboBox _combo = new()
    {
        Title = "Environment",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    public ComboApp()
    {
        _combo.SetItems(["alpha", "beta", "gamma"]);
    }

    public override TeaEffect? Update(Message message) => null;

    public override Screen Build(ScreenContext context) =>
        Screen.From(new WindowLayout
        {
            Body = new CenterLayout(_combo, width: 48, height: 8),
        });
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
