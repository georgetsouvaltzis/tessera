# PublicApiDashboard Example

This example simulates a realistic "external consumer" implementation using only public TeaSharp APIs:

- navigation via `DashboardNavigationTabs` wrapper over `Tabs`:
  - wheel navigation blocked
  - motion is hover-only (defensive guard restores selection if a future regression mutates it)
- data area via `ListView<T>` and `Table`
- operational feedback via `LogView` and `Notifications`
- status footer via `StatusBar`
- modal action via `Dialog` (deploy confirmation)
- shortcuts:
  - `Ctrl+D` (or `d`) deploy
  - `Ctrl+T` (or `t`) theme toggle
  - `Ctrl+C` quit

Run:

```bash
dotnet run --project examples/PublicApiDashboard
```
