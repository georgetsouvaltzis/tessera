---
sidebar_label: "Widgets: Data & Inspection"
---

# Widgets: Data And Inspection

Use these widgets when the surface is record-heavy, evidence-heavy, or inspection-heavy.

## Core table and record widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Table` | simple row/column readouts | lightweight tables for static or low-complexity rows | starter/admin views |
| `DataGrid` | interactive record grids | columns, selected row/cell state, sorting hooks, paging-like navigation | `DataWorkbench` |
| `TreeTable` | hierarchical tabular data | tree expansion with multiple value columns | structured record trees |
| `KeyValueList` | compact label/value inspection | dense property readouts | detail panels |
| `Timeline` | ordered events | event sequences and selection | audit/history views |
| `PivotTable` | cross-tab data | row/column pivots with explicit cell values | planning/reporting views |

## Rich content and inspection widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `MarkdownView` | markdown content | rendered markdown inside a terminal surface | docs/help panes |
| `RichTextView` | richer styled prose | mixed emphasis and styled content blocks | readme/help/detail panes |
| `JsonTreeView` | structured JSON browsing | node expansion and typed JSON tree inspection | payload inspection |
| `InspectorPanel` | collapsed/expanded detail sections | key/value fields plus detail rows | `DataWorkbench` style side panels |
| `PropertyGrid` | editable/readable property surfaces | property-by-property inspection | config and tooling UIs |
| `DiffView` | before/after comparison | side-by-side or line-oriented change review | review tooling |
| `QueryBuilder` | structured query editing | groups, rules, operators, query changes | `DataWorkbench` style workflows |

## Logs, traces, and command evidence

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `LogView` | appended log history | scrollable log reading | debugging and ops |
| `LogTailPanel` | tail-style log panels | compact streaming log tail surfaces | ops panels |
| `TraceViewer` | trace/event debugging | trace entry browsing and selection | debug/investigation tools |
| `CommandOutput` | command stdout/stderr rendering | channel-aware output lines | workflow tooling |
| `TerminalPanel` | terminal-like line feed | styled channel output in shell-like panes | command/task shells |
| `TaskRunnerPanel` | task/job execution history | task runs and selection | build/job flows |
| `ProcessListView` | process/task inventory | process rows and status inspection | advanced tooling |

## Feeds and activity widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `ActivityFeed` | business/ops activity streams | typed feed items with timestamps and kinds | `OpsWatch`, `DataWorkbench` |
| `Notifications` | lightweight visible notification stack | push items into a visible app feed | starter and shell flows |
| `NotificationInbox` | denser persistent inbox | advanced inbox workflow with read/pin state | advanced dev/ops flows |

## Related pages

- recipes: [recipes-data-and-workspaces](/docs/recipes-data-and-workspaces)
- widgets overview: [controls-overview](/docs/controls-overview)
- API inventory: [public-api-inventory](/docs/public-api-inventory)
