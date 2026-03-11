# TeaSharp Prebuilt Widgets

TeaSharp provides a prebuilt widget layer in `TeaSharp.Components.Prebuilt` aimed at 1.0-ready app scaffolding.

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

- `ButtonComponent` is now a first-class action control with unified keyboard + mouse activation.
  - Keyboard support: `enter` / `space` when focused.
  - Mouse support: hover tracking, left-click activation, pressed-state tracking, optional bordered rendering via `ButtonOptions.ShowBorder`, and configurable interaction behavior via `WidgetInteractionProfile`.
  - Component-level action state: `WasPressed`, `PressCount`, `Hovered`, `Pressed`.
- `TextInputComponent` provides single-line text entry with component-level accessors (`Value`, `SetValue`, `Placeholder`, `MaxLength`) plus cancel semantics via `CancelKey` (`esc` default), `CancelCount`, `WasCancelled`, and `LastCancelledValue`.
- `TextAreaComponent` provides multiline editing with component-level text access (`Value`, `SetValue`, `Clear`) and optional line numbers.
- `ListComponent<T>` provides filtering, selection, paging, and item replacement through component-level APIs (`SetItems`, `SetFilter`, `SelectedItem`, `SelectedIndex`, `PageSize`).
  - Mouse support: motion previews row hover (`▸` marker), left click selects a visible row, wheel scroll navigates selection.
- `TableComponent` exposes sort/page interactions directly (`PageSize`, `SortColumn`, `SortDescending`, `SetRows`, `SetVirtualWindow`) without leaking the lower-level table primitive.
- `DropdownComponent` provides open/close menu selection with configurable key bindings and a `DropdownOptions` constructor for common setup.
  - Mouse support: field click open/close, option click selection, wheel-driven highlight navigation when open.
- `ComboboxComponent` combines text filtering with keyboard-driven option selection through component-level filter access (`FilterText`, `Placeholder`, `SetFilterText`) and a `ComboboxOptions` constructor for common setup.
  - Mouse support: field click open/close, option click selection, wheel-driven highlight navigation when open.
- `LogViewerComponent` supports append, filter, pause, clear, and scrolling.
  - Friendly setup path: `LogViewerOptions`.
- `LayoutContainerComponent` supports `Vertical`, `Horizontal`, and `Grid` layout modes.
  - Mouse support: child hit-test routing and optional drag-resize split for 2-pane horizontal/vertical layouts (`PrimarySize`, `SetPrimarySize`, `ClearPrimarySize`).
  - Use `ComponentComposer` as the focus/routing owner when a layout container participates in a larger interactive screen.
- `CommandPaletteComponent` provides fuzzy command filtering and execution (`ctrl+p` default open key) with component-level query accessors (`QueryText`, `SetQueryText(...)`, `ClearQuery()`).
  - Mouse support: motion hover preview, wheel navigation, click execute, and outside-click close when open.
- `TreeViewComponent` provides hierarchical expand/collapse navigation with keyboard controls.
  - Mouse support: motion hover preview, click row selection, wheel navigation.
- `NotificationCenterComponent` provides persistent event feed, read/dismiss, and severity-based styling.
  - Mouse support: motion hover preview, click row selection, wheel navigation.
- `ToggleSwitchComponent`, `SliderComponent`, and `SpinnerComponent` provide interactive control primitives.
  - Mouse support: click activation and wheel interactions (toggle on/off, slider adjust, spinner advance).
- `BadgeComponent` provides compact state/health labeling.
- `MenuBarComponent` and `ContextMenuComponent` provide top-level and contextual action surfaces.
  - Mouse support: hover preview, click selection/execute, and wheel navigation.
  - Friendly setup paths: `MenuBarOptions`, `ContextMenuOptions`, and `SetItems(params ...)`.
- `NumberInputComponent`, `DatePickerComponent`, and `TimePickerComponent` provide structured value entry, with `NumberInputComponent.Text` exposing the rendered numeric text without leaking the text-input model.
  - Mouse support (`DatePickerComponent`, `TimePickerComponent`): day/field selection on click and wheel adjustment/navigation.
  - Friendly setup paths: `NumberInputOptions`, `DatePickerOptions`, `TimePickerOptions`.
- `MarkdownViewerComponent` provides scrollable markdown rendering for docs/help panes.
  - Friendly setup path: `MarkdownViewerOptions`.
- Most prebuilt widgets expose `ShowBorder` (`true` by default) for minimal/borderless layouts.
- Common widgets also expose options-based constructors for one-shot setup:
  - `LabelOptions`
  - `ButtonOptions`
  - `TextInputOptions`
  - `TextAreaOptions`
  - `ListOptions<T>`
  - `TableOptions`
  - `ProgressBarOptions`
  - `StatusBarOptions`
  - `DialogOptions`
  - `LayoutContainerOptions`
  - `TabsOptions`
  - `MenuBarOptions`
  - `ContextMenuOptions`
  - `NumberInputOptions`
  - `DatePickerOptions`
  - `TimePickerOptions`
  - `MarkdownViewerOptions`
  - `LogViewerOptions`
  - `ModalOptions`
- For narrower discovery, consumers can import the additive category catalogs:
  - `TeaSharp.Components.Prebuilt.PrebuiltCatalog`
  - `TeaSharp.Components.Productivity.ProductivityCatalog`
  - `TeaSharp.Components.UiKit.UiKitCatalog`
- Low-level key-map and interaction-profile properties remain supported, but are now marked advanced so the default surface stays focused on the common setup path.
- keep a single focus owner per interactive surface; for multi-pane screens that owner should be `ScreenComposer`, with `ComponentComposer` reserved for component subtrees.
- `ListComponent<T>`, `DropdownComponent`, and `ComboboxComponent` support state-driven styling through:
  - `WidgetVisualState`
  - `WidgetStatePalette`
  - item resolvers (`ItemStateResolver` / `OptionStateResolver`)
  - palette inheritance (`Parent` / `InheritFrom(...)`)
- Interactive prebuilt widgets expose `WidgetInteractionProfile` for unified hover/click/wheel behavior configuration.
- Components clone assigned `WidgetInteractionProfile` instances on ingress, so shared defaults can be reused safely without cross-component mutation.

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

### Options + Focus Example

```csharp
var input = new TextInputComponent(new TextInputOptions(
    Title: "Command",
    Placeholder: "type and press enter",
    ClearOnSubmit: true));

var progress = new ProgressBarComponent(new ProgressBarOptions(
    Title: "Deploy",
    Step: 0.1));

void SetFocus(int active)
{
    input.Focused = active == 0;
    progress.Focused = active == 1;
}

SetFocus(0);
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
dotnet run --project examples/TeaSharp.KanbanExample/TeaSharp.KanbanExample.csproj
```

Core gallery hotkeys:

- `1..5`: switch demo tabs
- `tab`: cycle focus
- `enter`/`space`: activate focused button/dialog
- `left`/`right`: adjust progress when focused
- `d`: toggle dialog on Overlay tab
- `q` or `ctrl+c`: quit
