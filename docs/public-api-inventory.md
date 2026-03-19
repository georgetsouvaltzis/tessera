# TeaSharp Public API Inventory

## Purpose

This document tracks the public API tiers so the pre-release redesign stays deliberate.

## Freeze Status

The current working freeze line is:

- Tier 1 is the supported default consumer path
- Tier 2 is the supported advanced escape-hatch path
- anything outside those tiers is still a candidate for internalization or deletion before first public release

Examples, README guidance, and starter docs should teach Tier 1 first. Tier 2 may remain public, but it should not be the default onboarding story.

## Tier 1: Default Consumer Path

These are the types new applications should discover first.

- `Tea`
- `TeaApp`
- `TeaApplication`
- `TeaApplicationBuilder`
- `TeaRuntimeOptions`
- `TeaEffect`
- `TeaEffects`
- `Message` and the typed message records in `TeaSharp`
- `TeaSharp.Styles.TeaStyle`
- `TeaSharp.Styles.AnsiColor`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Screen.Build(...)`
- `TeaSharp.Layout.*` object-model types
- `TeaSharp.Controls.Button`
- `TeaSharp.Controls.Breadcrumb`
- `TeaSharp.Controls.Label`
- `TeaSharp.Controls.Badge`
- `TeaSharp.Controls.BadgeTone`
- `TeaSharp.Controls.Accordion`
- `TeaSharp.Controls.AccordionSection`
- `TeaSharp.Controls.TextInput`
- `TeaSharp.Controls.TextArea`
- `TeaSharp.Controls.Choice`
- `TeaSharp.Controls.ComboBox`
- `TeaSharp.Controls.DropdownGlyphSet`
- `TeaSharp.Controls.CommandPalette`
- `TeaSharp.Controls.CommandPaletteItem`
- `TeaSharp.Controls.Dialog`
- `TeaSharp.Controls.ContextMenu`
- `TeaSharp.Controls.ContextMenuItem`
- `TeaSharp.Controls.ProgressBar`
- `TeaSharp.Controls.BarPoint`
- `TeaSharp.Controls.BarChart`
- `TeaSharp.Controls.LineChart`
- `TeaSharp.Controls.Gauge`
- `TeaSharp.Controls.MiniLog`
- `TeaSharp.Controls.StatItem`
- `TeaSharp.Controls.StatsCard`
- `TeaSharp.Controls.NumberInput`
- `TeaSharp.Controls.DatePicker`
- `TeaSharp.Controls.TimePicker`
- `TeaSharp.Controls.MarkdownView`
- `TeaSharp.Controls.MultiSelect`
- `TeaSharp.Controls.Paginator`
- `TeaSharp.Controls.RadioGroup`
- `TeaSharp.Controls.LogView`
- `TeaSharp.Controls.Modal`
- `TeaSharp.Controls.Notifications` (native)
- `TeaSharp.Controls.Toggle`
- `TeaSharp.Controls.Slider`
- `TeaSharp.Controls.Spinner`
- `TeaSharp.Controls.StatusBar`
- `TeaSharp.Controls.Tabs`
- `TeaSharp.Controls.ListView<T>`
- `TeaSharp.Controls.Table`
- `TeaSharp.Controls.DataGrid`
- `TeaSharp.Controls.TreeTable`
- `TeaSharp.Controls.KeyValueList`
- `TeaSharp.Controls.Timeline`
- `TeaSharp.Controls.Stepper`
- `TeaSharp.Controls.TreeItem`
- `TeaSharp.Controls.TreeView` (native)
- `TeaSharp.Controls.MenuBar`
- `TeaSharp.Controls.Toolbar`
- `TeaSharp.Controls.CommandBar`
- `TeaSharp.Controls.SearchBox`
- `TeaSharp.Controls.DiffView`
- `TeaSharp.Controls.PropertyGrid`
- `TeaSharp.Controls.FileExplorer`
- `TeaSharp.Controls.FuzzyFinder`
- `TeaSharp.Controls.ToastCenter`
- `TeaSharp.Controls.MenuItem`
- `TeaSharp.Controls.Control`

The intended beginner path is:

- build an app by deriving from `TeaApp`
- run it with the minimal startup lane (`Tea.RunAsync(new App())`) or configured startup lane (`Tea.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`)
- rely on automatic control routing; `Update(...)` handles only unhandled input plus runtime messages
- return `Screen` from `Build(ScreenContext)`
- assemble screens with `Screen.Build(...)` and shallow builder callbacks
- keep configuration in `TeaRuntimeOptions` and `ScreenOptions`
- follow canonical onboarding examples in order: `examples/HelloWorld` -> `examples/CounterForm` -> `examples/WorkspaceApp`
- treat `TeaSharp.Core` as the low-level advanced lane, not default onboarding
- use semantic theme tokens and palette-driven styling on the default path

## Tier 2: Advanced But Supported

These APIs remain public because they still offer real value, but they should not dominate the default path.

- `TeaSharp.Hosting.TeaHostingOptions`
- `TeaSharp.Hosting.TeaHost.CreateApplication(...)`
- `TeaSharp.Hosting.TeaHost.RunAsync(...)`
- `TeaSharp.Hosting.IProgramRenderer`
- `TeaSharp.Hosting.RenderOutput`
- `TeaSharp.Hosting.AnsiRendererOptions`
- `TeaSharp.Hosting.AnsiDiffRenderer`
- `TeaSharp.Hosting.NullRenderer`
- `TeaSharp.Hosting.ITerminalAdapter`
- `TeaSharp.Hosting.TerminalSize`
- `TeaSharp.Hosting.TerminalCapabilityProfile`
- `TeaSharp.Hosting.TerminalColorProfile`
- `TeaSharp.Hosting.ConsoleTerminalAdapter`
- `TeaSharp.Hosting.TerminalCapabilityDetector`
- `TeaSharp.Hosting.TerminalColorProfileDetector`
- `TeaSharp.Hosting.IEventDecoder`
- `TeaSharp.Hosting.EventDecodeResult`
- `TeaSharp.Hosting.EventDecoder`
- `TeaSharp.Hosting.TerminalCursorStyle`
- `TeaSharp.Controls.BarChartOptions`
- `TeaSharp.Controls.LineChartOptions`
- `ICanvasComponent` as a render-only advanced seam
- renderer, terminal, and capability-probing seams

Most of these types are now marked `EditorBrowsable(Advanced)`.

## Tier 3: Candidates For Further Narrowing

These areas still expose more mechanism than the long-term public design should:

- low-level widget models leaking through component configuration
- runtime seams that most apps never need
- duplicate terminology between root app types and older core/runtime types
- the remaining advanced component namespaces that still expose an alternate engine-shaped control story
- lower-level runtime/input helpers that still live deeper than the preferred TeaSharp-owned hosting surface
- overlap between `TeaSharp` and `TeaSharp.Core` mental models when boundaries are not documented clearly

## Current Direction

TeaSharp is shifting from:

- `InteractiveScreenModel`
- `InputRouter`

to:

- `Tea.RunAsync(...)`
- `TeaApplicationBuilder`
- `TeaApp`
- `Screen`
- `ScreenContext`
- `TeaRuntimeOptions`
- `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`
- root `TeaSharp.Controls` wrappers

The old `TeaHost.CreateProgram(...)` / `TeaProgramOptions` / `IScreen` program-hosting path has been removed.

The old `ScreenComposer` composition bridge has been removed. The previous static layout helper DSL is internal-only, and root layouts now compile through the scene compiler/runtime loop.

The first root controls that already own their implementation directly are:

- `Label`
- `Button`
- `TextInput`
- `TextArea`
- `Breadcrumb`
- `Choice`
- `ComboBox`
- `ContextMenu`
- `CommandPalette`
- `Dialog`
- `ProgressBar`
- `LogView`
- `Badge`
- `Slider`
- `Spinner`
- `Toggle`
- `NumberInput`
- `DatePicker`
- `TimePicker`
- `MarkdownView`
- `Modal`
- `Accordion`
- `RadioGroup`
- `MultiSelect`
- `Gauge`
- `MiniLog`
- `StatsCard`
- `BarChart`
- `LineChart`
- `ListView<T>`
- `Tabs`
- `DataGrid`
- `TreeTable`
- `KeyValueList`
- `Timeline`
- `Stepper`
- `MenuBar`
- `Toolbar`
- `CommandBar`
- `StatusBar`
- `Paginator`
- `SearchBox`
- `DiffView`
- `PropertyGrid`
- `FileExplorer`
- `FuzzyFinder`
- `ToastCenter`

Their old `TeaSharp.Components.Prebuilt.*` counterparts have been removed instead of kept as compatibility wrappers.

## Design Constraints

- normal apps should stay in `TeaSharp`
- normal apps should not import `TeaSharp.Core.*`
- normal apps should not manage terminal size manually
- normal apps should not manage input scopes or region routing manually
- custom widgets should remain possible through a small stable contract

## Theme Mapping Status

Current shipped theme mapping is centralized in `TeaSharp.Styles.TeaThemeControlExtensions` and split into domain partial files (`Basic`, `InputValue`, `Navigation`, `NavigationOverlay`, `NavigationPrimitives`, `DataAndFlow`, `ExplorerAndFeedback`, `RenderingTextUtilities`, `ModalAndCharts`).

Input/value controls with direct token mappings:

- `TextInput`, `TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`
- `TextInput` maps value/placeholder/focused-title styling; focus title marker is configurable through `FocusMarker` + `ShowFocusMarker`

Navigation/overlay controls with direct token mappings:

- `Choice`, `ComboBox`, `TreeView`, `MenuBar`, `ContextMenu`, `CommandPalette`, `Notifications`, `SearchBox`
- `Choice`/`ComboBox` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `Choice`/`ComboBox` keep glyph customization explicit through `DropdownGlyphSet`
- `TreeView` keeps title focus marker rendering configurable through `FocusMarker` + `ShowFocusMarker`
- `SearchBox` maps title/value/placeholder/match/navigation styles; title focus marker is configurable through `FocusMarker` + `ShowFocusMarker`

Navigation primitive controls with direct token mappings:

- `Accordion`, `MultiSelect`, `RadioGroup`

Rendering text utility controls with direct token mappings:

- `Badge`, `LogView`, `MarkdownView`, `MiniLog`

Modal/chart summary controls with direct token mappings:

- `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`

## Follow-up Targets

1. keep moving control authoring toward a single obvious configuration style
2. review Tier 2 periodically and internalize anything that is public only by inertia
3. keep `TeaSharp.Core` as the intentional low-level product and keep docs/examples explicit about when app authors should prefer `TeaSharp` instead
4. keep custom widget extensibility stable while internal runtime details continue to shrink and stay behind TeaSharp-owned internal adapters
5. preserve discoverability tests so legacy namespaces do not drift back onto the default path
6. keep V1 image scope out of the V1 default path docs (image rendering planned for V1.1)
