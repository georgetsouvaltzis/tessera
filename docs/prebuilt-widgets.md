# TeaSharp Control Catalog

TeaSharp now distinguishes between:

- root controls for the default app path
- advanced seams for specialized hosting and interop

Root controls are C#-first and no-DI by default.

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
44. `FileExplorer`
45. `FuzzyFinder`
46. `ToastCenter`
47. `DataGrid`
48. `TreeTable`
49. `KeyValueList`
50. `Timeline`
51. `Stepper`

These live in `TeaSharp.Controls`.

### Dropdown Visual Defaults

- `Choice` and `ComboBox` now use richer dropdown glyph defaults for closed/open states.
- `Choice.Glyphs` and `ComboBox.Glyphs` use `DropdownGlyphSet` for explicit collapsed/expanded/highlighted/selected markers.
- Border text visuals are overrideable per control through `BorderStyleText` and `FocusedBorderStyleText`.
- Theme defaults map those border text hooks to semantic border/focus tokens.

### Focus Marker and Title Hooks

- `ListView<T>` and `TreeView` expose `FocusMarker` and `ShowFocusMarker` for title focus rendering.
- `TextInput` and `SearchBox` expose `FocusMarker` and `ShowFocusMarker` with `TitleStyle`/`FocusedTitleStyle`.
- `TextInput` and `SearchBox` style hooks include title/value/placeholder plus border text hooks (`BorderStyleText`/`FocusedBorderStyleText`).
- `TextArea`, `NumberInput`, `DatePicker`, and `TimePicker` also expose border text hooks for focused/unfocused frame rendering.

### Navigation Overlay Glyph and Border Hooks

- `MenuBar`, `ContextMenu`, and `CommandPalette` support `BorderStyleText`/`FocusedBorderStyleText`.
- Glyph customization is typed through `MenuBarGlyphSet`, `ContextMenuGlyphSet`, and `CommandPaletteGlyphSet`.

### Table and TreeView Visual Hooks

- `Table` supports `BorderStyleText` and `FocusedBorderStyleText` for focused/unfocused frame glyph rendering.
- `TreeView` supports the same border text hooks and typed marker customization via `TreeViewGlyphSet`.

### DataGrid and TreeTable Visual Hooks

- `DataGrid` supports `BorderStyleText`/`FocusedBorderStyleText`, `ColumnSeparatorText`, `SortAscendingMarker`, and `SortDescendingMarker`.
- `TreeTable` supports `BorderStyleText`/`FocusedBorderStyleText`, `ColumnSeparatorText`, row markers (`SelectedRowMarker`/`UnselectedRowMarker`), and tree markers (`ExpandedBranchMarker`/`CollapsedBranchMarker`/`LeafMarker`).

### ContextMenu Bordered Title Behavior

- Bordered `ContextMenu` titles now preserve focused `FocusMarker` output by reserving width for the rendered title marker text.

### Beautiful UI Checklist (Current Phase)

- Apply semantic theme first (`TeaRuntimeOptions.Theme`), then control-type, instance, and state overrides.
- Use explicit focus/title hooks (`FocusMarker`, `ShowFocusMarker`, `TitleStyle`, `FocusedTitleStyle`) on interactive controls.
- Use border text hooks where supported (`BorderStyleText`, `FocusedBorderStyleText`) to avoid hardcoded frame emphasis.
- Use typed glyph sets for symbolic affordances (`DropdownGlyphSet`, `TreeViewGlyphSet`) instead of inline string literals.
- Keep monochrome-safe defaults when style hooks are left empty.

## Theme Mapping Snapshot

Current shipped `TeaThemeControlExtensions` mappings include:

- basic controls: `Button`, `ListView<T>`, `StatusBar`, `TextInput`, `Table`, `Tabs`
- input/value controls: `TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`
- navigation controls: `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`
- navigation primitives: `Accordion`, `MultiSelect`, `RadioGroup`
- navigation overlay details: `Choice`/`ComboBox` include border text token mapping plus `DropdownGlyphSet` marker customization
- navigation overlay details: `TreeView` includes border text token mapping plus `TreeViewGlyphSet` marker customization
- navigation overlay details: `MenuBar`/`ContextMenu`/`CommandPalette` include border text token mapping plus typed glyph-set customization
- data/flow controls: `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`, `Stepper`
- data/flow details: `DataGrid` and `TreeTable` include border text token mapping plus explicit separator/marker text APIs
- explorer/feedback controls: `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`
- rendering text utilities: `Badge`, `LogView`, `MarkdownView`, `MiniLog`
- modal/chart summary controls: `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`

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
