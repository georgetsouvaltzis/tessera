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
  - overlay is only composed while visible, so closed dialog state cannot intercept pointer hit-tests
- pointer semantics:
  - runtime pointer activation policy is explicitly `SingleClick` for terminal compatibility (some terminals/paths coalesce or consume double-click)
  - hover (`PointerEventKind.Motion`) is visual-only and does not mutate selection
  - single-click on a services row selects immediately in the default dashboard composition
  - click tab to switch
  - wheel does not change tabs
  - note for default framework behavior: `DoubleClick` policy transfers focus on first press and activates on qualifying second press
- shortcuts:
  - `Ctrl+D` (or `d`) deploy
  - `Ctrl+T` (or `t`) theme toggle
  - `Ctrl+C` quit

Terminal prerequisites:

- terminal should support CSI mouse/focus/paste input (Ghostty, iTerm2, Windows Terminal, and macOS Terminal are expected to work on the byte-stream input path)
- app must request mouse reporting (`runtime.Screen.MouseTracking = MouseTrackingMode.CellMotion` in this example)
- runtime input must remain enabled (`DisableInput == false`)
- if using tmux, enable mouse forwarding (`set -g mouse on`)

Run:

```bash
dotnet run --project examples/PublicApiDashboard
```

Troubleshooting: terminal selects text instead of app pointer interaction

- if drag/click highlights terminal text and app hover/click handlers do not fire:
- confirm `MouseTracking` is still configured (`CellMotion` or `AllMotion`)
- confirm `TEASHARP_CAPS` does not disable mouse (`mouse=0`)
- confirm terminal session is interactive (not a redirected non-interactive run)
- when click only shifts focus but does not activate, check policy (`PointerActivationPolicy.DoubleClick` requires second press)
