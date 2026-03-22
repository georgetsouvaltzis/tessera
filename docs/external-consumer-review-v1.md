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

#### P1 - `LineSeries.Append(...)` retention is still manual for streaming dashboards
- Status: open (unchanged).
- Repro snippet:
```csharp
var series = new LineSeries("Req/s");
series.Append(nextValue); // unbounded growth
// Consumer still needs manual trim + SetSamples(...)
```

#### P1 - `LinePlot` still lacks mixed-unit scale controls
- Status: open (unchanged).
- Repro snippet:
```csharp
plot.SetSeries([
    new LineSeries("Req/s", reqSeries),
    new LineSeries("P95 ms", latencySeries),
]);
// Missing: per-series scale mode or secondary Y-axis
```

#### P2 - Runtime theme switch still needs explicit per-control fan-out
- Status: open (partially improved by extension coverage, but still app-level boilerplate).
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

#### P2 - `ListView<T>` lacks programmatic selection setter
- Status: open.
- Impact: external quick-open command can set domain selection state, but cannot drive visual `ListView<T>` row selection through a public setter.
- Repro snippet:
```csharp
var list = new ListView<ServiceHealth>(item => item.Name);
list.SetItems(services);
// Missing: list.SetSelectedIndex(index) or list.Select(index)
```

## Wave 2 - Advanced Dashboard (Styling/State/Plot Readiness)

Context: implemented an `Analytics` screen in `examples/ExternalConsumerReviewApp` with status cards, endpoint table, and `LinePlot`.

### P1 - `StatsCard` has no border/padding style hooks
- Issue: `StatsCard` always draws default box chrome, so dashboards cannot align card borders with theme border tokens used by other controls.
- Repro snippet:
```csharp
var card = new StatsCard { Title = "Latency" };
card.ApplyTheme(theme);
// Missing: card.Border, card.BorderStyleText, card.FocusedBorderStyleText, card.Padding
```
- Suggested non-breaking fix: add `Border`, `Padding`, `BorderStyleText`, and `FocusedBorderStyleText` to `StatsCard`.
- Workaround used: custom title/key/value styles only; card frame remains default glyph styling.

### P1 - Plot streaming requires manual sample trimming
- Issue: `LineSeries.Append(...)` is unbounded and there is no capacity/retention API on `LineSeries` or `LinePlot`.
- Repro snippet:
```csharp
var series = new LineSeries("Req/s");
series.Append(nextValue); // keeps growing forever
// Consumer must copy+trim and call SetSamples(...)
```
- Suggested non-breaking fix: add `Capacity` + automatic trimming, or `TrimToLast(int count)` helpers.
- Workaround used: manual retention helper rebuilding `double[]` and calling `SetSamples(...)`.

### P1 - `LinePlot` lacks dual-axis or per-series scale controls
- Issue: realistic dashboards often mix units (for example req/s and ms). Current rendering uses one shared Y scale, making one series visually flatten the other.
- Repro snippet:
```csharp
plot.SetSeries([
    new LineSeries("Req/s", reqSeries),
    new LineSeries("P95 ms", latencySeries),
]);
// Missing: secondary axis or per-series normalization mode
```
- Suggested non-breaking fix: add optional per-series scale mode (`Shared`, `Normalized`, `SecondaryAxis`) and label hooks for each axis.
- Workaround used: accept reduced readability or split metrics into separate plots.

### P2 - `Options` naming is generic for advanced plot features
- Issue: enabling axes/grid/legend depends on `LinePlot.Options` (`LinePlotOptions`), but `Options` is ambiguous and marked advanced, reducing discoverability for consumers building first dashboards.
- Repro snippet:
```csharp
var plot = new LinePlot();
plot.Options = new LinePlotOptions(ShowAxes: true, ShowGrid: true);
```
- Suggested non-breaking fix: keep `Options` but add verb-style helpers (`ConfigureAxes`, `ConfigureLegend`, `ConfigureGrid`) for common setup.
