#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

projects=(
  "examples/AdvancedWidgets/AdvancedWidgets.csproj"
  "examples/ComboBox/ComboBox.csproj"
  "examples/Dropdown/Dropdown.csproj"
  "examples/Showcase/Showcase.csproj"
  "examples/Kanban/Kanban.csproj"
  "examples/ProductivityWidgets/ProductivityWidgets.csproj"
  "examples/WidgetGallery/WidgetGallery.csproj"
)

for project in "${projects[@]}"; do
  dotnet build "$project" -nologo -v minimal "$@"
done
