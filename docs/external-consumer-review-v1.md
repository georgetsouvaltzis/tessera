# External Consumer Review (V1)

Context: clean-room consumer pass by building `examples/ExternalConsumerReviewApp` using only `Tea`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`.

## Ranked Friction

### P1 - `Table` has no public selection state/events
- Issue: table supports keyboard/mouse row selection visually, but consumers cannot read selected row index/item or subscribe to selection changes.
- Repro snippet:
```csharp
var table = new Table("Name", "State");
table.SetRows(rows);
// Missing: table.SelectedIndex / table.SelectedRow / table.SelectionChanged
```
- Suggested non-breaking fix: add `SelectedRowIndex` (read-only), `TryGetSelectedRow(out IReadOnlyList<string> row)`, and `SelectionChanged` event. Keep existing behavior; just expose state.

### P1 - Runtime theme switch requires per-control boilerplate
- Issue: switching between Catppuccin/Rosé Pine requires manually re-applying theme to every control instance.
- Repro snippet:
```csharp
void ApplyTheme(TeaTheme theme)
{
    _tabs.ApplyTheme(theme);
    _table.ApplyTheme(theme);
    _list.ApplyTheme(theme);
    _notifications.ApplyTheme(theme);
    _dialog.ApplyTheme(theme);
    _status.ApplyTheme(theme);
}
```
- Suggested non-breaking fix: add helper on app surface (for example `ThemeScope.Apply(theme, params Control[] controls)`), or provide a documented aggregate helper for common dashboard control sets.
- Current V1 state: `TeaThemeOverrideBundle` helps local override composition, but there is still no first-party global fan-out helper equivalent to `ThemeScope`.

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

#### P1 - `Table` has no public selection state/events
- Status: open (unchanged).
- Impact: still cannot synchronize table row selection with quick-open or side-nav actions.
- Repro snippet:
```csharp
var table = new Table("Name", "State");
table.SetRows(rows);
// Missing: table.SelectedIndex / table.SelectedRow / table.SelectionChanged
```

#### P2 - Runtime theme switch still needs explicit per-control fan-out
- Status: open (partially improved by extension coverage and `TeaThemeOverrideBundle`, but still app-level boilerplate).
- Repro snippet:
```csharp
void ApplyTheme(TeaTheme theme)
{
    _dashboardRail.ApplyTheme(theme);
    _dashboardGrid.ApplyTheme(theme);
    _healthBoard.ApplyTheme(theme);
    _distributionPlot.ApplyTheme(theme);
    // ...repeat for each control instance
}
```
- Notes: a `ThemeScope`-style helper is still absent from the public API surface in this lane.

#### P2 - `ListView<T>` lacks programmatic selection setter
- Status: open.
- Impact: external quick-open command can set domain selection state, but cannot drive visual `ListView<T>` row selection through a public setter.
- Repro snippet:
```csharp
var list = new ListView<ServiceHealth>(item => item.Name);
list.SetItems(services);
// Missing: list.SetSelectedIndex(index) or list.Select(index)
```

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
