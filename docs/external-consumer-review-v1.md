# External Consumer Review (V1)

Context: clean-room consumer pass by building `examples/ExternalConsumerReviewApp` using only `Tea`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`.

## Ranked Friction

### P1 - `Table` selection state/event access
- Status: resolved in `51d46a3`.
- Prior issue: table supported keyboard/mouse row selection visually, but consumers could not read selected row index/item or subscribe to selection changes.
- Repro snippet:
```csharp
var table = new Table("Name", "State");
table.SetRows(rows);
var selectedIndex = table.SelectedRowIndex;
var selectedRow = table.SelectedRow;
var hasSelectedRow = table.TryGetSelectedRow(out var row);
table.SelectionChanged += (_, args) => { };
```

### P1 - Runtime theme switch requires per-control boilerplate
- Status: resolved in `8ff286d`.
- Prior issue: switching between Catppuccin/Rosé Pine required manually re-applying theme to every control instance.
- Repro snippet:
```csharp
var applied = ThemeScope.Apply(theme, _tabs, _table, _list, _notifications, _dialog, _status);
```
- Current V1 state: `TeaThemeOverrideBundle` remains useful for local override composition; `ThemeScope.Apply(...)` now covers first-party global fan-out.

## Resolved In This Lane

### P2 - `Notifications` selection event
- Status: resolved with additive API.
- New surface: `Notifications.SelectionChanged` (`ListSelectionChangedEventArgs<InboxItem>` payload).
- Validation: `NotificationsSelectionChangedEventTests` covers `SetSelectedIndex`, `Clear`, and `RemoveSelected` transitions.

## Notes
- Current example demonstrates navigation, list/table, notifications, dialog flow, theme switching, and a dedicated `Dashboard APIs` tab without `TeaSharp.Core`.
- No breaking API changes proposed in this pass.

## Wave 3 - Dashboard API Tranche Re-Audit (March 22, 2026)

Context: extended `examples/ExternalConsumerReviewApp` to exercise newly landed dashboard/control APIs and theme-extension paths from an external consumer perspective.

### Resolved (evidence-backed)

#### Dashboard visualization/control primitives are now consumable from onboarding namespaces
- Status: resolved.
- APIs exercised in example:
  - `DashboardGrid.SetTiles(...)`, `DashboardGrid.SelectionChanged`
  - `BulletChart.SetRanges(...)`, `BulletChart.SetValue(...)`, `BulletChart.SetTarget(...)`
  - `HealthBoard.SetServices(...)`, `HealthBoard.SetSelectedIndex(...)`, `HealthBoard.Acknowledge(...)`
  - `BoxPlot.SetSeries(...)`
- Evidence commits:
  - `8593562` (`DashboardGrid`)
  - `dfd4221` (`BulletChart`)
  - `18adc16` (`HealthBoard`)
  - `6ae3c5b` (`BoxPlot`)

#### Dashboard workflow/navigation surfaces are externally usable
- Status: resolved.
- APIs exercised in example:
  - `SideNavRail.SetItems(...)`, `SideNavRail.SelectionChanged`, `SideNavRail.Activated`
  - `ResizablePaneGroup.SetPanes(...)`, `ResizablePaneGroup.SetSplitRatio(...)`
  - `JumpList.SetItems(...)`, `JumpList.Activated`
  - `AutocompleteInput.SetSuggestions(...)`, `AutocompleteInput.SuggestionCommitted`
  - `QuickOpenOverlay.SetItems(...)`, `QuickOpenOverlay.Open()`, `QuickOpenOverlay.Submitted`
- Evidence commits:
  - `d236de2` (`SideNavRail`)
  - `77cc95d` (`ResizablePaneGroup`)
  - `187468c` (`JumpList`)
  - `50212de` + `be23de7` (`AutocompleteInput`)
  - `d91c934` (`QuickOpenOverlay`)

#### Theme/styling extension path exists for expansion tranche controls
- Status: resolved.
- APIs exercised in example:
  - `ApplyTheme(this TControl, TeaTheme)` and `ApplyThemeDefaults(this TControl, TeaTheme)` for the above controls
  - control-level focus/border overrides after theme apply (external-app layering)
- Evidence commits:
  - `4e005ed`, `1c1b748`, `03c7a43`, `db63e01` (theme/parity wiring coverage)

### Open (re-audited)
No active P1/P2 open findings in this scope after `51d46a3`, `a9f774f`, and `8ff286d`.

### Resolved After Latest Landed Commits (March 22, 2026)

#### P1 - `StatsCard` border/padding style hooks
- Status: resolved by `c21a6ce`.
- New consumer surface:
  - `StatsCard.Border`
  - `StatsCard.Padding`
  - `StatsCard.BorderStyleText`
  - `StatsCard.FocusedBorderStyleText`
- Theme wiring evidence:
  - `TeaThemeControlExtensions.ApplyTheme(this StatsCard, TeaTheme)` and `ApplyThemeDefaults(...)` now map border tokens.

#### P1 - Plot retention ergonomics
- Status: resolved by `4fc0ca3` + `1781ee7`.
- New consumer surface:
  - `LineSeries.Capacity`, `LineSeries.TrimToLast(int)`
  - `ScatterPlot.Capacity`, `ScatterPlot.TrimToLast(int)`
  - `Sparkline.TrimToLast(int)` with retained `Capacity`

#### P1 - Mixed-unit `LinePlot` readability
- Status: resolved by `4fc0ca3`.
- New consumer surface:
  - `LineSeries.ScaleMode` (`Shared`/`Normalized`)
  - `LinePlotOptions.SharedAxisLabel`
  - `LinePlotOptions.NormalizedAxisLabel`

#### P2 - Generic `Options` discoverability for `LinePlot`
- Status: resolved by `4fc0ca3`.
- New consumer helpers:
  - `LinePlot.ConfigureAxes(...)`
  - `LinePlot.ConfigureGrid(...)`
  - `LinePlot.ConfigureLegend(...)`

### Resolved After Follow-up Consumer API Commits (March 22, 2026)

#### P1 - `Table` selection state/event gap
- Status: resolved by `51d46a3`.
- New consumer surface:
  - `Table.SelectedRowIndex`
  - `Table.SelectedRow`
  - `Table.TryGetSelectedRow(...)`
  - `Table.SelectionChanged`

#### P2 - `ListView<T>` programmatic selection setter gap
- Status: resolved by `a9f774f`.
- New consumer surface:
  - `ListView<T>.SetSelectedIndex(int)`
  - `ListView<T>.Select(int)` (compatibility wrapper)

#### P2 - Runtime theme fan-out helper gap
- Status: resolved by `8ff286d`.
- New consumer surface:
  - `ThemeScope.Apply(TeaTheme, params Control[])`
  - `ThemeScope.Apply(TeaTheme, IEnumerable<Control?>)`

## Wave 2 - Advanced Dashboard (Styling/State/Plot Readiness)

Context: implemented an `Analytics` screen in `examples/ExternalConsumerReviewApp` with status cards, endpoint table, and `LinePlot`.

### P1 - `StatsCard` border/padding style hooks
- Status: resolved in `c21a6ce`.
- Prior issue: `StatsCard` always drew default box chrome, so dashboards could not align card borders with theme border tokens.
- Repro snippet:
```csharp
var card = new StatsCard { Title = "Latency" };
card.ApplyTheme(theme);
card.Border = BorderStyle.Rounded;
card.Padding = Thickness.All(1);
```

### P1 - Plot streaming retention ergonomics
- Status: resolved in `4fc0ca3` + `1781ee7`.
- Prior issue: `LineSeries.Append(...)` was unbounded and required manual trim copies.
- Repro snippet:
```csharp
var series = new LineSeries("Req/s");
series.Capacity = 240;
series.Append(nextValue); // auto-trims old samples
series.TrimToLast(120);
```

### P1 - `LinePlot` mixed-unit readability
- Status: resolved in `4fc0ca3`.
- Prior issue: mixed units (for example req/s and ms) flattened one series under one shared Y scale.
- Repro snippet:
```csharp
plot.SetSeries([
    new LineSeries("Req/s", reqSeries) { ScaleMode = LineSeriesScaleMode.Shared },
    new LineSeries("P95 ms", latencySeries) { ScaleMode = LineSeriesScaleMode.Normalized },
]);
plot.ConfigureAxes(showAxes: true, sharedAxisLabel: "req/s", normalizedAxisLabel: "ms (norm)");
```

### P2 - `LinePlot` options discoverability
- Status: resolved in `4fc0ca3` (`LinePlot`); note that other plotting controls still expose advanced `Options` directly.
- Repro snippet:
```csharp
var plot = new LinePlot();
plot.ConfigureAxes(showAxes: true, sharedAxisLabel: "value");
plot.ConfigureGrid(true);
plot.ConfigureLegend(true);
```

## Wave 4 - ControlPlaneOpsDashboard Implementation Feedback (March 23, 2026)

Context: built `examples/ControlPlaneOpsDashboard` as a coherent control-plane app using only public onboarding namespaces.

### What was easy
- Cross-widget composition is strong: `Tabs`, `SideNavRail`, `DashboardGrid`, `Table`, `HealthBoard`, `Notifications`, `TaskRunnerPanel`, and overlays integrate without touching internals.
- Runtime theme fan-out + local override flow is practical for real screens (`ApplyTheme(...)`, dashboard override bundle pattern).
- Periodic updates are straightforward via `TeaEffects.Periodic(...)`.

### Friction encountered while implementing
- `ScatterPlot` does not expose bordered-frame hooks (`Border`, `Padding`, `BorderStyleText`, `FocusedBorderStyleText`) unlike most dashboard widgets.
- `LogView` exposes `Append(...)` but not `AppendLine(...)`; easy to adapt, but discoverability is weaker when moving from other log/terminal APIs.
- Theme token naming is easy to misread at first pass (`Border.Default` and `Focus.Ring` are valid; `Border.Primary` / `Focus.Primary` are not).

### Top API adjustments suggested
1. Add non-breaking frame/style parity to `ScatterPlot` (`Border`, `Padding`, border style hooks).
2. Add `LogView.AppendLine(string)` as compatibility alias to `Append(string)`.
3. Add a short token crosswalk in docs (`Border.Default/Strong/Focused`, `Focus.Ring/Title/Border`).
4. Add a compact “dashboard composition cookbook” doc page with common production layouts and control bundles.
5. Keep expanding canonical aliasing where legacy names remain (same direction as `Selected*` and title-style alias work).

## Wave 4 Follow-up - ControlPlaneOpsDashboard Hardening (March 23, 2026)

- Implemented app-level reliability policy for shortcuts: control-plane actions now use Ctrl-chords (`Ctrl+1..5`, `Ctrl+T`, `Ctrl+P`, `Ctrl+D`, `Ctrl+A`, `Ctrl+N`, `Ctrl+R`) to avoid text-entry collisions.
- Added bounded activity-log retention (ring-buffer policy) at app level to prevent unbounded growth while preserving UX.
- Quick-open list refresh is now frozen while overlay is open and refreshed when idle (close/cancel/submit path), removing open-overlay churn.
