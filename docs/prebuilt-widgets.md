# TeaSharp Control Catalog

TeaSharp now distinguishes between:

- root controls for the default app path
- advanced seams for specialized hosting and interop

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
27. `CommandPalette`
28. `ContextMenu`
29. `Badge`
30. `Modal`
31. `Accordion`
32. `Gauge`
33. `MiniLog`
34. `StatsCard`
35. `BarChart`
36. `LineChart`
37. `Breadcrumb`
38. `Paginator`
39. `Toolbar`
40. `CommandBar`
41. `SearchBox`
42. `DiffView`
43. `PropertyGrid`

These live in `TeaSharp.Controls`.

## Advanced Seams

Use advanced seams only when your app needs runtime or rendering control beyond normal app authoring.

Common advanced seams:

- `TeaSharp.Hosting.TeaHost`
- `TeaSharp.Hosting.TeaHostingOptions`
- `TeaSharp.Hosting.IProgramRenderer`
- `TeaSharp.Hosting.ITerminalAdapter`
- `TeaSharp.Hosting.IEventDecoder`
- `Screen.From(LayoutNode)` and `Screen.From(ICanvasComponent)` for explicit advanced composition interop
- `TryConsume...` control methods for transitional advanced polling scenarios

## How They Fit Together

Most applications should stay on:

- `TeaApp`
- `Screen.Build(...)`
- `TeaSharp.Controls`
- `TeaSharp.Layout`

Advanced seams are supported, but they should be opt-in and uncommon for regular app teams.

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
        Screen.Build(window =>
        {
            window.Body(new CenterLayout
            {
                Content = _combo,
                Width = 48,
                Height = 8,
            });
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

All of these now run on the new `TeaApp` startup/composition path, even when they demonstrate advanced seams.

## Migration Guidance

- Prefer root control names when available.
- Treat older `*Component` names as transitional and avoid using them in new app code.
- Keep `TeaSharp.Controls` and `TeaSharp.Layout` as the main app imports.
- Reach for advanced seams only when the root catalog and runtime options cannot cover the scenario.

See also:

- `docs/migration-map.md`
- `docs/namespace-migration.md`
