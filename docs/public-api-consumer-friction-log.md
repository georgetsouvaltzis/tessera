# Public API Consumer Friction Log

Context: consumer-centric pass re-checked against current `examples/PublicApiDashboard` and latest control/theme commits.

## Status Matrix

| Friction Item | Status | Brief Rationale | Pointers |
|---|---|---|---|
| Header composition ergonomics | Resolved | `WindowBuilder.HeaderRow(...)` now supports multi-control header composition directly. | `9cf07b2`, `src/TeaSharp/Layout/ScreenBuilder.cs:57`, `tests/TeaSharp.Tests/WindowBuilderHeaderRowCompositionTests.cs` |
| Dialog result handling shape | Resolved | Typed `Closed` event landed; consumers can branch on one event payload (`DialogResult`) instead of wiring both events manually. | `184e3ae`, `src/TeaSharp/Controls/Dialog.cs:32`, `src/TeaSharp/Controls/DialogClosedEventArgs.cs`, `tests/TeaSharp.Tests/DialogClosedEventTests.cs` |
| Table data binding loop | Resolved | Incremental row mutation APIs landed (`AddRow`, `ReplaceRow`, `RemoveRowAt`, `ClearRows`) while keeping `SetRows`. | `ff557e6`, `src/TeaSharp/Controls/Table.cs:191`, `src/TeaSharp/Controls/Table.cs:206`, `src/TeaSharp/Controls/Table.cs:220`, `tests/TeaSharp.Tests/TableRowMutationApiTests.cs` |
| Cross-control focus/selection conventions | Resolved | For the dashboard control set, focus/title/row style hooks are now coherent and theme/focus-marker parity checks exist. | `src/TeaSharp/Controls/ListView.cs:45`, `src/TeaSharp/Controls/Table.cs:38`, `src/TeaSharp/Controls/Notifications.cs:23`, `src/TeaSharp/Controls/LogView.cs:30`, `tests/TeaSharp.Tests/ThemeFocusMarkerParityPolicyTests.cs`, `tests/TeaSharp.Tests/BorderedControlParityPolicyTests.cs` |
| Theme + local override workflow | Resolved | Reusable dashboard override-bundle API landed and is used by the consumer example. | `d067b1d`, `src/TeaSharp/Styles/TeaThemeOverrideBundle.cs:10`, `src/TeaSharp/Styles/TeaThemeOverrideBundleExtensions.cs:17`, `examples/PublicApiDashboard/Program.cs:351`, `tests/TeaSharp.Tests/ThemeOverrideBundleApiErgonomicsTests.cs` |
| Runtime tick boilerplate | Resolved | Auto-rescheduling periodic effect API landed (`TeaEffects.Periodic`) with runtime unwrapping/reschedule plumbing. | `07ae666`, `src/TeaSharp/TeaEffects.cs:81`, `src/TeaSharp/TeaApp.cs:108`, `src/TeaSharp/Internal/TeaPeriodicEffectMessage.cs`, `tests/TeaSharp.Tests/TeaEffectsPeriodicApiErgonomicsTests.cs` |
| Notification surface split (`Notifications` vs `NotificationInbox`) | Resolved | `Notifications` now exposes full inbox-style item/selection mutation APIs as the default onboarding path; `NotificationInbox` remains available for advanced dev/ops workflows. | `86df879`, `src/TeaSharp/Controls/Notifications.cs:83`, `src/TeaSharp/Controls/Notifications.cs:147`, `tests/TeaSharp.Tests/NotificationsPrimaryApiTests.cs`, `src/TeaSharp/Controls/InboxItem.cs:4` |
| Notifications selection observation | Resolved | `Notifications.SelectionChanged` now emits typed selection payloads (`ListSelectionChangedEventArgs<InboxItem>`) so consumers no longer need polling to react to selection changes. | `src/TeaSharp/Controls/Notifications.cs`, `tests/TeaSharp.Tests/NotificationsSelectionChangedEventTests.cs`, `docs/external-consumer-review-v1.md` |
| Selection mutation/event conventions | Resolved (policy) | Selection API normalization landed (`SetSelectedIndex` pass + `Selected*` aliases). Residual compatibility names (`Current*`) are now explicitly governed by post-V1 migration policy rather than left ambiguous in V1. | `refactor: normalize setselectedindex across list controls`, `refactor: add selected aliases for selection event args`, `docs/post-v1-selection-naming-migration.md`, `src/TeaSharp/Controls/KeyValueListSelectionChangedEventArgs.cs:41`, `src/TeaSharp/Controls/PropertyGridSelectionChangedEventArgs.cs:41`, `src/TeaSharp/Controls/ValidationSelectionChangedEventArgs.cs:41`, `src/TeaSharp/Controls/JsonTreeSelectionChangedEventArgs.cs:41` |

## Next Top 3 Ergonomic Priorities

1. Execute V1.x de-emphasis: keep compatibility members, but remove `Current*` from onboarding examples/docs where canonical `Selected*` exists.
2. Continue naming/docs consistency pass for remaining public theme-extension APIs to reduce source-diving.
3. Keep advanced-vs-default path labels explicit in widget docs so onboarding stays `TeaSharp`-first.

## Wave-2 External Consumer Findings (Open)

| Friction Item | Severity | Brief Rationale | Pointers |
|---|---|---|---|
| StatsCard border/padding styling gap | P1 | `StatsCard` lacks `Border`/`Padding`/border style hooks, so status cards cannot match theme-driven border behavior used by most bordered controls. | `examples/ExternalConsumerReviewApp/ExternalConsumerReviewApp.Wave2.cs`, `src/TeaSharp/Controls/StatsCard.cs`, `docs/external-consumer-review-v1.md` |
| LinePlot streaming retention ergonomics | P1 | `LineSeries.Append(...)` is unbounded; consumers must implement manual copy+trim loops for long-running dashboards. | `examples/ExternalConsumerReviewApp/ExternalConsumerReviewApp.Wave2.cs`, `src/TeaSharp/Controls/LineSeries.cs`, `docs/external-consumer-review-v1.md` |
| LinePlot mixed-unit readability (single shared Y scale) | P1 | No per-series scale mode or secondary axis; mixed metrics (req/s vs ms) become hard to read in one panel. | `examples/ExternalConsumerReviewApp/ExternalConsumerReviewApp.Wave2.cs`, `src/TeaSharp/Controls/LinePlot.cs`, `docs/external-consumer-review-v1.md` |
| Generic `Options` naming for plot controls | P2 | Advanced plot setup hangs off generic `Options` property (`LinePlotOptions`, etc.), which is less discoverable for first-time consumers. | `src/TeaSharp/Controls/LinePlot.cs`, `src/TeaSharp/Controls/Sparkline.cs`, `src/TeaSharp/Controls/ScatterPlot.cs`, `docs/external-consumer-review-v1.md` |
