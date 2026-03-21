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

## Next Top 3 Ergonomic Priorities

1. Normalize selection mutation/event conventions across list-like controls (`SetSelectedIndex`/`SelectionChanged` payload shape).
2. Consolidate overlapping notification control surfaces (`Notifications` vs `NotificationInbox`) into a clearer primary app path.
3. Continue naming/docs consistency pass for remaining public theme-extension APIs to reduce source-diving.
