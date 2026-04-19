---
sidebar_label: "Widgets: Dashboards & Plots"
---

# Widgets: Dashboards, Planning And Plots

Use these widgets when the surface is metric-heavy, chart-heavy, or built around visual density.

## Compact metrics and status

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `StatsCard` | small grouped metrics | keyed metric/value blocks inside one card | `OpsWatch` |
| `Gauge` | bounded status readings | compact gauge-style state display | dashboards |
| `ProgressBar` | single progress dimension | bounded 0..1 progress with label/title | workflows and transfers |
| `Badge` | small semantic indicators | tone-driven state chips and counts | rails and cards |
| `MiniLog` | tiny embedded log strip | compact recent-event text | dashboards and sidebars |
| `HealthBoard` | service/status health view | service health rows, glyphs, ack-style semantics | `OpsWatch` |

## Chart and plot widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Sparkline` | tiny trend lines | compact historical trend hints | cards and dense panels |
| `TelemetryChart` | rolling telemetry traces | compact streaming telemetry with render modes | `OpsWatch` |
| `LineChart` | simple line series | straightforward trend plotting | dashboards |
| `AreaPlot` | filled trend history | area-under-line visual emphasis | monitoring panels |
| `ScatterPlot` | point clusters | point-distribution analysis | analytics views |
| `BoxPlot` | distribution summaries | quartiles/spread/outlier-style summaries | analytics views |
| `Histogram` | bucketed distributions | counts across numeric ranges | analytics views |
| `LinePlot` | denser plotting surface | richer line plotting with options/render modes | analytics/workbench surfaces |
| `PlotPanel` | framed plot host | plotting container with panel affordances | advanced dashboard panels |
| `BulletChart` | target versus actual | compact performance-against-goal view | scorecards |
| `BarChart` | category comparison | per-category bar values | category metrics |
| `Heatmap` | cell intensity grids | dense two-dimensional signal mapping | `DataWorkbench` |
| `TreeMapChart` | weighted hierarchical areas | nested weighted rectangles | portfolio/usage views |

## Dashboard and planning layout widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `DashboardGrid` | tile-based dashboards | explicit dashboard tile arrangement | dashboards |
| `CalendarMonthView` | month calendars | date-grid viewing and date selection | planning screens |
| `SchedulerTimeline` | schedule/timeline planning | time-ordered entries and selection | planning/ops scheduling |
| `KanbanBoard` | lane-based flow tracking | cards grouped by lane/state | workflow boards |

## Related pages

- showcase: [showcase](/docs/showcase)
- recipes: [recipes-effects-and-refresh](/docs/recipes-effects-and-refresh)
- API inventory: [public-api-inventory](/docs/public-api-inventory)
