# TeaSharp Migration Map

This is the working map from legacy pre-release APIs to the current default path.

## Startup

- `Tea.CreateProgram(model)` -> `Tea.RunAsync(app)` or `Tea.CreateBuilder().UseApp(...).Build().RunAsync()`
- advanced host customization moved to `TeaSharp.Hosting.TeaHost.CreateApplication(...)` / `RunAsync(...)`
- `TeaProgramOptions` -> internalized; migrate to `TeaRuntimeOptions` plus `TeaSharp.Hosting.TeaHostingOptions` when advanced hosting seams are required
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
- `BadgeComponent` -> `Badge`
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
- `ModalComponent` -> `Modal`
- `AccordionComponent` -> `Accordion`
- `AccordionSection` -> `TeaSharp.Controls.AccordionSection`
- `ContextMenuComponent` -> `ContextMenu`
- `ContextMenuItem` -> `TeaSharp.Controls.ContextMenuItem`
- `CommandPaletteComponent` -> `CommandPalette`
- `CommandPaletteItem` -> `TeaSharp.Controls.CommandPaletteItem`

## Composition

- `ScreenComposer` -> build `Screen` from `TeaSharp.Layout` object-model types
- `InteractiveScreenModel` -> `TeaApp` with automatic control routing and `Update(...)` for unhandled input/runtime messages
- `InputRouter` -> typed `Message` handling in `TeaApp.Update(...)`
- `ScreenRegionKey` -> internalized; rely on implicit tree order on the public path
- `Stack` / `Split` / `Panel` / `Dock` / `Overlay` / `Center` / `Slot` static helper DSLs -> internalized; replace with `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`

## Notes

- `TextBlockComponent`, `ButtonComponent`, `TextInputComponent`, `TextAreaComponent`, `StatusBarComponent`, `DropdownComponent`, and `ComboboxComponent` are removed, not merely hidden.
- Legacy types without a supported public replacement may still exist internally as bridges, but they are no longer part of the supported public path.
- Advanced screen composition still lives under the older component namespaces. Widgets without a root wrapper should be treated as advanced.
