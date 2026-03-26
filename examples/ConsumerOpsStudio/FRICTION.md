# ConsumerOpsStudio Friction Notes

## Easy
- Public controls compose well for a realistic ops screen: `SideNavRail + CommandBar + Tabs + ListView + Table + LinePlot + Gauge + Dialog + CommandPalette + Notifications + LogView + StatusBar`.
- Pointer + keyboard event hooks are consistent enough to drive app-level command routing.

## Awkward (with explicit workarounds used)
- Selection sync on `ListView<T>` during live resort:
  - Friction: calling `SetItems` while list ordering changes can drift selection by index.
  - Workaround implemented: preserve selected service ID, repopulate list, then reselect by ID->index in `SyncServiceListSelection()`.

- Selection sync on `Table` rows:
  - Friction: row-selection events are index/visible-page based, and there is no public API to programmatically set selected row by ID after data refresh.
  - Workaround implemented: maintain `_visibleWorkItemIds` aligned with rendered rows and map `SelectionChanged.SelectedIndex` back to stable IDs.
  - Workaround implemented: keep row ordering stable enough per tab so user selection remains meaningful across refreshes.

- Runtime theme switching:
  - Friction: runtime theme is straightforward to set at startup, but in-app dynamic theme mode switching is not one direct high-level toggle across all controls.
  - Workaround implemented: `ApplyLocalOverrides()` reapplies per-control style properties for all controls when toggling normal/alert mode.

## Custom Logic Workarounds
- Shared command behavior across buttons/command bar/palette:
  - Friction: multiple activation surfaces can diverge in behavior if handled independently.
  - Workaround implemented: single `PerformCommand(string)` dispatch path used by all interaction sources.

- Dialog action context:
  - Friction: dialog callbacks expose accept/dismiss, but not domain action context.
  - Workaround implemented: pending action state tracked explicitly via `PendingDialogAction` + `_pendingServiceId`.

## Candidate API Improvements
- Additive: helper APIs for ID-based selection retention on collection controls.
- Additive: a first-class runtime theme switch/reapply primitive.
- Deeper: `Table` public API for programmatic row selection by index/ID.
