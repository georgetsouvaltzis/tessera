# TeaSharp Components

TeaSharp includes a deterministic drawing and component layer split across category namespaces:

- `TeaSharp.Components.Primitives`
- `TeaSharp.Components.Composition`
- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.UiKit`
- `TeaSharp.Components.Advanced`
- `TeaSharp.Components.Charting`
- `TeaSharp.Components.Dashboard`

Recommended app shape: `docs/app-pattern.md`.

## Motivation

The design follows patterns used in Bubble Tea examples:

- `examples/cellbuffer/main.go`: fixed-size cell buffer drawing.
- `examples/canvas/main.go`: compositional layering/card layout.
- `examples/mouse/main.go`: event-driven interaction over rendered content.

## API

- `Primitives`:
  - `Rect`: immutable geometry helper with `Inset`, `Intersect`, and `Contains(x, y)` hit testing.
  - `Thickness`: standard spacing value object with `Left`, `Top`, `Right`, `Bottom`, `Horizontal`, `Vertical`, `All(...)`, and `Symmetric(...)`.
  - `Canvas`: fixed-size character grid renderer.
  - `Set`, `Get`, `WriteText`
  - `DrawHorizontalLine`, `DrawVerticalLine`, `DrawBox`
  - `DrawBox` supports `BorderStyle.None|SingleLine|Rounded|Heavy|Ascii`
  - `BorderStyle.Single` remains as a compatibility alias and is no longer the preferred spelling
  - text modes:
    - `CanvasTextMode.Fast` (default): char-cell fast path
    - `CanvasTextMode.GraphemeAware`: wide/combining grapheme-aware text placement
  - `Render` (returns full frame string)
- `Composition`:
  - `ICanvasComponent`: render-only component contract.
  - `IStatefulComponent`: component with `Update(IMessage)` for model-local state.
  - `IMouseStatefulComponent`: component with bounds-aware mouse handling (`UpdateMouse(MouseMsg, Rect)`).
  - `IFocusableComponent`: explicit focus contract for components that participate in keyboard focus.
  - `IInteractiveComponent`: convenience contract for focusable stateful mouse-aware components.
  - `KeyboardRoutingMode`: `FocusedOnly` (default) or `Broadcast`.
  - `ComponentComposer`: lower-level slot-based subtree composition (`Add`, `Clear`, `Update`, `Render`).
    - focus ownership APIs: `SetFocusedSlot`, `FocusFirst`, `FocusNext`, `FocusPrevious`, `ClearFocus`
    - mouse routing via slot hit-testing
    - explicit click-to-focus through `IFocusableComponent`
    - focused-slot keyboard routing by default
    - wheel fallback to focused slot when pointer is outside any slot
    - reserve this for local component trees and sub-layouts, not as the default full-app shell
  - `ScreenComposer`: named interactive regions for screen-scale layout snapshots.
    - region identity uses `ScreenRegionKey`; raw string overloads remain only as advanced convenience bridges
    - build once per frame, route later from the stored snapshot
    - `Frame(...)` creates a standard header/body/footer shell for common app layouts
    - focus ownership APIs: `SetFocus`, `FocusNext`, `FocusPrevious`
    - typed focus state via `FocusedRegionKey`
    - mouse routing by registered region bounds instead of repeated app-local rect math
    - preferred-focus selection via `CompleteFrame(...)`
    - overlay helpers: `AddOverlayComponent`, `AddModalComponent`, `AddPaletteComponent`, `AddToastOverlay`
    - this is the recommended full-app composition surface
  - `InputRouter`: app-level key precedence for multi-mode screens.
    - ordered scopes: `System`, `Modal`, `Palette`, `Command`, `FocusedRegion`, `Global`
    - scope behaviors: `ContinueWhenUnhandled` or `CaptureWhileActive`
    - optional `blocksGlobalShortcuts` guard for text-entry regions so plain character shortcuts do not leak into app-global handlers
  - `InteractiveScreenModel`: app-shell base for screen-oriented models.
    - owns `Screen`, `InputRouter`, lazy `EnsureScreen`, and per-frame `RenderScreen(...)`
    - app models implement `ComposeScreen(...)`, `GetBodyRect()`, and optional `PreferredFocusRegionKey`
- Namespace-shaped catalogs:
  - `TeaSharp.Components.Prebuilt.PrebuiltCatalog`
  - `TeaSharp.Components.Productivity.ProductivityCatalog`
  - `TeaSharp.Components.UiKit.UiKitCatalog`
  - these are optional factory entrypoints for consumers who prefer narrower discovery over direct `new ...(...)`
- `Widgets`:
  - `DrawPanel`
  - `DrawProgressBar`
  - `DrawSparkline`
  - `DrawList`
  - `DrawCard`
  - `DrawTable`
- `Charts`:
  - `Charts.DrawLineChart(...)`
  - `Charts.DrawBarChart(...)`
  - optional options records:
    - `LineChartOptions` (`ShowAxes`, `Legend`, `XLabel`, `YLabel`, `Zoom`, `Offset`)
    - `BarChartOptions` (`ShowScale`, `Legend`)
  - `LineChartComponent` (bounded sample history)
    - interactive helpers: `ZoomIn`, `ZoomOut`, `Pan`
  - `BarChartComponent` (named value bars)
- dashboard-oriented components:
  - `GaugeComponent`
  - `StatsCardComponent` + `StatsCardItem`
  - `MiniLogComponent`
- UI kit components and layout helpers:
  - `Layout` (`Classify`, `SplitVertical`, `SplitHorizontal`, `Grid`)
  - `UiTheme` (status fill, skeleton fill, modal backdrop)
  - `UiWidgets` (`DrawBreadcrumb`, `DrawStatusBar`, `DrawTimeline`, `DrawTree`, `DrawCalendar`, `DrawSkeleton`) with optional theme overloads
  - stateful components:
    - `TabsComponent`
    - `AccordionComponent`
    - `SortableTableComponent`
      - optional virtual window rendering (`EnableVirtualization`, `SetVirtualWindow`)
      - configurable key bindings (`NextPageKey`, `PreviousPageKey`, `ToggleSortDirectionKey`, `NextSortColumnKey`, `VirtualForwardKey`, `VirtualBackwardKey`)
    - `CheckboxListComponent`
    - `RadioGroupComponent`
    - `SelectComponent`
    - `ToastCenterComponent`
    - `ModalComponent`
    - UI-kit controls expose configurable key bindings for navigation/toggle actions (instead of fixed hardcoded keys).
  - prebuilt widget components (`PrebuiltWidgets`):
    - `LabelComponent`
    - `ButtonComponent`
    - `TextInputComponent`
    - `TextAreaComponent`
    - `ListComponent<T>`
    - `DropdownComponent`
    - `ComboboxComponent`
    - `TableComponent`
    - `ProgressBarComponent`
    - `StatusBarComponent`
    - `LogViewerComponent`
    - `DialogComponent`
    - `LayoutContainerComponent`
    - `BadgeComponent`
    - `ToggleSwitchComponent`
    - `SliderComponent`
    - `SpinnerComponent`
    - `CommandPaletteComponent`
    - `TreeViewComponent`
    - `NotificationCenterComponent`
    - `MenuBarComponent`
    - `ContextMenuComponent`
    - `NumberInputComponent`
    - `DatePickerComponent`
    - `TimePickerComponent`
    - `MarkdownViewerComponent`
    - prebuilt widgets now allow key behavior injection:
      - `TextInputComponent.KeyMap`
      - `TextInputComponent.CancelKey` (`esc` default), plus cancellation observability (`CancelCount`, `WasCancelled`, `LastCancelledValue`)
      - `TextAreaComponent.InputKeyMap` / `TextAreaComponent.ViewportKeyMap`
      - `ListComponent<T>.KeyMap`
      - `DropdownComponent.ToggleOpenKey`, `OpenKey`, `CloseKey`, `NextItemKey`, `PreviousItemKey`, `ConfirmSelectionKey`
      - `ComboboxComponent.InputKeyMap`, `OpenKey`, `CloseKey`, `NextItemKey`, `PreviousItemKey`, `ConfirmSelectionKey`
      - `LogViewerComponent.ViewportKeyMap`, `TogglePauseKey`, `ClearKey`
      - `ProgressBarComponent.IncreaseKey` / `DecreaseKey`
      - `DialogComponent.AcceptKey` / `DismissKey`
      - options-based constructors (`LabelOptions`, `ButtonOptions`, `TextInputOptions`, `TextAreaOptions`, `ListOptions<T>`, `TableOptions`, `ProgressBarOptions`, `StatusBarOptions`, `DialogOptions`, `LayoutContainerOptions`, `TabsOptions`, `MenuBarOptions`, `ContextMenuOptions`, `NumberInputOptions`, `DatePickerOptions`, `TimePickerOptions`, `MarkdownViewerOptions`, `LogViewerOptions`, `ModalOptions`)
      - optional catalog entrypoints (`PrebuiltCatalog`, `ProductivityCatalog`, `UiKitCatalog`) for namespace-scoped discovery
      - component-level state accessors instead of raw nested models (`TextInputComponent.Value` / `Placeholder` / `MaxLength`, `TextAreaComponent.Value`, `ListComponent<T>.SelectedItem` / `SetItems(...)`, `ComboboxComponent.FilterText` / `Placeholder`, `CommandPaletteComponent.QueryText` / `SetQueryText(...)` / `ClearQuery()`, `TableComponent.PageSize` / `SortColumn` / `SortDescending`, `NumberInputComponent.Text`, `ButtonComponent.WasPressed` / `PressCount` / `Hovered` / `IsPressed`)
      - action events for event-driven app code (`ButtonComponent.Pressed`, `TextInputComponent.Submitted` / `Cancelled`, `DialogComponent.Accepted` / `Dismissed`, `NumberInputComponent.Submitted`, `MenuBarComponent.ItemActivated`, `ContextMenuComponent.ItemExecuted`, `CommandPaletteComponent.ItemExecuted`)
      - selection/change events for navigation-heavy widgets (`ListComponent<T>.SelectionChanged`, `DropdownComponent.SelectionChanged`, `ComboboxComponent.SelectionChanged`, `TabsComponent.SelectionChanged`, `DatePickerComponent.DateChanged`, `TimePickerComponent.ValueChanged`)
      - one-shot interaction consumption helpers remain available when you want pull-style integration (`ButtonComponent.TryConsumePress()`, `TextInputComponent.TryConsumeSubmit(...)` / `TryConsumeCancel(...)`, `DialogComponent.TryConsumeResult(...)`, `NumberInputComponent.TryConsumeSubmit(...)`, `MenuBarComponent.TryConsumeActivation(...)`, `ContextMenuComponent.TryConsumeExecution(...)`, `CommandPaletteComponent.TryConsumeExecution(...)`)
      - low-level key-map / interaction-profile properties are still available, but now marked advanced so default IntelliSense emphasizes the common setup path
      - use `ScreenComposer` + `ScreenRegionKey` for screen-scale region orchestration, `InputRouter` for mode/global key precedence, and `InteractiveScreenModel` when an app follows the standard screen-shell pattern
      - use `ScreenComposer.MasterDetail(...)` or `InteractiveScreenModel.MasterDetail(...)` for the common header + master + detail + footer shell instead of rebuilding pane math per app
      - use `ScreenComposer.Dashboard(...)` or `InteractiveScreenModel.Dashboard(...)` for the common header + sidebar + main + footer shell
      - use `ScreenComposer.Form(...)` or `InteractiveScreenModel.Form(...)` for the common header + body + actions + footer shell
      - use `CreateDialogWorkflow(...)` when modal dialogs need open/close plus focus-restore behavior
      - focus helpers for app shells (`CreateFocusChain(...)`, `HandleTabNavigation(...)`, `CaptureFocus()`, `RestoreFocus(...)`, `FocusFirstInteractive()`)
      - border-capable widgets now expose `Border` (`BorderStyle`) plus `Padding` (`Thickness`) so frame appearance and inner spacing use standard UI terms
      - state styling primitives for child items (`WidgetVisualState`, `WidgetStatePalette`, `ItemStateResolver`/`OptionStateResolver`)
      - state palette inheritance (`WidgetStatePalette.Parent` / `InheritFrom(...)`)
      - shared interaction behavior profile (`WidgetInteractionProfile`) for hover/click/wheel semantics
      - mouse interactions:
        - `ButtonComponent` unified action handling: hover, click activation, and the same action path for mouse + `enter/space`
        - `ListComponent<T>` row hover preview (motion), click selection, and wheel navigation
        - `TabsComponent` motion hover preview, click tab activation, and wheel tab cycling
        - `SortableTableComponent` motion row preview, click row selection/header sort toggle, and wheel page/window navigation
        - `DropdownComponent` and `ComboboxComponent` field click-open, option click-select, and wheel highlight navigation
        - `TreeViewComponent` and `NotificationCenterComponent` motion hover preview, click row selection, and wheel navigation
        - `CommandPaletteComponent` motion hover preview, wheel navigation, click execute, and outside-click close
        - `ToggleSwitchComponent`, `SliderComponent`, and `SpinnerComponent` click activation + wheel interactions
        - `MenuBarComponent` and `ContextMenuComponent` motion hover preview, click activation, wheel navigation, and options-first item setup
        - `DatePickerComponent` and `TimePickerComponent` click day/field selection plus wheel adjustments
        - `LayoutContainerComponent` child mouse routing + optional drag-resize split (`PrimarySize`, `SetPrimarySize`, `ClearPrimarySize`)

## Example Integration

`Showcase` now has a dashboard page (press `2`) that renders:

- system status panel
- count gauge
- line chart (throughput)
- bar chart (status mix)
- capability stats card
- action/state table
- mini live-event log + scrollable log viewport
- command input footer

The protocol probe page remains available (press `1`) for low-level VT debugging.
Capability showcase page is available on `3` and demonstrates grapheme-aware canvas rendering plus custom component composition (`UnicodeShowcaseComponent`).
Showcase page now cycles multiple UI surfaces with `left/right` tabs:
- `Overview`: unicode/timeline/tree/calendar
- `Data`: line/bar charts + sortable/paged table
- `Forms`: accordion + checklist + radio/select + summary card
Showcase routes keyboard to one focused pane at a time. Use `tab` to move focus to the showcase region, then `p`/`P` to cycle pane focus.
Workspace pages have explicit input modes:
- `nav`: safe navigation mode (no single-letter side effects)
- `cmd`: command/hotkey mode
Press `:` to enter `cmd` mode and focus command input immediately. Press `esc` to return to `nav` mode and restore prior non-command focus.
Showcase pane hotkeys are available when showcase pane focus is active (including `nav` mode): `t` toast, `m` modal, `a` accordion, `z` checklist, `r` theme, `f` density, `c` table column, `v` table sort, `[`/`]` table page, `p`/`P` pane cycle.
The dashboard composes chart components through `ComponentComposer` and uses stateful models from `TeaSharp.Widgets`.

## Widget Gallery App

A dedicated app now exists for the prebuilt widget set:

- project: `examples/WidgetGallery`
- run: `dotnet run --project examples/WidgetGallery/WidgetGallery.csproj`
- docs: `docs/prebuilt-widgets.md`
- this is the recommended copy/paste starter for the current public app API surface

Scenario app example:

- project: `examples/Kanban`
- run: `dotnet run --project examples/Kanban/Kanban.csproj`
- flow: multi-board Kanban with lane movement, quick card creation, delete confirmation dialog, and activity feed.
- note: this app still uses a more manual `IModel` composition style and should be treated as an advanced example, not the default starter template

## Custom Components

Create custom components by implementing `ICanvasComponent`:

```csharp
public sealed class ClockComponent : ICanvasComponent
{
    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, "Clock");
        var body = rect.Inset(1, 1);
        canvas.WriteText(body.X, body.Y, DateTimeOffset.Now.ToString("HH:mm:ss"), body.Width);
    }
}
```

If the component owns local state and needs messages, implement `IStatefulComponent` and route messages through `ComponentComposer.Update(message)` only when building a local component subtree.
If it should take focus inside composed layouts, also implement `IFocusableComponent`.

For a fuller guide with a custom component walkthrough, see `docs/custom-components.md`.
For the recommended multi-pane app shell, see `docs/app-pattern.md`.
