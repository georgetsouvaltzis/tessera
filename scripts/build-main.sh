#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

projects=(
  "src/TeaSharp.Core/TeaSharp.Core.csproj"
  "src/TeaSharp/TeaSharp.csproj"
  "tests/TeaSharp.Tests/TeaSharp.Tests.csproj"
  "tests/TeaSharp.IntegrationTests/TeaSharp.IntegrationTests.csproj"
)

for project in "${projects[@]}"; do
  dotnet build "$project" -nologo -v minimal "$@"
done
