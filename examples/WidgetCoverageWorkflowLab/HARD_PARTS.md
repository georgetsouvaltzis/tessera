# HARD_PARTS

Consumer perspective from building `WidgetCoverageWorkflowLab` only via public APIs.

## 1) TagInput and TokenEditor state-change detection

- what was hard:
  Detecting when tags/reviewer tokens changed so validation and derived views could update immediately.
- why it was hard:
  `TagInput` and `TokenEditor` expose state (`Tags`, `Tokens`) but no additive change/commit events for normal consumer workflows.
  A typical C# app expects event hooks, not polling snapshots every update cycle.
- workaround used:
  Snapshot polling (`string.Join(...)`) in app update loop and re-running validation when snapshots differ.
- exact public API improvement that would help:
  Add events:
  `event EventHandler<TagInputChangedEventArgs> TagInput.Changed`
  `event EventHandler<TokenEditorChangedEventArgs> TokenEditor.Changed`
  with added/removed token data and current selection index.

## 2) NotificationInbox selection/reactive wiring

- what was hard:
  Keeping inspector/status in sync with user selection in `NotificationInbox`.
- why it was hard:
  `NotificationInbox` has selection APIs but no `SelectionChanged` event, unlike `Notifications`.
  A consumer cannot react to selection changes without polling `SelectedIndex`/`SelectedItem`.
- workaround used:
  Triggered explicit `Select(...)` in app-owned flows and treated inbox as mostly passive/read-only in derived state.
- exact public API improvement that would help:
  Add
  `event EventHandler<ListSelectionChangedEventArgs<InboxItem>> NotificationInbox.SelectionChanged`.

## 3) DataForm + external editors two-way sync

- what was hard:
  Coordinating `DataForm<ChangeDraft>` edits with external controls (`TextInput`, `AutocompleteInput`, `NumberInput`).
- why it was hard:
  Consumer must manually mirror values across controls and model updates.
  There is no built-in small helper for “commit field then reflect into bound sibling controls”.
- workaround used:
  Manual synchronization in each event handler (`Submitted`, `SuggestionCommitted`, number submits), plus `SetModel(...)` refreshes.
- exact public API improvement that would help:
  Add a small additive helper on `DataForm<TModel>`:
  `bool TryUpdateField(string key, string value, out string? error)`
  that reuses validators/commit semantics and keeps internal edit state consistent.

## 4) SearchBox match-navigation payload ergonomics

- what was hard:
  Connecting `SearchBox` navigation to actual matched content for the runbook editor flow.
- why it was hard:
  Navigation event provides direction/index metadata, but not the matched item/value.
  Consumer must keep and index its own match list everywhere.
- workaround used:
  Maintained local filtered suggestion array and manual index arithmetic.
- exact public API improvement that would help:
  Add optional payload on navigation event:
  `string? CurrentMatchText` (or generic overload for external match providers).

## 5) Issue routing boilerplate across many controls

- what was hard:
  Mapping validation issue field keys to focus/selection transitions across DataForm + non-DataForm controls.
- why it was hard:
  Consumers need a large switch statement for focus choreography in realistic forms/workflow apps.
- workaround used:
  Centralized `RouteIssueSelection(string field)` method with explicit per-control routing.
- exact public API improvement that would help:
  Add additive helper in app-facing layer:
  `ValidationRouter` utility that maps field keys to focus callbacks, e.g. `Map("ticket", _ticketInput.RequestFocus)`.
