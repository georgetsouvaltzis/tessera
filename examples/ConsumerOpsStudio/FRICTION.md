# ConsumerOpsStudio Friction Notes

## Resolved In This Repo State
- Table reselection after refresh:
  - Previously needed workaround pressure because table rows could not be reselected programmatically.
  - Now resolved via `Table.SetSelectedIndex(int)`.
  - App update: `RefreshWorkRows()` now preserves current tab selection by stable ID and re-applies selection after `SetRows(...)`.

## Still Open (With Current Workarounds)
- `ListView<T>` live-sort selection drift:
  - Friction: `SetItems(...)` after resort shifts index-based selection.
  - Workaround: keep selected service ID, repopulate, then reselect by ID->index in `SyncServiceListSelection()`.

- Table row identity is still app-managed:
  - Friction: table exposes row values/events but no first-class row key identity.
  - Workaround: maintain `_visibleWorkItemIds` aligned to rendered rows and map event indices back to stable IDs.

- Dynamic theme switching ergonomics:
  - Friction: runtime theme setup is easy at startup, but in-app theme mode switching is not a single high-level control-tree operation.
  - Workaround: `ApplyLocalOverrides()` reapplies per-control styles when toggling normal/alert mode.

- Multi-surface command behavior:
  - Friction: button, command bar, and palette activation paths can diverge.
  - Workaround: route all command entry points through one `PerformCommand(string)` dispatch.

- Dialog intent context:
  - Friction: dialog callbacks return accept/dismiss but not domain intent.
  - Workaround: track pending intent in app state with `PendingDialogAction` + `_pendingServiceId`.

## Candidate API Improvements
- Additive: first-class selection retention helpers (ID-keyed) for list/table controls.
- Additive: higher-level runtime theme mode switching/reapply primitive.
- Deeper: table row key API so apps can avoid external index-to-ID maps.
