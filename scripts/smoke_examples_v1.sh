#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BOUNDED_SECONDS="${1:-4}"

usage() {
  cat <<'EOF'
Usage:
  scripts/smoke_examples_v1.sh [bounded-seconds]

Behavior:
  Runs canonical examples with a bounded startup window.
  If process is still alive at timeout, it is treated as startup success and terminated.
  If process exits before timeout:
    - exit code 0 => treated as success
    - non-zero exit => treated as failure
EOF
}

if [[ "${BOUNDED_SECONDS}" == "--help" || "${BOUNDED_SECONDS}" == "-h" ]]; then
  usage
  exit 0
fi

if ! [[ "${BOUNDED_SECONDS}" =~ ^[0-9]+$ ]]; then
  echo "bounded-seconds must be an integer, got: ${BOUNDED_SECONDS}" >&2
  exit 1
fi

EXAMPLES=(
  "DataWorkbench:examples/DataWorkbench/DataWorkbench.csproj"
  "OpsWatch:examples/OpsWatch/OpsWatch.csproj"
  "GitConsole:examples/GitConsole/GitConsole.csproj"
)

ARTIFACT_DIR="${ROOT_DIR}/.artifacts/smoke_examples_v1"
mkdir -p "${ARTIFACT_DIR}"

PASS_COUNT=0
FAIL_COUNT=0

run_example() {
  local name="$1"
  local project="$2"
  local log_file
  local pid
  local exit_code

  log_file="${ARTIFACT_DIR}/teasharp_${name}_smoke.$$.${RANDOM}.log"

  echo "+ dotnet run --project ${project} --no-build  (bounded ${BOUNDED_SECONDS}s)"
  (
    cd "${ROOT_DIR}"
    dotnet run --project "${project}" --no-build
  ) >"${log_file}" 2>&1 &
  pid=$!

  sleep "${BOUNDED_SECONDS}"

  if kill -0 "${pid}" 2>/dev/null; then
    kill "${pid}" 2>/dev/null || true
    wait "${pid}" 2>/dev/null || true
    echo "PASS ${name} startup alive >=${BOUNDED_SECONDS}s (terminated intentionally) log=${log_file}"
    rm -f "${log_file}"
    PASS_COUNT=$((PASS_COUNT + 1))
    return 0
  fi

  set +e
  wait "${pid}"
  exit_code=$?
  set -e

  if [[ "${exit_code}" -eq 0 ]]; then
    echo "PASS ${name} exited early with code 0 log=${log_file}"
    rm -f "${log_file}"
    PASS_COUNT=$((PASS_COUNT + 1))
    return 0
  fi

  echo "FAIL ${name} exited early with code ${exit_code} log=${log_file}" >&2
  tail -n 20 "${log_file}" >&2 || true
  FAIL_COUNT=$((FAIL_COUNT + 1))
  return 1
}

for entry in "${EXAMPLES[@]}"; do
  IFS=":" read -r name project <<<"${entry}"
  run_example "${name}" "${project}" || true
done

echo "SUMMARY pass=${PASS_COUNT} fail=${FAIL_COUNT}"

if [[ "${FAIL_COUNT}" -ne 0 ]]; then
  exit 1
fi
