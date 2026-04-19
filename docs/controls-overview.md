---
sidebar_label: Components
---

# Widgets Overview

Tessera ships a broad widget surface. Learn it by product problem, not alphabet.

Use this page as the bridge between [Starter Examples](/docs/examples) and [Public API Inventory](/docs/public-api-inventory).

## Read the widget docs in this order

| Family | Use it when you need | Best examples | Page |
| --- | --- | --- | --- |
| Inputs & forms | data entry, validation, pickers, and model editing | `CounterForm` | [Inputs & Forms](/docs/widgets-inputs-and-forms) |
| Navigation & workflow | tabs, rails, commands, search, file/workflow movement | `WorkspaceApp`, `GitConsole` | [Navigation & Workflow](/docs/widgets-navigation-and-workflow) |
| Data & inspection | tables, logs, diffs, inspectors, traces, record-heavy surfaces | `DataWorkbench`, `GitConsole` | [Data & Inspection](/docs/widgets-data-and-inspection) |
| Dashboards, planning & plots | metrics, charts, boards, schedules, visual density | `OpsWatch`, `DataWorkbench` | [Dashboards & Plots](/docs/widgets-dashboards-and-plots) |
| Shells & overlays | dialogs, notifications, pane systems, shell chrome | `WorkspaceApp`, `DataWorkbench` | [Shells & Overlays](/docs/widgets-shells-and-overlays) |

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

If you still need the exact type name after that, open [Public API Inventory](/docs/public-api-inventory).

## Minimal sample

```csharp
using Tessera.Controls;
using Tessera.Layout;

var nav = new Tabs();
var grid = new DataGrid();
var status = new StatusBar { LeftText = "Ready" };

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body =>
    {
        body.Row(0.12f, nav);
        body.Row(0.88f, grid);
    });
});
```

This is the standard "navigation + content + status" shell that most product apps grow from.

## What this page does not replace

The Widgets section is for discovery and capability mapping. Use the reference pages when you need:

- exact public type names
- naming policy
- terminal-specific caveats
- theming hook details
- helper records, glyph sets, options, and event args

For those, use:

- [API Reference](/docs/api-reference)
- [API Guidelines](/docs/public-api-guidelines)
- [Public API Inventory](/docs/public-api-inventory)
- [Theme System](/docs/theme-system)
