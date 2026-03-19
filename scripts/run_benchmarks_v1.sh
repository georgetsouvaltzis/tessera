#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_PATH="$ROOT_DIR/benchmarks/TeaSharp.Benchmarks/TeaSharp.Benchmarks.csproj"
CONFIGURATION="Release"

usage() {
  cat <<'EOF'
Usage:
  scripts/run_benchmarks_v1.sh list
  scripts/run_benchmarks_v1.sh all
  scripts/run_benchmarks_v1.sh scenario "<filter>"
  scripts/run_benchmarks_v1.sh shortlist

Modes:
  list      List benchmarks (--list flat)
  all       Run all benchmarks (--filter "*")
  scenario  Run a single filter pattern
  shortlist Run V1-oriented filter shortlist:
            Resize, Overlay, LogTail, Startup, LargeTable, StyledHeavy
EOF
}

run() {
  echo "+ $*"
  "$@"
}

if [[ ! -f "$PROJECT_PATH" ]]; then
  echo "Benchmark project not found: $PROJECT_PATH" >&2
  exit 1
fi

MODE="${1:-all}"
run dotnet build "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-restore --nologo -v minimal

case "$MODE" in
  list)
    run dotnet run --project "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-build -- --list flat
    ;;
  all)
    run dotnet run --project "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-build -- --filter "*"
    ;;
  scenario)
    if [[ $# -lt 2 ]]; then
      echo "Missing filter argument for scenario mode." >&2
      usage
      exit 1
    fi

    run dotnet run --project "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-build -- --filter "$2"
    ;;
  shortlist)
    FILTERS=("*Resize*" "*Overlay*" "*LogTail*" "*Startup*" "*LargeTable*" "*StyledHeavy*")
    for filter in "${FILTERS[@]}"; do
      run dotnet run --project "$PROJECT_PATH" --configuration "$CONFIGURATION" --no-build -- --filter "$filter"
    done
    ;;
  *)
    usage
    exit 1
    ;;
esac
