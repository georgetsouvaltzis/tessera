---
title: Screens and Layout
---

Tessera favors explicit screen assembly over nested mini-DSLs.

## Default tools

- `Screen.Build(...)`
- `WindowBuilder`
- `RowLayout`
- `ColumnLayout`
- `CenterLayout`
- `PanelLayout`

## Guideline

Prefer shallow, named regions:

- header
- left pane
- body
- footer

That keeps terminal screens readable in code and easier to evolve.
