# Public API Consumer Friction Log

Context: built `examples/PublicApiDashboard` as if from an external C# app team, using only `TeaSharp`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`.

## 1) Header Composition Ergonomics
- Scenario: placing `Tabs` and a primary action button in one top area.
- Friction: `Screen.Build` header accepts one content slot, so composing mixed-height header content requires extra `RowLayout`/`ColumnLayout` plumbing.
- Proposed API improvement (non-breaking preferred): add `window.Header(...)` overloads that accept multiple slots or a builder callback (similar to body composition).
- Severity: Medium
- Suggested owner lane: A

## 2) Dialog Result Handling Shape
- Scenario: open modal from key/button and branch logic on accept vs dismiss.
- Friction: most consumer code ends up wiring both `Accepted`/`Dismissed` events manually and tracking context externally.
- Proposed API improvement (non-breaking preferred): add a typed `Closed` event with `DialogResult`, preserving existing events.
- Severity: Medium
- Suggested owner lane: A

## 3) Table Data Binding Loop
- Scenario: dashboard telemetry updates table rows frequently.
- Friction: public API is row-array oriented (`SetRows`), so incremental updates require rebuilding row lists repeatedly.
- Proposed API improvement (non-breaking preferred): add optional row model APIs (`SetItems<T>`, `UpdateRow`, `ReplaceRow`) while keeping current `SetRows`.
- Severity: Medium
- Suggested owner lane: A

## 4) Cross-Control Focus/Selection Conventions
- Scenario: keeping `ListView<T>`, `Table`, `Notifications`, and `LogView` visuals cohesive.
- Friction: style hook names are mostly consistent, but row-state hook naming still differs enough that consumers keep checking docs/source.
- Proposed API improvement (non-breaking preferred): publish a small "state-style naming matrix" and add missing aliases where feasible.
- Severity: Low
- Suggested owner lane: B

## 5) Theme + Local Override Workflow
- Scenario: apply theme defaults, then tweak several controls locally for product identity.
- Friction: consumer code repeats token-merge snippets (selected row, focused border) across controls.
- Proposed API improvement (non-breaking preferred): provide helper methods or cookbook extension snippets for common override bundles.
- Severity: Low
- Suggested owner lane: B

## 6) Runtime Tick Boilerplate
- Scenario: periodic telemetry simulation in app update loop.
- Friction: recurring `TeaEffects.Tick(...)` re-scheduling pattern is verbose in small apps.
- Proposed API improvement (non-breaking preferred): add an opt-in periodic timer helper for `TeaApp` with named intervals.
- Severity: Low
- Suggested owner lane: C

## Assumptions and Workarounds Used
- Assumption: periodic dashboard refresh can be simulated via repeated `TeaEffects.Tick(...)`.
- Workaround: table rows are fully rebuilt each update tick; no incremental table update API used.
- Assumption: event-based dialog handling (`Accepted`/`Dismissed`) is the supported path for command confirmation.
- Workaround: merged border and selected-row styles are computed once per theme application to reduce repetition in render code.
