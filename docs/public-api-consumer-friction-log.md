# Public API Consumer Friction Log

Context: consumer-centric pass re-checked against current `examples/PublicApiDashboard` and latest control/theme commits.

## Status Matrix

| Friction Item | Status | Brief Rationale | Pointers |
|---|---|---|---|
| Header composition ergonomics | Resolved | `WindowBuilder.HeaderRow(...)` now supports multi-control header composition directly. | `9cf07b2`, `src/TeaSharp/Layout/ScreenBuilder.cs:57`, `tests/TeaSharp.Tests/WindowBuilderHeaderRowCompositionTests.cs` |
| Dialog result handling shape | Resolved | Typed `Closed` event landed; consumers can branch on one event payload (`DialogResult`) instead of wiring both events manually. | `184e3ae`, `src/TeaSharp/Controls/Dialog.cs:32`, `src/TeaSharp/Controls/DialogClosedEventArgs.cs`, `tests/TeaSharp.Tests/DialogClosedEventTests.cs` |
| Table data binding loop | Resolved | Incremental row mutation APIs landed (`AddRow`, `ReplaceRow`, `RemoveRowAt`, `ClearRows`) while keeping `SetRows`. | `ff557e6`, `src/TeaSharp/Controls/Table.cs:191`, `src/TeaSharp/Controls/Table.cs:206`, `src/TeaSharp/Controls/Table.cs:220`, `tests/TeaSharp.Tests/TableRowMutationApiTests.cs` |
| Cross-control focus/selection conventions | Resolved | For the dashboard control set, focus/title/row style hooks are now coherent and theme/focus-marker parity checks exist. | `src/TeaSharp/Controls/ListView.cs:45`, `src/TeaSharp/Controls/Table.cs:38`, `src/TeaSharp/Controls/Notifications.cs:23`, `src/TeaSharp/Controls/LogView.cs:30`, `tests/TeaSharp.Tests/ThemeFocusMarkerParityPolicyTests.cs`, `tests/TeaSharp.Tests/BorderedControlParityPolicyTests.cs` |
| Theme + local override workflow | Open | Guidance improved, but consumers still repeat local override code; no reusable override-bundle helper API yet. | `examples/PublicApiDashboard/Program.cs:336`, `docs/prebuilt-widgets.md:259` |
| Runtime tick boilerplate | Open | `TeaEffects.Every` exists, but periodic update loops still require explicit re-scheduling in app code. | `examples/PublicApiDashboard/Program.cs:174`, `src/TeaSharp/TeaEffects.cs:42`, `src/TeaSharp/TeaEffects.cs:58` |

## Next Top 3 Ergonomic Priorities

1. Add reusable override-bundle helpers to reduce repeated local theme override snippets.
2. Add an opt-in periodic effect helper that removes manual self-rescheduling in `Update(...)`.
3. Normalize selection mutation/event conventions across list-like controls (`SetSelectedIndex`/`SelectionChanged` payload shape).
