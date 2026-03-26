# ConsumerTelemetryLab

Focused consumer-style telemetry/operations app built only on public TeaSharp APIs.

## Run

```bash
dotnet run --project examples/ConsumerTelemetryLab/ConsumerTelemetryLab.csproj
```

## What It Exercises

- live telemetry updates across multiple services
- plotting composition: `PlotPanel`, `Sparkline`, `AreaPlot`, `LinePlot`, `ScatterPlot`, `Histogram`
- tabs + side panels + filters + incident queue + alert inbox + activity log
- keyboard + pointer semantics (single-click pointer activation)
- theme switching and explicit style/glyph/state overrides

## Interaction Map

- `q` or `Ctrl+C`: quit
- `t`: switch theme (Catppuccin/Rose Pine)
- `p`: pause/resume telemetry
- `l`: toggle load profile (`nominal`/`incident`)
- `n`: select next service
- `i`: request incident drilldown for current service
- `a`: mark alerts as read
- `r`: reset telemetry/incidents
- pointer: click tabs, filters, services, incident table rows, alerts
