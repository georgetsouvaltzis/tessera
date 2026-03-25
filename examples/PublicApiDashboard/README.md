# PublicApiDashboard Example

This example simulates a realistic "external consumer" implementation using only public TeaSharp APIs:

- navigation via `DashboardNavigationTabs` wrapper over `Tabs`:
  - wheel navigation blocked
  - motion is hover-only (defensive guard restores selection if a future regression mutates it)
  - runtime mouse mode uses `CellMotion` as hard fallback to prevent hover spam in noisy terminals
- data area via `ListView<T>` and `Table`
- operational feedback via `LogView` and `Notifications`
- status footer via `StatusBar`
- modal action via `Dialog` (deploy confirmation)
- pointer semantics:
  - runtime pointer activation policy is explicitly `SingleClick` for terminal compatibility (some terminals/paths coalesce or consume double-click)
  - click tab to switch
  - wheel does not change tabs
- shortcuts:
  - `Ctrl+D` (or `d`) deploy
  - `Ctrl+T` (or `t`) theme toggle
  - `Ctrl+C` quit

Run:

```bash
dotnet run --project examples/PublicApiDashboard
```
