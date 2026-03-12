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
- `DialogComponent` -> `Dialog`
- `StatusBarComponent` -> `StatusBar`
- `TabsComponent` -> `Tabs`
- `ListComponent<T>` -> `ListView<T>`
- `TableComponent` -> `Table`
- `MenuBarComponent` -> `MenuBar`
- `MenuBarItem` -> `MenuItem`

## Composition

- `ScreenComposer` -> build `Screen` from `TeaSharp.Layout` object-model types
- `InteractiveScreenModel` -> `TeaApp` + `HandleScreenInput(...)`
- `InputRouter` -> typed `Message` handling in `TeaApp.Update(...)`
- `ScreenRegionKey` -> implicit tree order unless you are on the advanced path
- `Stack` / `Split` / `Panel` / `Dock` / `Overlay` / `Center` / `Slot` static helper DSLs -> `StackLayout`, `SplitLayout`, `PanelLayout`, `DockLayout`, `OverlayLayout`, `CenterLayout`, `LayoutSlot`

## Notes

- Legacy types are still available for now, but the ones with a root-level replacement are marked `EditorBrowsable(Advanced)`.
- Advanced widgets and advanced screen composition still live under the older component namespaces until they are redesigned or promoted.
