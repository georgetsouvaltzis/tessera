---
sidebar_label: Components
---

# Widgets Overview

Tessera ships a broad widget surface. End-users should learn it by problem domain, not by one giant alphabetical dump.

Use this page as the bridge between the starter examples and the full [public-api-inventory.md](public-api-inventory.md).

## Read the widget docs in this order

| Family | Use it when you need | Best examples | Page |
| --- | --- | --- | --- |
| Inputs & forms | data entry, validation, pickers, and model editing | `CounterForm` | [widgets-inputs-and-forms.md](widgets-inputs-and-forms.md) |
| Navigation & workflow | tabs, rails, commands, search, file/workflow movement | `WorkspaceApp`, `GitConsole` | [widgets-navigation-and-workflow.md](widgets-navigation-and-workflow.md) |
| Data & inspection | tables, logs, diffs, inspectors, traces, record-heavy surfaces | `DataWorkbench`, `GitConsole` | [widgets-data-and-inspection.md](widgets-data-and-inspection.md) |
| Dashboards, planning & plots | metrics, charts, boards, schedules, visual density | `OpsWatch`, `DataWorkbench` | [widgets-dashboards-and-plots.md](widgets-dashboards-and-plots.md) |
| Shells & overlays | dialogs, notifications, pane systems, shell chrome | `WorkspaceApp`, `DataWorkbench` | [widgets-shells-and-overlays.md](widgets-shells-and-overlays.md) |

## Start with the widgets you will actually use first

For most product apps, learn the widget surface in roughly this order:

1. inputs and forms
2. navigation and workflow
3. data and inspection
4. dashboards and planning surfaces
5. shells, panes, and overlays

## How to choose a control

Ask these questions in order:

1. Is the user editing, navigating, or inspecting?
2. Is the surface single-pane or multi-pane?
3. Does the control need to be compact, data-dense, or high-visibility?
4. Will it need theme overrides or state styling?

If you still need the exact type name after that, open [public-api-inventory.md](public-api-inventory.md).

## What this page does not replace

The Widgets section is for discovery and capability mapping. Use the reference pages when you need:

- exact public type names
- naming policy
- terminal-specific caveats
- theming hook details
- helper records, glyph sets, options, and event args

For those, use:

- [api-reference.mdx](api-reference.mdx)
- [public-api-guidelines.md](public-api-guidelines.md)
- [public-api-inventory.md](public-api-inventory.md)
- [theme-system.md](theme-system.md)
