# ConsumerTelemetryLab Friction Notes

## 1) Incident table drilldown cannot hard-select a row by API

- Current workaround in app:
  - Store requested incident id as `_incidentSortAnchorId`.
  - Reorder incident rows so the anchor incident is placed at row `0`.
  - Prompt user to click row for hard selection.
- Evidence:
  - `ConsumerTelemetryLabApp.Data.cs` (`VisibleIncidentsForTable`, `RequestIncidentDrilldown`)
- Why this exists:
  - Public `Table` API in current surface does not expose programmatic selected-row setter by index/key.

Additive API candidates:

- `Table.SetSelectedIndex(int index)`
- `Table.SetSelectedKey(string key)` with key selector supplied at row binding time
- `Table.TryEnsureVisible(int index)` to keep selected row in viewport

## 2) Selection sync depends on row text conventions

- Current workaround in app:
  - On table selection event, parse first column as id/name and map back to domain model.
- Evidence:
  - `ConsumerTelemetryLabApp.cs` (`_serviceTable.SelectionChanged`, `_incidentTable.SelectionChanged`)
- Why this exists:
  - Selection event payload is row cells; no typed identity payload.

Additive API candidates:

- Generic typed table variant, e.g. `Table<T>`
- Selection event payload carrying `RowIndex` and optional user key/object

## 3) Plot/dashboard styling parity across controls is uneven

- Current app impact:
  - `ScatterPlot` and `Histogram` are composed cleanly in `PlotPanel`, but style/frame knobs are not symmetric with some other controls.
- Why this matters:
  - For dashboard composition, consistent per-widget framing/styling options reduce theme glue code and visual mismatch.

Additive API candidates:

- Unified style/frame contract for all plot widgets (title, border/frame/padding, focus marker parity)
- Shared `PlotWidgetStyle` or equivalent applied per plot instance
