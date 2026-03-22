#!/usr/bin/env zsh
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT_PATH="$ROOT_DIR/benchmarks/TeaSharp.Benchmarks/TeaSharp.Benchmarks.csproj"
BENCHMARK_DLL_PATH="$ROOT_DIR/benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/TeaSharp.Benchmarks.dll"
BASELINE_PATH="$ROOT_DIR/docs/perf-baselines/v1-slo-gate-baseline.json"
OUTPUT_PATH="$ROOT_DIR/docs/perf-baselines/latest-slo-gate-result.json"

usage() {
  cat <<'EOF'
Usage:
  scripts/perf_gate_v1.sh run
  scripts/perf_gate_v1.sh dry-run

Modes:
  run      Execute startup/input-latency gate benchmarks and compare against baseline thresholds.
  dry-run  Validate baseline contract and emit machine-readable scaffold output without running benchmarks.
EOF
}

run() {
  echo "+ $*"
  "$@"
}

MODE="${1:-run}"

if [[ "$MODE" == "help" || "$MODE" == "--help" || "$MODE" == "-h" ]]; then
  usage
  exit 0
fi

if [[ ! -f "$PROJECT_PATH" ]]; then
  echo "Benchmark project not found: $PROJECT_PATH" >&2
  exit 1
fi

if [[ ! -f "$BASELINE_PATH" ]]; then
  echo "Baseline file not found: $BASELINE_PATH" >&2
  exit 1
fi

run dotnet build "$PROJECT_PATH" --configuration Release --no-restore --nologo -v minimal --tl:off --no-dependencies

if [[ ! -f "$BENCHMARK_DLL_PATH" ]]; then
  echo "Benchmark executable not found: $BENCHMARK_DLL_PATH" >&2
  exit 1
fi

case "$MODE" in
  run)
    run dotnet "$BENCHMARK_DLL_PATH" \
      --perf-gate \
      --baseline "$BASELINE_PATH" \
      --output "$OUTPUT_PATH"
    ;;
  dry-run)
    run dotnet "$BENCHMARK_DLL_PATH" \
      --perf-gate \
      --baseline "$BASELINE_PATH" \
      --output "$OUTPUT_PATH" \
      --dry-run
    ;;
  *)
    usage
    exit 1
    ;;
esac

echo "Perf gate result: $OUTPUT_PATH"
