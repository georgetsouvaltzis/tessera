#!/usr/bin/env bash
set -euo pipefail

# template: safe scaffold for TeaSharp vs competitor comparison
# TODO: replace placeholders with real commands once both apps are available locally.

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUTPUT_DIR="${ROOT_DIR}/.artifacts/perf"
mkdir -p "${OUTPUT_DIR}"

SCENARIO_ID="${SCENARIO_ID:-TODO_SCENARIO_ID}"
RUNS="${RUNS:-20}"
TERMINAL_PROFILE="${TERMINAL_PROFILE:-TODO_SAME_TERMINAL_PROFILE}"

TEASHARP_CMD="${TEASHARP_CMD:-TODO_TEASHARP_COMMAND}"
COMPETITOR_CMD="${COMPETITOR_CMD:-TODO_COMPETITOR_COMMAND}"

echo "template: TeaSharp TUI comparison scaffold"
echo "scenario=${SCENARIO_ID}"
echo "runs=${RUNS}"
echo "terminal=${TERMINAL_PROFILE}"

if [[ "${TEASHARP_CMD}" == TODO_* ]] || [[ "${COMPETITOR_CMD}" == TODO_* ]]; then
  echo "TODO: set TEASHARP_CMD and COMPETITOR_CMD before running real comparisons."
  echo "No benchmark command executed."
  exit 0
fi

echo "TODO: warmup and measured runs should use the same machine and same terminal."
echo "TODO: collect median, p95, alloc for both commands."
echo "TODO: write raw output files under ${OUTPUT_DIR}/${SCENARIO_ID}/"

# Placeholder only. Intentionally does not execute user-provided commands yet.
exit 0
