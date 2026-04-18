---
sidebar_label: "Widgets: Shells & Overlays"
---

# Widgets: Shells, Panes And Overlays

Use these widgets when the app needs shell chrome, transient overlays, or denser pane management.

## Dialogs, overlays, and transient UI

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `Dialog` | focused modal prompts | confirmation/result flow with dialog result events | shell-style apps |
| `Modal` | framed modal content | custom modal surface hosting | denser shells |
| `ContextMenu` | pointer/selection context actions | contextual action menus with item execution | tool/workbench flows |
| `CommandPalette` | command overlay | searchable command execution | `GitConsole`-style flows |
| `QuickOpenOverlay` | jump overlay | quick entity/file open flow | workspaces |
| `ToastCenter` | transient toast notifications | short-lived toast message surfaces | advanced notification flows |

## Shell chrome and persistent utilities

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `StatusBar` | footer/system strip | left/right status text across the shell | starters and flagships |
| `Notifications` | lightweight visible feed | stacked notifications in one pane | starter and shell flows |
| `NotificationInbox` | persistent inbox | read/pin inbox workflow for denser apps | advanced dev/ops flows |
| `EmptyState` | blank-state guidance | explicit “nothing here yet” surface | onboarding and empty panels |
| `KeyBindingHelpDialog` | shortcut reference | visible keybinding help inside the UI | advanced tooling |
| `PaletteEditor` | palette/theme editing | interactive palette work and swatch selection | theme tooling |

## Pane and workspace management

| Widget | Use it for | Capabilities | Good example |
| --- | --- | --- | --- |
| `SplitView` | simple split-pane layouts | two-region split surfaces | early multi-pane shells |
| `ResizablePaneGroup` | adjustable pane layouts | draggable/resizable pane groups | workbenches |
| `DockWorkspace` | docked shell composition | dock panes around a central workspace | advanced workspaces |
| `PaneTabs` | tabbed subpanes | tab management inside one region | workspaces |
| `InspectorPanel` | structured side inspection | collapse/expand detail sections | `DataWorkbench` |

## When to choose which shell widget

- use `SplitView` for the first split
- use `ResizablePaneGroup` once the user needs control of pane sizes
- use `DockWorkspace` when the layout itself becomes a product feature
- use `PaneTabs` when one pane needs multiple subviews
- use `Dialog` or `Modal` when the user must resolve something before continuing

## Related pages

- recipes: [recipes-data-and-workspaces.md](recipes-data-and-workspaces.md)
- widgets overview: [controls-overview.md](controls-overview.md)
- architecture: [architectural-review.md](architectural-review.md)
