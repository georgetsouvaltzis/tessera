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
- same terminal profile and dimensions per comparison
- warmup included before recorded samples
- minimum 10 measured iterations per scenario

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
