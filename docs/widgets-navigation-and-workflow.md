---
sidebar_label: "Widgets: Navigation & Workflow"
---

# Widgets: Navigation And Workflow

Use these widgets when the user needs to move between sections, search, execute commands, or drive a workflow-heavy surface.

## Section and route navigation

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Tabs` | small view switching | named tab strips, keyboard tab changes, pointer selection | `WorkspaceApp` |
| `Breadcrumb` | location path display | drill-down path with clickable segments | record/detail flows |
| `SideNavRail` | left-rail navigation | icons, badges, selection, activation | `WorkspaceApp`, `DataWorkbench` |
| `JumpList` | section or target jumping | fast jump targets with activation events | command and review flows |
| `Accordion` | collapsible section stacks | expand/collapse grouped content | settings and detail panels |
| `Paginator` | page movement | compact page navigation for paged results | paged tables and lists |

## Command and search widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `CommandPalette` | global command launch | searchable command execution overlay | `GitConsole` style flows |
| `QuickOpenOverlay` | fuzzy-open style jumping | search and jump into entities/files/items | workspace shells |
| `SearchBox` | inline query entry | query text, match metadata, prev/next navigation | search-heavy screens |
| `SearchResultsView` | result listing for a query | focused result browsing after search | search workflows |
| `FuzzyFinder` | standalone fuzzy selection | string or item-based fuzzy finding | advanced open/go-to flows |

## Menu and action surfaces

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `MenuBar` | application-wide menus | top-level menu items and submenu launching | shell-style apps |
| `Toolbar` | compact tool rows | visible tool items with selection changes | productivity shells |
| `CommandBar` | visible command/action rows | focused action strips and item activation | command-heavy apps |

## Lists and tree widgets

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `ListView<T>` | straightforward list browsing | simple typed item lists with selection | starters and detail panes |
| `VirtualizedListView<T>` | larger lists | reduced rendering cost for long lists | large feeds and records |
| `GroupedListView<TGroup,TItem>` | grouped lists | grouped headers + item selection | inboxes and grouped queues |
| `TreeView` | hierarchical navigation | native tree interaction with expand/collapse | settings and file-like hierarchies |
| `FileExplorer` | file/folder navigation | directory tree browsing with file items | advanced tooling flows |

## Workflow-specific boards and flows

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `KanbanBoard` | queue-and-lane workflow | lanes, cards, selection, movement-oriented boards | planning and ops queues |
| `Stepper` | short process stages | compact sequence marker | guided tasks |
| `Wizard` | longer guided task flow | step descriptions and completion state | setup and handoff flows |

## Related pages

- recipes: [recipes-data-and-workspaces](/docs/recipes-data-and-workspaces)
- showcase: [showcase](/docs/showcase)
- API inventory: [public-api-inventory](/docs/public-api-inventory)
