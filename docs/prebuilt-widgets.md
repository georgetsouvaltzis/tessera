# TeaSharp Prebuilt Widgets

TeaSharp provides a prebuilt widget layer in `TeaSharp.Components.PrebuiltWidgets` aimed at 1.0-ready app scaffolding.

## Available Widgets

1. `LabelComponent`
2. `ButtonComponent`
3. `TextInputComponent`
4. `TextAreaComponent`
5. `ListComponent<T>`
6. `TableComponent`
7. `ProgressBarComponent`
8. `DropdownComponent`
9. `ComboboxComponent`
10. `DialogComponent`
11. `StatusBarComponent`
12. `LogViewerComponent`
13. `LayoutContainerComponent`
14. `BadgeComponent`
15. `ToggleSwitchComponent`
16. `SliderComponent`
17. `SpinnerComponent`
18. `CommandPaletteComponent`
19. `TreeViewComponent`
20. `NotificationCenterComponent`
21. `MenuBarComponent`
22. `ContextMenuComponent`
23. `NumberInputComponent`
24. `DatePickerComponent`
25. `TimePickerComponent`
26. `MarkdownViewerComponent`

## Notes

- `TextInputComponent` wraps `TextInputModel` (single-line flow).
- `TextAreaComponent` uses multiline editing and viewport rendering with optional line numbers.
- `ListComponent<T>` wraps `ListModel<T>` including filtering and paging behaviors.
- `TableComponent` wraps `SortableTableComponent` for sort/page interactions.
- `DropdownComponent` provides open/close menu selection with configurable key bindings.
- `ComboboxComponent` combines text filtering with keyboard-driven option selection.
- `LogViewerComponent` supports append, filter, pause, clear, and scrolling.
- `LayoutContainerComponent` supports `Vertical`, `Horizontal`, and `Grid` layout modes.
- `CommandPaletteComponent` provides fuzzy command filtering and execution (`ctrl+p` default open key).
- `TreeViewComponent` provides hierarchical expand/collapse navigation with keyboard controls.
- `NotificationCenterComponent` provides persistent event feed, read/dismiss, and severity-based styling.
- `ToggleSwitchComponent`, `SliderComponent`, and `SpinnerComponent` provide interactive control primitives.
- `BadgeComponent` provides compact state/health labeling.
- `MenuBarComponent` and `ContextMenuComponent` provide top-level and contextual action surfaces.
- `NumberInputComponent`, `DatePickerComponent`, and `TimePickerComponent` provide structured value entry.
- `MarkdownViewerComponent` provides scrollable markdown rendering for docs/help panes.
- Most prebuilt widgets expose `ShowBorder` (`true` by default) for minimal/borderless layouts.
- `ListComponent<T>`, `DropdownComponent`, and `ComboboxComponent` support state-driven styling through:
  - `WidgetVisualState`
  - `WidgetStatePalette`
  - item resolvers (`ItemStateResolver` / `OptionStateResolver`)
  - palette inheritance (`Parent` / `InheritFrom(...)`)

### State Styling Example

```csharp
var list = new ListComponent<string>(["todo", "done"], x => x)
{
    ShowBorder = false,
    Focused = true,
    ItemStateResolver = item => item == "done"
        ? [WidgetVisualState.Completed]
        : [],
};

// Override the default completed style.
list.ItemStatePalette[WidgetVisualState.Completed] = new WidgetStateAppearance
{
    TextStyle = TeaStyle.Empty.WithStrikethrough().WithForeground(AnsiColor.BrightGreen),
    Prefix = "[x] ",
};

// Share app-level defaults through inheritance.
var appPalette = WidgetStatePalette.CreateDefault();
list.ItemStatePalette.InheritFrom(appPalette);
```

## Gallery Example

A dedicated widget demo app is available:

```bash
dotnet run --project examples/TeaSharp.WidgetGallery/TeaSharp.WidgetGallery.csproj
```

Dedicated widget-focused demos are also available:

```bash
dotnet run --project examples/TeaSharp.DropdownExample/TeaSharp.DropdownExample.csproj
dotnet run --project examples/TeaSharp.ComboBoxExample/TeaSharp.ComboBoxExample.csproj
dotnet run --project examples/TeaSharp.AdvancedWidgetsExample/TeaSharp.AdvancedWidgetsExample.csproj
dotnet run --project examples/TeaSharp.ProductivityWidgetsExample/TeaSharp.ProductivityWidgetsExample.csproj
```

Core gallery hotkeys:

- `1..5`: switch demo tabs
- `tab`: cycle focus
- `enter`/`space`: activate focused button/dialog
- `left`/`right`: adjust progress when focused
- `d`: toggle dialog on Overlay tab
- `q` or `ctrl+c`: quit
