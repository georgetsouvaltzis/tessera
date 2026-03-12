# TeaSharp Migration Map

This is the working map from legacy pre-release APIs to the current default path.

## Startup

- `Tea.CreateProgram(model)` -> `Tea.RunAsync(app)` or `Tea.CreateBuilder().UseApp(...).Build().RunAsync()`
- `TeaProgramOptions` -> `TeaRuntimeOptions`
- `IScreen` -> `TeaApp`
- `Effect` helpers -> `TeaEffects`
- `ScreenOutput` / `TerminalOutput` -> `Screen` / `ScreenOptions`

## Default Controls

- `TextBlockComponent` -> `Label`
- `ButtonComponent` -> `Button`
- `TextInputComponent` -> `TextInput`
- `TextAreaComponent` -> `TextArea`
- `DropdownComponent` -> `Choice`
- `ComboboxComponent` -> `ComboBox`
- `DialogComponent` -> `Dialog`
- `ProgressBarComponent` -> `ProgressBar`
- `LogViewerComponent` -> `LogView`
- `NotificationCenterComponent` -> `Notifications`
- `ToggleSwitchComponent` -> `Toggle`
- `SliderComponent` -> `Slider`
- `SpinnerComponent` -> `Spinner`
- `StatusBarComponent` -> `StatusBar`
- `TabsComponent` -> `Tabs`
- `ListComponent<T>` -> `ListView<T>`
- `TableComponent` -> `Table`
- `TreeViewComponent` -> `TreeView`
- `TreeItemNode` -> `TreeItem`
- `MenuBarComponent` -> `MenuBar`
- `MenuBarItem` -> `MenuItem`

## Composition

- `ScreenComposer` -> build `Screen` from `TeaSharp.Layout` object-model types
- `InteractiveScreenModel` -> `TeaApp` with automatic control routing and `Update(...)` for unhandled input/runtime messages
- `InputRouter` -> typed `Message` handling in `TeaApp.Update(...)`
- `ScreenRegionKey` -> implicit tree order unless you are on the advanced path
- `Stack` / `Split` / `Panel` / `Dock` / `Overlay` / `Center` / `Slot` static helper DSLs -> `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`

## Notes

- Legacy types are still available for now, but the ones with a root-level replacement are marked `EditorBrowsable(Advanced)`.
- Advanced widgets and advanced screen composition still live under the older component namespaces until they are redesigned or promoted.
