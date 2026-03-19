# TeaSharp Performance Plan V1

This plan defines V1 performance goals, measurement methodology, and release gates.

## Goals and SLO Targets

Primary metrics:
- startup time to first rendered frame
- frame time under workload
- allocations per rendered frame
- p95 input latency

SLO targets (Release build, local terminal, same machine profile per run):
- Startup (HelloWorld): <= 120 ms to first frame
- Startup (WorkspaceApp): <= 250 ms to first frame
- Frame time p95 (normal UI load): <= 16 ms
- Frame time p95 (heavy styled output): <= 33 ms
- Allocations/frame (normal UI load): <= 32 KB
- Allocations/frame (heavy styled output): <= 96 KB
- Input latency p95 (normal UI load): <= 12 ms
- Input latency p95 (heavy load): <= 25 ms

## Scenario Matrix

Required benchmark scenarios:
1. Static dashboard:
- low churn, mixed controls
- validates baseline frame cost

2. Log tail stream:
- high append rate + scrolling
- validates sustained render/update behavior

3. Large table:
- wide + tall dataset with selection movement
- validates list/table throughput

4. Overlay stress:
- command palette/context overlays on active base layout
- validates layered composition and focus transitions

5. Resize storm:
- repeated terminal size changes
- validates recomposition and runtime stability

6. Styled heavy output:
- frequent style/state changes across many cells
- validates style diffing/render overhead

Supplemental benchmark coverage (not part of the six-scenario gate checklist):
- viewport no-decoration render loop (`LogView`)
- validates hot-path viewport rendering with and without final materialization

## Harness Approach

Two-layer harness:
- Micro + component performance:
  - BenchmarkDotNet scenarios for hot paths and control rendering loops
  - deterministic inputs, fixed terminal sizes
- End-to-end smoke metrics:
  - integration runs with timed startup/frame/input probes
  - scenario scripts with stable seeds and fixed event sequences

Measurement rules:
- Release configuration only
- benchmark project enables Release-only `AllowUnsafeBlocks` for BenchmarkDotNet-generated harness compatibility
- same terminal profile and dimensions per comparison
- warmup included before recorded samples
- minimum 10 measured iterations per scenario

Benchmark modes:
- render-only mode:
  - measures control/layout/style work up to render calls
  - excludes string materialization (`canvas.Render()`)
- render+materialize mode:
  - includes final buffer/string materialization (`canvas.Render()`)
  - captures end-to-end frame cost and allocation impact seen by app authors

Gating policy by mode:
- render-only is the regression gate for renderer/layout internals (hot-path control cost)
- render+materialize is the release-facing gate for frame/allocation budgets
- Public V1 perf gate requires both mode families to remain within regression budget

Harness quick commands:
- List benchmarks:
  - `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --list flat`
- Run all benchmarks in Release:
  - `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*"`
- Run a single benchmark scenario filter:
  - `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*LargeTable*"`
- Mode-specific examples (dual-mode instrumentation):
  - render-only slice: `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Only"`
  - render+materialize slice: `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Frame" --filter "*ScrollLogTail" --filter "*FirstFrameRender" --filter "*OverlayStressFrames" --filter "*ResizeStormFrames"`
  - viewport slice: `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Viewport*"`
- Future V1 gate scenario filters (when benchmark classes are present):
  - `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*Resize*"`
  - `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*Overlay*"`
  - `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*LogTail*"`
- Scripted path (optional):
  - `scripts/run_benchmarks_v1.sh list|all|scenario "<filter>"|shortlist`
  - `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - `scripts/run_benchmarks_v1.sh iteration-template`
  - script execution modes (`all|scenario|shortlist*`) run with `--inProcess` for trend/gate stability
  - script performs lazy build (build only when benchmark output is missing)

BenchmarkDotNet artifacts/report directory:
- `benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/BenchmarkDotNet.Artifacts/`

Expected `--list flat` scenarios:
- `TeaSharp.Benchmarks.StartupRenderBenchmarks.StartupLikeFirstFrameRender`
- `TeaSharp.Benchmarks.StartupRenderBenchmarks.StartupLikeFirstFrameRenderOnly`
- `TeaSharp.Benchmarks.LogTailStreamBenchmarks.AppendAndScrollLogTail`
- `TeaSharp.Benchmarks.LogTailStreamBenchmarks.AppendAndScrollLogTailOnly`
- `TeaSharp.Benchmarks.LargeTableBenchmarks.RenderLargeTableFrame`
- `TeaSharp.Benchmarks.LargeTableBenchmarks.RenderLargeTableFrameOnly`
- `TeaSharp.Benchmarks.OverlayStressBenchmarks.RenderOverlayStressFrames`
- `TeaSharp.Benchmarks.OverlayStressBenchmarks.RenderOverlayStressFramesOnly`
- `TeaSharp.Benchmarks.ResizeStormBenchmarks.RenderResizeStormFrames`
- `TeaSharp.Benchmarks.ResizeStormBenchmarks.RenderResizeStormFramesOnly`
- `TeaSharp.Benchmarks.StyledHeavyOutputBenchmarks.RenderStyledHeavyFrame`
- `TeaSharp.Benchmarks.StyledHeavyOutputBenchmarks.RenderStyledHeavyFrameOnly`
- `TeaSharp.Benchmarks.ViewportRenderBenchmarks.RenderViewportNoDecoration`
- `TeaSharp.Benchmarks.ViewportRenderBenchmarks.RenderViewportNoDecorationOnly`

## Comparison Protocol vs Other TUIs

Comparison is methodology-first, not marketing-first:
- compare identical scenario definitions
- same machine, same terminal, same resolution
- same runtime mode (release/optimized)
- report median, p95, and memory/alloc deltas
- include caveats for non-equivalent feature sets

Fairness rules:
- do not compare feature-rich scenario against minimal baseline scenario
- publish benchmark scripts and raw output
- avoid cherry-picked single-run numbers

## Regression Budget and Release Gate

Regression budget (relative to last accepted baseline):
- startup regression > 10% -> fail gate
- frame time p95 regression > 10% -> fail gate
- allocations/frame regression > 15% -> fail gate
- input latency p95 regression > 10% -> fail gate

Release gate for Public V1:
- all required scenarios executed
- no metric exceeds regression budget
- SLO targets met or variance explained with approved mitigation plan
- perf report attached to release checklist

## Iteration Reporting (Before/After)

Purpose:
- capture one comparable before/after pair for the same commit range, machine, terminal, and runtime config
- make gate decisions explicit by mode (`render-only` and `render+materialize`)

Workflow:
1. run shortlist in the selected mode on baseline commit (before)
2. run shortlist in the same mode on candidate commit (after)
3. append one iteration row per scenario with both modes in the same row
4. mark gate result (`pass`/`fail`) with short reason

Minimum report fields:
- date (UTC), before commit, after commit
- scenario name, render-only before/after mean+alloc, materialize before/after mean+alloc
- mean/alloc delta % for each mode
- gating result
