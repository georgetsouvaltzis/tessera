# PlottingDashboard

Production-style telemetry dashboard example built with TeaSharp plotting controls.

## Widgets
- `Sparkline` for CPU percent trend
- `AreaPlot` for memory trend
- `LinePlot` for p50/p95/p99 latency timeline
- `ScatterPlot` for latency jitter distribution over time
- `Histogram` for error distribution
- `PlotPanel` for multi-panel composition

## Controls
- `q` or `Ctrl+C`: quit
- `t`: toggle theme (`catppuccin` / `rose-pine`)
- `m`: toggle data mode (`smooth` / `bursty`)
- `p`: pause/resume data updates
- `r`: reset all telemetry streams

## Run
```bash
dotnet run --project examples/PlottingDashboard
```
