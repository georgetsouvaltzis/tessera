# M4 API Cleanup Candidates

Scope: repo-evidence audit for Public V1 cleanup (naming clarity, ergonomics, XML docs coverage, onboarding surface clarity).

## Candidate Issues

| # | Symbol/Path | Problem | Non-breaking fix option | Breaking fix option | Priority |
|---|---|---|---|---|---|
| 1 | `docs/public-api-inventory.md`, `src/TeaSharp/Controls/EmptyState.cs`, `src/TeaSharp/Controls/ValidationSummary.cs` | Inventory omits shipped controls (`EmptyState`, `ValidationSummary`), causing source-of-truth drift. | Add missing entries and add a docs parity check. | None needed. | P0 |
| 2 | `src/TeaSharp/Controls/Notifications.cs`, `src/TeaSharp/Controls/NotificationInbox.cs` | Two overlapping notification widgets with divergent APIs (`Push` vs `Add`, different models/features). | Declare one primary in docs, add adapters, mark alternate as advanced. | Remove one API surface and migrate callers to the retained control. | P0 |
| 3 | `src/TeaSharp/Controls/SearchResultsView.Rendering.cs:64` | Focused border style behavior differs from most controls (replace vs merge). | Add optional compatibility switch, default to current behavior short-term. | Standardize to merge semantics across all bordered controls. | P0 |
| 4 | `src/TeaSharp/Styles/TeaThemeControlExtensions.DevOpsAndWorkflows.cs:7` (and similar extension files) | Large parts of public theme extension surface have no XML docs. | Add XML docs (`summary/param/returns`) across missing extension methods. | None needed. | P0 |
| 5 | `src/TeaSharp/Styles/TeaStyle.cs` | Core styling API has sparse XML docs for public members and methods. | Add full XML docs for public surface and reset/merge semantics. | None needed. | P0 |
| 6 | `src/TeaSharp/Controls/NotificationInbox.cs:25`, `src/TeaSharp/Controls/Notifications.cs:23` | High-public-surface controls with low XML coverage on properties/methods. | Fill XML docs for all public members touched in M4. | None needed. | P1 |
| 7 | `src/TeaSharp/Controls/SelectionChangedEventArgs.cs`, `src/TeaSharp/Controls/ListSelectionChangedEventArgs.cs`, multiple controls | Selection event payload types are fragmented (generic, non-generic, custom). | Introduce canonical generic payload and keep old events as adapters. | Converge all controls to one event args family. | P1 |
| 8 | `src/TeaSharp/Controls/NotificationInbox.cs:127`, `src/TeaSharp/Controls/ActivityFeed.cs:307`, `src/TeaSharp/Controls/ListView.cs:196` | Selection mutation API names differ (`Select`, `SetSelectedIndex`, none). | Add a shared selection API contract (`TrySelectIndex`/`SetSelectedIndex`) without removing current methods. | Standardize and remove divergent verbs. | P1 |
| 9 | `src/TeaSharp/Controls/DropdownGlyphSet.cs`, `src/TeaSharp/Controls/DataGrid.cs:124`, `src/TeaSharp/Controls/TreeTable.cs:121` | Glyph customization style is inconsistent (typed glyph sets vs many raw marker strings). | Add typed glyph-set wrappers while preserving existing marker properties. | Replace marker string properties with glyph-set objects. | P1 |
| 10 | `src/TeaSharp/Controls/*` bordered controls (`BorderStyleText`/`FocusedBorderStyleText`) | Name implies text styling; actual purpose is frame/border glyph styling. | Add alias properties (`BorderStyle`, `FocusedBorderStyle`) and keep old names with deprecation plan. | Rename properties across controls. | P2 |
| 11 | `src/TeaSharp/Controls/JsonTreeNode.cs:75`, `src/TeaSharp/Controls/InspectorSection.cs:36`, `src/TeaSharp/Controls/TreeMapNode.cs:55` | Public mutable collections leak internal mutation semantics. | Add read-only views + explicit mutation methods; keep existing members temporarily. | Switch to immutable/read-only public collection surfaces. | P1 |
| 12 | `tests/TeaSharp.Tests/ThemeFocusMarkerParityPolicyTests.cs:76` | Focus-marker parity policy list covers only three controls; drift risk for broader control set. | Auto-discover controls exposing `FocusMarker` and enforce extension mapping parity. | None needed. | P2 |
| 13 | `src/TeaSharp/Tea.cs`, `src/TeaSharp/TeaApp.cs`, `src/TeaSharp/TeaApplication.cs`, `src/TeaSharp/TeaApplicationBuilder.cs` | Startup naming is correct but term overlap can still confuse onboarding. | Add explicit “when to use” guidance + analyzers/docs examples for each type. | Collapse/rename startup types to fewer concepts. | P2 |
| 14 | `TeaSharp.Controls` namespace (see `docs/prebuilt-widgets.md`) | Onboarding namespace is crowded with controls + support models/events/options in one surface. | Introduce sub-namespaces for support models with type-forwarded compatibility. | Move support types to dedicated namespaces and remove old placements. | P1 |

## Proposed Execution Order

1. Fix source-of-truth drift and XML doc gaps first: items 1, 4, 5, 6.
2. Decide notification surface strategy before freeze: item 2.
3. Standardize behavioral inconsistency with highest UI impact: item 3.
4. Normalize selection semantics/events and mutation verbs: items 7, 8.
5. Normalize glyph customization patterns: item 9.
6. Address mutable-collection exposures: item 11.
7. Improve policy enforcement and onboarding clarity: items 12, 13, 14.
8. Evaluate rename/deprecation wave for border naming once freeze migration plan is accepted: item 10.

