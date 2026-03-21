# External Consumer Review (V1)

Context: clean-room consumer pass by building `examples/ExternalConsumerReviewApp` using only `Tea`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`.

## Ranked Friction

### P1 - `Table` has no public selection state/events
- Issue: table supports keyboard/mouse row selection visually, but consumers cannot read selected row index/item or subscribe to selection changes.
- Repro snippet:
```csharp
var table = new Table("Name", "State");
table.SetRows(rows);
// Missing: table.SelectedIndex / table.SelectedRow / table.SelectionChanged
```
- Suggested non-breaking fix: add `SelectedRowIndex` (read-only), `TryGetSelectedRow(out IReadOnlyList<string> row)`, and `SelectionChanged` event. Keep existing behavior; just expose state.

### P1 - Runtime theme switch requires per-control boilerplate
- Issue: switching between Catppuccin/Rosé Pine requires manually re-applying theme to every control instance.
- Repro snippet:
```csharp
void ApplyTheme(TeaTheme theme)
{
    _tabs.ApplyTheme(theme);
    _table.ApplyTheme(theme);
    _list.ApplyTheme(theme);
    _notifications.ApplyTheme(theme);
    _dialog.ApplyTheme(theme);
    _status.ApplyTheme(theme);
}
```
- Suggested non-breaking fix: add helper on app surface (for example `ThemeScope.Apply(theme, params Control[] controls)`), or provide a documented aggregate helper for common dashboard control sets.

## Resolved In This Lane

### P2 - `Notifications` selection event
- Status: resolved with additive API.
- New surface: `Notifications.SelectionChanged` (`ListSelectionChangedEventArgs<InboxItem>` payload).
- Validation: `NotificationsSelectionChangedEventTests` covers `SetSelectedIndex`, `Clear`, and `RemoveSelected` transitions.

## Notes
- Current example demonstrates navigation, list/table, notifications, dialog flow, and theme switching without `TeaSharp.Core`.
- No breaking API changes proposed in this pass.
