# TeaSharp Migration Map

This is the working map from legacy pre-release APIs to the current default path.

## Startup

- `Tea.CreateProgram(model)` -> `Tea.RunAsync(app)` or `Tea.CreateBuilder().UseApp(...).Build().RunAsync()`
- advanced program hosting moved to `TeaSharp.Hosting.TeaHost`
- `TeaProgramOptions` -> `TeaRuntimeOptions` on the default path, or `TeaSharp.Hosting.TeaProgramOptions` on the advanced hosting path
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
- `ScreenRegionKey` -> implicit tree order unless you are on the advanced path
- `Stack` / `Split` / `Panel` / `Dock` / `Overlay` / `Center` / `Slot` static helper DSLs -> `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`

## Notes

- Legacy types are still available for now, but the ones with a root-level replacement are marked `EditorBrowsable(Advanced)`.
- Advanced screen composition still lives under the older component namespaces. Widgets without a root wrapper should be treated as advanced.
