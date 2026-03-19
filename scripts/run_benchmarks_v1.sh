#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_PATH="$ROOT_DIR/benchmarks/TeaSharp.Benchmarks/TeaSharp.Benchmarks.csproj"
CONFIGURATION="Release"
OUTPUT_DLL="$ROOT_DIR/benchmarks/TeaSharp.Benchmarks/bin/$CONFIGURATION/net10.0/TeaSharp.Benchmarks.dll"

usage() {
  cat <<'EOF'
Usage:
  scripts/run_benchmarks_v1.sh list
  scripts/run_benchmarks_v1.sh all
  scripts/run_benchmarks_v1.sh scenario "<filter>"
  scripts/run_benchmarks_v1.sh shortlist
  scripts/run_benchmarks_v1.sh shortlist-render-only
  scripts/run_benchmarks_v1.sh shortlist-materialize
  scripts/run_benchmarks_v1.sh iteration-template

Modes:
  list      List benchmarks (--list flat)
  all       Run all benchmarks in inProcess mode (--filter "*")
  scenario  Run a single filter pattern
  shortlist Run V1-oriented filter shortlist:
            Resize, Overlay, LogTail, Startup, LargeTable, StyledHeavy
  shortlist-render-only   Run six render-only methods (current "*Only" suffix)
  shortlist-materialize   Run six materialize methods (current non-"Only" names)
  iteration-template      Print compact before/after report template
EOF
}

run() {
  echo "+ $*"
  "$@"
}

run_benchmark() {
  run dotnet run --project "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-build -- --inProcess "$@"
}

ensure_build_if_missing() {
  if [[ -f "$OUTPUT_DLL" ]]; then
    return
  fi

  run dotnet build "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-restore --nologo -v minimal
}

if [[ ! -f "$PROJECT_PATH" ]]; then
  echo "Benchmark project not found: $PROJECT_PATH" >&2
  exit 1
fi

MODE="${1:-all}"

if [[ "$MODE" == "help" || "$MODE" == "--help" || "$MODE" == "-h" ]]; then
  usage
  exit 0
fi

if [[ "$MODE" == "iteration-template" ]]; then
  cat <<'EOF'
Date (UTC):
Before commit:
After commit:
Host/terminal note:

| Scenario | Render-only (before -> after) | Render-only alloc (before -> after) | Materialize (before -> after) | Materialize alloc (before -> after) | RO mean delta % | MAT mean delta % | Gate |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Startup | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| LogTail | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| LargeTable | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| OverlayStress | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| ResizeStorm | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| StyledHeavy | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |

Result summary:
EOF
  exit 0
fi

run_render_only_shortlist() {
  local filters=(
    "*StartupLikeFirstFrameRenderOnly"
    "*AppendAndScrollLogTailOnly"
    "*RenderLargeTableFrameOnly"
    "*RenderOverlayStressFramesOnly"
    "*RenderResizeStormFramesOnly"
    "*RenderStyledHeavyFrameOnly"
  )

  for filter in "${filters[@]}"; do
    run_benchmark --filter "$filter"
  done
}

run_materialize_shortlist() {
  local filters=(
    "*StartupLikeFirstFrameRender"
    "*AppendAndScrollLogTail"
    "*RenderLargeTableFrame"
    "*RenderOverlayStressFrames"
    "*RenderResizeStormFrames"
    "*RenderStyledHeavyFrame"
  )

  for filter in "${filters[@]}"; do
    run_benchmark --filter "$filter"
  done
}

case "$MODE" in
  list)
    ensure_build_if_missing
    run dotnet run --project "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-build -- --list flat
    ;;
  all)
    ensure_build_if_missing
    run_benchmark --filter "*"
    ;;
  scenario)
    if [[ $# -lt 2 ]]; then
      echo "Missing filter argument for scenario mode." >&2
      usage
      exit 1
    fi

    ensure_build_if_missing
    run_benchmark --filter "$2"
    ;;
  shortlist)
    ensure_build_if_missing
    FILTERS=("*Resize*" "*Overlay*" "*LogTail*" "*Startup*" "*LargeTable*" "*StyledHeavy*")
    for filter in "${FILTERS[@]}"; do
      run_benchmark --filter "$filter"
    done
    ;;
  shortlist-render-only)
    ensure_build_if_missing
    run_render_only_shortlist
    ;;
  shortlist-materialize)
    ensure_build_if_missing
    run_materialize_shortlist
    ;;
  *)
    usage
    exit 1
    ;;
esac
