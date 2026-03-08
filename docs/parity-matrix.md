# TeaSharp Bubble Tea Parity Matrix

Snapshot date: 2026-03-07  
Legend: `done` = implemented, `todo` = not implemented.

## Programming Model

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Model lifecycle | `Init / Update / View` | done | Core interface matches expected loop shape. |
| Program run loop | event-driven message loop | done | Single message channel + command channel. |
| External messages | `Program.Send` | done | `TeaProgram.Send(IMessage)` supported. |
| Message filtering | middleware/filter hook | done | `ProgramOptions.Filter` supports drop/transform. |
| FPS throttling | max render rate | done | `MaxFps` throttle plus adaptive burst coalescing (`AdaptiveFramePacing`) are implemented. |
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
| Raw terminal writes | `tea.Raw(...)` | done | `RawOutputMsg` + `Tea.Cmd.Raw(...)` write unmanaged terminal sequences through renderer. |
| Capability/OSC query commands | `RequestCapability` + color/clipboard requests | done | `Tea.Cmd.RequestCapability`, clipboard OSC52 commands, and color query commands are implemented and decoder-backed. |
| Command error handling | panic/error propagation policy | done | `CatchCommandExceptions=true` emits `CommandErrorMsg` by default and supports recover hooks via `RecoverCommandException`; `CatchCommandExceptions=false` deterministically propagates command failures through the run loop interrupt path. |

## Input/Terminal Protocol

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Character input | UTF-8 keys | done | Rune decode + `Console.ReadKey` fallback. |
| Navigation keys | arrows/tab/enter/backspace/esc | done | Core keys mapped, including SS3/CSI function-key variants (`F1`-`F12`). |
| VT control decode | CSI/SS3/OSC parsing | done | CSI/SS3/OSC decode coverage includes cursor/edit/function keys, enhanced CSI-u typing (press/repeat/release), resize, focus, paste, mode reports, and unknown-sequence fallback handling. |
| DCS capability decode | XTGETTCAP response parsing | done | DCS `ESC P ... ESC \\` capability responses decode to `CapabilityMsg` for runtime/profile refinement and app-level handling. |
| Ctrl modifiers | control key combos | done | Control-path decode covers control bytes, CSI `u`, modifyOtherKeys (`CSI 27;...~`), and event typing (`;2`/`;3` and `:3` forms). |
| Alt/meta handling | alt key combos | done | Escape-prefix fallbacks, console modifiers, nested escape forms, and enhanced CSI modifier decoding (including meta combinations) are implemented and fixture-covered. |
| Bracketed paste protocol | start/end/content handling | done | Start/end decode and aggregated `PasteMsg` content are implemented. |
| Mouse protocol | X10/SGR mouse messages | done | X10/SGR/1015-style decode is implemented with base `MouseMsg` and typed variants (`MouseClickMsg`, `MouseReleaseMsg`, `MouseMotionMsg`, `MouseWheelMsg`), including extended button mapping through `MouseButton.Button24` plus fixture coverage for high-button and modifier paths. |
| Focus reporting | focus in/out messages | done | CSI focus in/out decode + render-mode toggle implemented. |
| Resize updates | runtime terminal resize events | done | Initial size, CSI resize decode, Unix `SIGWINCH`-assisted checks with polling fallback, and Windows console-input resize signal registration/fallback are implemented with regression coverage. |

## Rendering

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| ANSI output | VT rendering | done | ANSI renderer active. |
| Diff rendering | efficient incremental updates | done | Renderer uses an explicit frame cell-buffer (`RenderFrameBuffer`) with row+cell run diffing, wide/combining continuation safety, bottom-row retention on overflow, and style-aware patching across supported SGR attributes. |
| Alt screen | alternate buffer enter/leave | done | `View.AltScreen` implemented. |
| Cursor visibility/position | cursor control | done | Show/hide, absolute positioning, and optional cursor-shape/blink control via DECSCUSR (`CSI Ps SP q`) are integrated into render lifecycle and teardown. |
| Terminal color controls | foreground/background/cursor color | done | `View.ForegroundColor`, `View.BackgroundColor`, and `View.CursorColor` emit OSC color set/reset sequences (`10/11/12`, `110/111/112`). |
| Native terminal progress | terminal progress bar channel | done | `View.Progress` emits OSC `9;4` progress state/value sequences (default/error/warning/indeterminate/reset). |
| Synchronized updates | synchronized paint | done | Frame output supports synchronized update wrapping (`?2026h`/`?2026l`) with capability gating and mode-report-driven runtime refinement. |
| Window title | OSC title | done | `View.WindowTitle` now emits OSC title sequence. |
| Keyboard enhancement request | kitty key enhancement negotiation | done | `View.KeyboardEnhancements` emits kitty keyboard flag sequences and decodes enhancement reports via `KeyboardEnhancementsMsg`. |
| Mouse interception hook | view-level mouse callback | done | `View.OnMouse` can emit a command from last-rendered view context before normal model update flow. |
| Style/render integration | lipgloss-like style composition | done | Composable ANSI style API (`TeaStyle`, `AnsiColor`) is integrated with renderer SGR parsing/diff patching and component primitives for style-safe composition. |
| Component text pipeline | grapheme-safe component text rendering | done | `Canvas` provides deterministic fast and grapheme-aware text paths for wide/combining glyph placement, with compatibility behavior validated in component tests. |

