# TeaSharp Bubble Tea Parity Matrix

Snapshot date: 2026-03-04  
Legend: `done` = implemented, `partial` = usable but incomplete, `todo` = not implemented.

## Programming Model

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Model lifecycle | `Init / Update / View` | done | Core interface matches expected loop shape. |
| Program run loop | event-driven message loop | done | Single message channel + command channel. |
| External messages | `Program.Send` | done | `TeaProgram.Send(IMessage)` supported. |
| Message filtering | middleware/filter hook | done | `ProgramOptions.Filter` supports drop/transform. |
| FPS throttling | max render rate | partial | Basic `MaxFps` throttle; no adaptive strategy. |
| Cancellation | program stop and linked tokens | done | Linked CTS + `StopAsync`. |

## Commands/Effects

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| No-op command | `nil` command behavior | done | `Commands.None`. |
| Quit command | `tea.Quit` | done | `QuitMsg` path implemented. |
| Interrupt command | interrupt signal path | done | `InterruptMsg` to exception path. |
| Batch commands | `tea.Batch` | done | Concurrent scheduling via command loop. |
| Sequence commands | `tea.Sequence` | done | Serial execution path implemented. |
| Timers | `Tick / Every` | done | Supported in `Commands`. |
| Command error handling | panic/error propagation policy | partial | Optional exception wrapping via `CommandErrorMsg`; no panic/recover policy parity yet. |

## Input/Terminal Protocol

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Character input | UTF-8 keys | done | Rune decode + `Console.ReadKey` fallback. |
| Navigation keys | arrows/tab/enter/backspace/esc | done | Core keys mapped. |
| VT control decode | CSI/SS3/OSC parsing | partial | Core cursor/edit keys, resize, OSC consumption implemented; full matrix still pending. |
| Ctrl modifiers | control key combos | partial | Core ctrl path plus CSI `u` / modifyOtherKeys (`CSI 27;...~`) decode supported; full key matrix parity still pending. |
| Alt/meta handling | alt key combos | partial | Escape-prefix + console modifiers + CSI enhanced key modifiers supported; edge cases missing. |
| Bracketed paste protocol | start/end/content handling | done | Start/end decode and aggregated `PasteMsg` content are implemented. |
| Mouse protocol | X10/SGR mouse messages | partial | SGR 1006 + basic X10 decode implemented with base `MouseMsg` plus typed variants (`MouseClickMsg`, `MouseReleaseMsg`, `MouseMotionMsg`, `MouseWheelMsg`); richer button edge cases and high-button parity still pending. |
| Focus reporting | focus in/out messages | done | CSI focus in/out decode + render-mode toggle implemented. |
| Resize updates | runtime terminal resize events | partial | Initial size + CSI parser support, plus Unix `SIGWINCH`-assisted checks with polling fallback; Windows still relies on polling. |

## Rendering

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| ANSI output | VT rendering | done | ANSI renderer active. |
| Diff rendering | efficient incremental updates | partial | Cell-run diff with grapheme/wide-char width handling is implemented; full style/cell-attribute parity still pending. |
| Alt screen | alternate buffer enter/leave | done | `View.AltScreen` implemented. |
| Cursor visibility/position | cursor control | partial | Show/hide + absolute position supported; no style/blink parity. |
| Synchronized updates | synchronized paint | partial | Frame output is wrapped with `?2026h`/`?2026l`; `DECRPM` mode queries/reports are now surfaced, but terminal support is still best-effort. |
| Window title | OSC title | done | `View.WindowTitle` now emits OSC title sequence. |
| Style/render integration | lipgloss-like style composition | partial | Basic component drawing API (`Rect`, `Canvas`, `Widgets`) added for deterministic panels/progress/sparkline/list composition; no ANSI style engine/lipgloss-equivalent yet. |

## Cross-Platform Runtime

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Windows VT setup | console mode configuration | done | VT input/output setup + restore implemented. |
| Unix raw mode | non-canonical no-echo mode | done | `stty raw -echo` with restore path. |
| TTY fallback | interactive run under redirected stdio | done | `/dev/tty` binding + console-key fallback. |
| Capability negotiation | terminal feature detection | partial | Environment-driven `TerminalCapabilityProfile` now gates focus/mouse/paste/sync toggles and `DECRPM` queries; `TerminalCapabilitiesMsg` is emitted at startup and refined at runtime from `ModeReportMsg`; Unix-like detection is enriched via best-effort `infocmp -x` parsing. Full terminfo-database parity and deeper runtime probing are still pending. |

## Test Parity

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Behavior tests | loop/command semantics | done | Core regression tests pass. |
| Protocol decode tests | key/mouse/paste parser fixtures | partial | Golden fixtures include CSI/SS3/OSC, modifiers, focus, and extended mouse buttons/modifier combinations; terminal-specific fixture breadth still pending. |
| Renderer snapshots | render diff correctness | done | Deterministic renderer snapshots now cover first-frame mode/title sequences, incremental diff patches, and reset teardown control sequences. |

## Priority Gap Plan

1. P1: Cell-buffer renderer (row+cell diff) replacing line-only diff.
2. P1: Runtime resize watcher parity across macOS/Linux/Windows.
3. P2: Deep capability probing (beyond env + basic `infocmp` enrichment).
4. P2: Expand mouse parity for extended/high-button mappings and compatibility fixtures.
