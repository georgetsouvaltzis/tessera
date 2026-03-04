# TeaSharp Components

TeaSharp includes a deterministic component drawing layer in `TeaSharp.Components` for building Bubble Tea-style terminal layouts.

## Motivation

The design follows patterns used in Bubble Tea examples:

- `examples/cellbuffer/main.go`: fixed-size cell buffer drawing.
- `examples/canvas/main.go`: compositional layering/card layout.
- `examples/mouse/main.go`: event-driven interaction over rendered content.

## API

- `Rect`: immutable geometry helper with `Inset` and `Intersect`.
- `Canvas`: fixed-size character grid renderer.
  - `Set`, `Get`, `WriteText`
  - `DrawHorizontalLine`, `DrawVerticalLine`, `DrawBox`
  - text modes:
    - `CanvasTextMode.Fast` (default): char-cell fast path
    - `CanvasTextMode.GraphemeAware`: wide/combining grapheme-aware text placement
  - `Render` (returns full frame string)
- `Composition`:
  - `ICanvasComponent`: render-only component contract.
  - `IStatefulComponent`: component with `Update(IMessage)` for model-local state.
  - `ComponentComposer`: slot-based composition (`Add`, `Clear`, `Update`, `Render`).
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
  - `LineChartComponent` (bounded sample history)
  - `BarChartComponent` (named value bars)
- dashboard-oriented components:
  - `GaugeComponent`
  - `StatsCardComponent` + `StatsCardItem`
  - `MiniLogComponent`

## Example Integration

`TeaSharp.Examples` now has a dashboard page (press `2`) that renders:

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
The dashboard composes chart components through `ComponentComposer` and uses stateful models from `TeaSharp.Widgets`.

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

If the component owns local state and needs messages, implement `IStatefulComponent` and route messages through `ComponentComposer.Update(message)`.

For a fuller guide with a custom component walkthrough, see `docs/custom-components.md`.
