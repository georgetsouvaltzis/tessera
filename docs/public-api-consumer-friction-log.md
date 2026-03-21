# Public API Consumer Friction Log

Context: consumer-centric pass re-checked against current `examples/PublicApiDashboard` and latest control/theme commits.

## Status Matrix

| Friction Item | Status | Brief Rationale | Pointers |
|---|---|---|---|
| Header composition ergonomics | Open | Header still accepts a single slot; mixed header composition still pushed into body/layout plumbing. | `examples/PublicApiDashboard/Program.cs:187`, `examples/PublicApiDashboard/Program.cs:227` |
| Dialog result handling shape | Open | `Dialog` now exposes `LastResult` and `TryConsumeResult`, but no single typed `Closed` event yet; consumers still commonly wire both events. | `examples/PublicApiDashboard/Program.cs:133`, `src/TeaSharp/Controls/Dialog.cs:22`, `src/TeaSharp/Controls/Dialog.cs:27`, `src/TeaSharp/Controls/Dialog.cs:143`, `src/TeaSharp/Controls/Dialog.cs:172` |
| Table data binding loop | Open | `Table` remains row-array oriented via `SetRows`; no incremental row API (`UpdateRow`/`ReplaceRow`) on the public surface. | `examples/PublicApiDashboard/Program.cs:182`, `src/TeaSharp/Controls/Table.cs:173` |
| Cross-control focus/selection conventions | Resolved | For the dashboard control set, focus/title/row style hooks are now coherent and theme/focus-marker parity checks exist. | `src/TeaSharp/Controls/ListView.cs:45`, `src/TeaSharp/Controls/Table.cs:38`, `src/TeaSharp/Controls/Notifications.cs:23`, `src/TeaSharp/Controls/LogView.cs:30`, `tests/TeaSharp.Tests/ThemeFocusMarkerParityPolicyTests.cs`, `tests/TeaSharp.Tests/BorderedControlParityPolicyTests.cs` |
| Theme + local override workflow | Open | Guidance improved, but consumers still repeat local override code; no reusable override-bundle helper API yet. | `examples/PublicApiDashboard/Program.cs:336`, `docs/prebuilt-widgets.md:259` |
| Runtime tick boilerplate | Open | `TeaEffects.Every` exists, but periodic update loops still require explicit re-scheduling in app code. | `examples/PublicApiDashboard/Program.cs:174`, `src/TeaSharp/TeaEffects.cs:42`, `src/TeaSharp/TeaEffects.cs:58` |

## Next Top 3 Ergonomic Priorities

1. Add a single dialog completion event (`Closed` + `DialogResult`) while keeping `Accepted`/`Dismissed`.
2. Add incremental `Table` mutation APIs (`UpdateRow`, `ReplaceRow`, keyed updates) without removing `SetRows`.
3. Add header composition overloads that support multiple controls directly (`window.Header(...)` builder/slot overloads).

