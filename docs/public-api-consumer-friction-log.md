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
| Selection mutation/event conventions | In progress | Selection API normalization moved forward (`SetSelectedIndex` pass + `Selected*` aliases on selection event args). Residual gap: some `Current*` members remain primary for compatibility and still need final naming cleanup policy for post-V1. | `refactor: normalize setselectedindex across list controls`, `refactor: add selected aliases for selection event args`, `src/TeaSharp/Controls/KeyValueListSelectionChangedEventArgs.cs:41`, `src/TeaSharp/Controls/PropertyGridSelectionChangedEventArgs.cs:41`, `src/TeaSharp/Controls/ValidationSelectionChangedEventArgs.cs:41`, `src/TeaSharp/Controls/JsonTreeSelectionChangedEventArgs.cs:41` |

## Next Top 3 Ergonomic Priorities

1. Close the residual selection naming gap by deciding post-V1 migration from `Current*` primaries to canonical `Selected*` primaries.
2. Continue naming/docs consistency pass for remaining public theme-extension APIs to reduce source-diving.
3. Keep advanced-vs-default path labels explicit in widget docs so onboarding stays `TeaSharp`-first.
