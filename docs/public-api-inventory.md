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
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Screen.Build(...)`
- `TeaSharp.Layout.*` object-model types
- `TeaSharp.Controls.Button`
- `TeaSharp.Controls.Label`
- `TeaSharp.Controls.Badge`
- `TeaSharp.Controls.BadgeTone`
- `TeaSharp.Controls.Accordion`
- `TeaSharp.Controls.AccordionSection`
- `TeaSharp.Controls.TextInput`
- `TeaSharp.Controls.TextArea`
- `TeaSharp.Controls.Choice`
- `TeaSharp.Controls.ComboBox`
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
- `TeaSharp.Controls.RadioGroup`
- `TeaSharp.Controls.LogView`
- `TeaSharp.Controls.Modal`
- `TeaSharp.Controls.Notifications`
- `TeaSharp.Controls.Toggle`
- `TeaSharp.Controls.Slider`
- `TeaSharp.Controls.Spinner`
- `TeaSharp.Controls.StatusBar`
- `TeaSharp.Controls.Tabs`
- `TeaSharp.Controls.ListView<T>`
- `TeaSharp.Controls.Table`
- `TeaSharp.Controls.TreeItem`
- `TeaSharp.Controls.TreeView`
- `TeaSharp.Controls.MenuBar`
- `TeaSharp.Controls.MenuItem`
- `TeaSharp.Controls.Control`

The intended beginner path is:

- build an app by deriving from `TeaApp`
- run it with `Tea.RunAsync(...)` or `TeaApplicationBuilder`
- rely on automatic control routing; `Update(...)` handles only unhandled input plus runtime messages
- return `Screen` from `Build(ScreenContext)`
- assemble screens with `Screen.Build(...)` and shallow builder callbacks
- keep configuration in `TeaRuntimeOptions` and `ScreenOptions`

## Tier 2: Advanced But Supported

These APIs remain public because they still offer real value, but they should not dominate the default path.

- `TeaSharp.Hosting.TeaHostingOptions`
- `TeaSharp.Hosting.TeaHost.CreateApplication(...)`
- `TeaSharp.Hosting.TeaHost.RunAsync(...)`
- `TeaSharp.Hosting.IProgramRenderer`
- `TeaSharp.Hosting.AnsiDiffRenderer`
- `TeaSharp.Hosting.NullRenderer`
- `TeaSharp.Hosting.ITerminalAdapter`
- `TeaSharp.Hosting.ConsoleTerminalAdapter`
- `TeaSharp.Hosting.IEventDecoder`
- `TeaSharp.Hosting.EventDecoder`
- `TeaSharp.Controls.BarChartOptions`
- `TeaSharp.Controls.LineChartOptions`
- `ICanvasComponent`
- renderer, terminal, and capability-probing seams

Most of these types are now marked `EditorBrowsable(Advanced)`.

## Tier 3: Candidates For Further Narrowing

These areas still expose more mechanism than the long-term public design should:

- low-level widget models leaking through component configuration
- runtime seams that most apps never need
- duplicate terminology between root app types and older core/runtime types
- the remaining advanced component namespaces that still expose an alternate engine-shaped control story

## Current Direction

TeaSharp is shifting from:

- `InteractiveScreenModel`
- `ScreenComposer`
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

The old tree-oriented stack now mostly compiles behind the scenes as an internal bridge. The previous static layout helper DSL is also internal-only.

The first root controls that already own their implementation directly are:

- `Label`
- `Button`
- `TextInput`
- `TextArea`
- `Choice`
- `ComboBox`
- `ContextMenu`
- `CommandPalette`
- `Dialog`
- `ProgressBar`
- `LogView`
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
- `ListView<T>`
- `Tabs`
- `MenuBar`
- `StatusBar`

Their old `TeaSharp.Components.Prebuilt.*` counterparts have been removed instead of kept as compatibility wrappers.

## Design Constraints

- normal apps should stay in `TeaSharp`
- normal apps should not import `TeaSharp.Core.*`
- normal apps should not manage terminal size manually
- normal apps should not manage input scopes or region routing manually
- custom widgets should remain possible through a small stable contract

## Follow-up Targets

1. keep moving control authoring toward a single obvious configuration style
2. review Tier 2 periodically and internalize anything that is public only by inertia
3. keep custom widget extensibility stable while internal runtime details continue to shrink
4. preserve discoverability tests so legacy namespaces do not drift back onto the default path
