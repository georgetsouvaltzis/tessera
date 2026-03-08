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

## Notes

- `TextInputComponent` wraps `TextInputModel` (single-line flow).
- `TextAreaComponent` uses multiline editing and viewport rendering with optional line numbers.
- `ListComponent<T>` wraps `ListModel<T>` including filtering and paging behaviors.
- `TableComponent` wraps `SortableTableComponent` for sort/page interactions.
- `DropdownComponent` provides open/close menu selection with configurable key bindings.
- `ComboboxComponent` combines text filtering with keyboard-driven option selection.
- `LogViewerComponent` supports append, filter, pause, clear, and scrolling.
- `LayoutContainerComponent` supports `Vertical`, `Horizontal`, and `Grid` layout modes.

## Gallery Example

A dedicated widget demo app is available:

```bash
dotnet run --project examples/TeaSharp.WidgetGallery/TeaSharp.WidgetGallery.csproj
```

Dedicated widget-focused demos are also available:

```bash
dotnet run --project examples/TeaSharp.DropdownExample/TeaSharp.DropdownExample.csproj
dotnet run --project examples/TeaSharp.ComboBoxExample/TeaSharp.ComboBoxExample.csproj
```

Core gallery hotkeys:

- `1..5`: switch demo tabs
- `tab`: cycle focus
- `enter`/`space`: activate focused button/dialog
- `left`/`right`: adjust progress when focused
- `d`: toggle dialog on Overlay tab
- `q` or `ctrl+c`: quit