## Widget Layer

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Viewport model | scrollable content model | done | `ViewportModel` supports vertical/horizontal scrolling, wheel handling, optional soft-wrap, optional line-number gutter (`ShowLineNumbers`), and highlighted visual rows (`HighlightVisualLine`). |
| Text input model | editable input model | done | `TextInputModel` supports cursor movement, selection, submit handling, word-level edits, and multiline editing with vertical navigation. |
| List model | selectable/filterable list | done | `ListModel<T>` supports paging/filtering/selection visibility/wheel navigation, async load helpers (`SetItemsAsync`, `AppendItemsAsync`), tracked async orchestrators (`ReloadAsync`, `AppendAsync`), and optional custom sorting (`SortComparison`). |
| Keymap/help model | reusable key bindings + help | done | `KeyBinding`, widget keymaps, compact help wrapping, and expanded column help layout (`HelpView.RenderColumns`) are implemented. |

## Component Layer

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Composable components | reusable view components | done | `ICanvasComponent`, `IStatefulComponent`, and `ComponentComposer` support slot-based composition and stateful message routing. |
| Chart primitives | sparkline/plot-style components | done | `Charts.DrawLineChart`, `Charts.DrawBarChart`, `LineChartComponent`, and `BarChartComponent` are implemented with axes/labels/legend, plus zoom+offset windowing (`LineChartOptions.Zoom/Offset`) and component helpers (`ZoomIn`, `ZoomOut`, `Pan`). |
| UI-kit widgets/layout | reusable higher-level components | done | `Layout` helpers, tabs/accordion/table/forms/toast/modal/timeline/tree/calendar/skeleton are implemented with theming (`UiTheme`) and sortable-table virtual window rendering (`EnableVirtualization`, `SetVirtualWindow`). A prebuilt 1.0-focused widget layer now also ships (`Label`, `Button`, `TextInput`, `TextArea`, `List`, `Table`, `ProgressBar`, `Tabs`, `Modal/Dialog`, `StatusBar`, `LogViewer`, `LayoutContainer`) with dedicated gallery app coverage. |

## Cross-Platform Runtime

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Windows VT setup | console mode configuration | done | VT input/output setup + restore implemented. |
| Unix raw mode | non-canonical no-echo mode | done | `stty raw -echo` with restore path. |
| TTY fallback | interactive run under redirected stdio | done | `/dev/tty` binding + console-key fallback. |
| Capability negotiation | terminal feature detection | done | Environment + terminfo + bounded active `DECRPM` probing drive capability gating and runtime refinement (`+probe-timeout` / `+probe-partial-timeout`), with legacy mouse preservation heuristics and explicit `TEASHARP_CAPS` overrides. |
| Color profile signaling | profile detection + runtime update | done | Startup emits `ColorProfileMsg` from env/profile detection (overridable via `ProgramOptions.ColorProfile`), with capability-response refinement support. |

## Test Parity

| Area | Bubble Tea Capability | TeaSharp | Notes |
|---|---|---|---|
| Behavior tests | loop/command semantics | done | Core regression tests pass. |
| Integration tests | deterministic app-level UX behavior | done | NUnit integration suite covers workspace routing/showcase flows and tmux-backed smoke scenarios for command-mode, hotkeys, pane cycling, and quit semantics. |
| Protocol decode tests | key/mouse/paste parser fixtures | done | Golden protocol fixtures cover CSI/SS3/OSC, modifier matrices, CSI-u event typing, focus, paste, and extended mouse/button/modifier paths. |
| Terminal behavior fixtures | emulator-specific key/paste/focus regressions | done | Fixture coverage includes Ghostty, iTerm2, tmux/xterm, kitty, wezterm, Apple Terminal, alacritty, konsole/meta-key paths, and urxvt-style mouse sequences. |
| Renderer snapshots | render diff correctness | done | Deterministic renderer snapshots now cover first-frame mode/title sequences, incremental diff patches, and reset teardown control sequences. |

## Priority Gap Plan

1. P2: Continue collecting terminal-version fixture captures to harden regression confidence across emulator updates.
2. P2: Expand Windows CI matrix for console input/resize interactions.
3. P3: Add performance benchmarks for renderer and decoder hot paths.
