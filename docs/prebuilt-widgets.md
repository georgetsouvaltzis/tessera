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
8. `TabsComponent`
9. `ModalComponent` / `DialogComponent`
10. `StatusBarComponent`
11. `LogViewerComponent`
12. `LayoutContainerComponent`

## Notes

- `TextInputComponent` wraps `TextInputModel` (single-line flow).
- `TextAreaComponent` uses multiline editing and viewport rendering with optional line numbers.
- `ListComponent<T>` wraps `ListModel<T>` including filtering and paging behaviors.
- `TableComponent` wraps `SortableTableComponent` for sort/page interactions.
- `LogViewerComponent` supports append, filter, pause, clear, and scrolling.
- `LayoutContainerComponent` supports `Vertical`, `Horizontal`, and `Grid` layout modes.

## Gallery Example

A dedicated widget demo app is available:

```bash
dotnet run --project examples/TeaSharp.WidgetGallery/TeaSharp.WidgetGallery.csproj
```

Core gallery hotkeys:

- `1..5`: switch demo tabs
- `tab`: cycle focus
- `enter`/`space`: activate focused button/dialog
- `left`/`right`: adjust progress when focused
- `d`: toggle dialog on Overlay tab
- `q` or `ctrl+c`: quit
