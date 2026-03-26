# ConsumerTelemetryLab Friction Notes

## Pre-RC follow-up: DataForm keyed selection

- New additive public API available: `DataForm<T>.SelectField(string key)`.
- Relevance to this app: indirect only (this app currently uses list/table drilldowns, not `DataForm`).
- Outcome: form-focused consumers can now select fields by stable key without index coupling.

## Resolved in this slice

### Incident drilldown table sync via selection API

- Previous workaround removed:
  - no `_incidentSortAnchorId`
  - no row reordering/anchor-to-top behavior
- Current behavior:
  - incident rows remain sorted by state/severity/age
  - drilldown sync calls `Table.SetSelectedIndex(...)` after `SetRows(...)`
- Evidence:
  - `ConsumerTelemetryLabApp.Data.cs` (`RefreshListsAndTables`, `SortedIncidentsForTable`, `RequestIncidentDrilldown`)

Additive API candidates:

- `Table.SetSelectedKey(string key)` with key selector supplied at row binding time
- `Table.TryEnsureVisible(int index)` to keep selected row in viewport

## Still open friction

### 1) Selection sync depends on row text conventions

- Current workaround:
  - On table selection event, parse first column as id/name and map back to domain model.
- Evidence:
  - `ConsumerTelemetryLabApp.cs` (`_serviceTable.SelectionChanged`, `_incidentTable.SelectionChanged`)
- Why this exists:
  - Selection event payload is row cells; no typed identity payload.

Additive API candidates:

- Generic typed table variant, e.g. `Table<T>`
- Selection event payload carrying `RowIndex` and optional user key/object

### 2) Plot/dashboard styling parity across controls is uneven

- Current app impact:
  - `ScatterPlot` and `Histogram` are composed cleanly in `PlotPanel`, but style/frame knobs are not symmetric with some other controls.
- Why this matters:
  - For dashboard composition, consistent per-widget framing/styling options reduce theme glue code and visual mismatch.

Additive API candidates:

- Unified style/frame contract for all plot widgets (title, border/frame/padding, focus marker parity)
- Shared `PlotWidgetStyle` or equivalent applied per plot instance
