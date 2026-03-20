#!/usr/bin/env bash

set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$REPO_ROOT"

TERM_VALUE="${TERM:-<unset>}"
TERM_PROGRAM_VALUE="${TERM_PROGRAM:-<unset>}"
echo "HOST | TERM=${TERM_VALUE} TERM_PROGRAM=${TERM_PROGRAM_VALUE}"

FAILURES=0

run_check() {
  local name="$1"
  shift

  echo "RUN  | ${name}"
  if "$@"; then
    echo "PASS | ${name}"
  else
    echo "FAIL | ${name}"
    FAILURES=1
  fi
}

run_check \
  "detector-tests" \
  dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "CapabilityDetector_"

run_check \
  "renderer-font-gating-tests" \
  dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "Renderer_FontSpec_|Renderer_StructuredFontRequest_|Renderer_ITerm2Profile_"

run_check \
  "host-ghostty-evidence-hook" \
  dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "CapabilityDetector_HostEnvironment_Ghostty_EvidenceHook"

if [[ "$FAILURES" -eq 0 ]]; then
  echo "PASS | terminal-matrix-v1"
else
  echo "FAIL | terminal-matrix-v1"
fi

exit "$FAILURES"
