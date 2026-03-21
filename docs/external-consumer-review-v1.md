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
- Current example demonstrates navigation, list/table, notifications, dialog flow, and theme switching without `TeaSharp.Core`.
- No breaking API changes proposed in this pass.

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
