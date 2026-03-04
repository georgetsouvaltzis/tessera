# TeaSharp Components

TeaSharp now includes a lightweight component drawing API in `TeaSharp.Components` for deterministic terminal UI composition.

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
  - `Render` (returns full frame string)
- `Widgets`:
  - `DrawPanel`
  - `DrawProgressBar`
  - `DrawSparkline`
  - `DrawList`
  - `DrawCard`
  - `DrawTable`

## Example Integration

`TeaSharp.Examples` now has a dashboard page (press `2`) that renders:

- system status panel
- live progress bar
- sparkline chart
- component summary card
- action/state table
- event footer

The protocol probe page remains available (press `1`) for low-level VT debugging.
The workspace now composes these components with stateful models from `TeaSharp.Widgets`.
