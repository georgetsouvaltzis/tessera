# TeaSharp - Software Design Document (SDD)

## 1. Overview
TeaSharp is a cross-platform .NET 10 terminal UI stack inspired by Bubble Tea + Ultraviolet.

Single repository. Layered packages:

- `TeaSharp.Core`: terminal primitives + program runtime.
- `TeaSharp`: ergonomic framework surface over core.
- `TeaSharp.Cli`: developer tooling (`dotnet run --project src/TeaSharp.Cli -- wizard`) for scaffold generation.
- `TeaSharp.Examples`: runnable sample apps.
- `TeaSharp.Tests`: lightweight in-repo behavioral regression suite.
- `TeaSharp.IntegrationTests`: NUnit integration tests (`dotnet test`) for UX contracts and end-to-end message routing.

Primary goal: deterministic message-driven TUI runtime with portable terminal behavior on macOS, Linux, and Windows.

## 2. Goals

- Cross-platform runtime on `net10.0`.
- Bubble Tea-like programming model: `Init -> Update -> View`.
- Async effect model via commands (`Command`).
- Renderer with incremental redraw strategy and ANSI escape support.
- Input pipeline that decodes terminal bytes into structured messages.
- Testability without a real terminal (fake adapter + no-op renderer).

## 3. Non-Goals (Phase 1)

- Full parity with all Ultraviolet/Bubble Tea protocol edge cases.
- Complete ECMA-48 coverage.
- Full terminfo database integration.
- Maximum-performance cell-level diff parity with UV renderer.

## 3.1 Safety Constraint

- TeaSharp forbids `unsafe` blocks/project settings in all first-party projects.
- Native interop must remain safe managed P/Invoke signatures.

## 4. Architecture

### 4.1 Layers

1. `Abstractions`
- `IModel`, `IMessage`, `View`, `UpdateResult`, `Command`.

2. `Application`
- `TeaProgram` event loop, command scheduling, filtering, rendering orchestration.
  - adaptive frame pacing (`ProgramOptions.AdaptiveFramePacing`) for burst message coalescing.
  - recoverable command exception hook (`ProgramOptions.RecoverCommandException`) before `CommandErrorMsg` fallback.

3. `Input`
- `EventDecoder`: bytes -> typed messages.
- `TerminalReader`: stream pump + decoder loop.

4. `Rendering`
- `IProgramRenderer` abstraction.
- `AnsiDiffRenderer`: frame cell-buffer ANSI renderer with row+cell run diffing.
- `NullRenderer`: test/daemon mode.

5. `Terminal`
- `ITerminalAdapter` abstraction.
- `ConsoleTerminalAdapter`: OS integration and console mode management.
- `TerminalCapabilityDetector` + `TerminalCapabilityProfile`: environment + `infocmp`-enriched feature gating for renderer VT modes.
  - explicit capability overrides via `TEASHARP_CAPS` (`focus|mouse|paste|sync|decrpm` boolean flags).

6. `Commands`
- `Commands` static helpers: `Quit`, `Interrupt`, `Batch`, `Sequence`, `Tick`, `Every`.

7. `Components`
- `TeaSharp.Components`: deterministic canvas primitives (`Rect`, `Canvas`, `Widgets`) with selectable text pipeline (`CanvasTextMode.Fast` / `CanvasTextMode.GraphemeAware`) and configurable border styles, component composition contracts (`ICanvasComponent`, `IStatefulComponent`, `ComponentComposer`), chart primitives (`Charts`, `LineChartComponent`, `BarChartComponent`) with optional axes/legend/scale options, dashboard widgets (`GaugeComponent`, `StatsCardComponent`, `MiniLogComponent`), reusable UI kit (`Layout`, `UiWidgets`, tabs/accordion/table/forms/toast/modal components), and a prebuilt widget layer (`Label`, `Button`, `TextInput`, `TextArea`, `List`, `Table`, `ProgressBar`, `Tabs`, `Modal/Dialog`, `StatusBar`, `LogViewer`, `LayoutContainer`).

8. `Styles`
- `TeaSharp.Styles`: composable ANSI style model (`TeaStyle`, `AnsiColor`) for foreground/background and text attributes.

9. `Widgets`
- `TeaSharp.Widgets`: stateful widget models (`ViewportModel`, `TextInputModel`, `ListModel<T>`) with reusable keymaps/help.
  - viewport gutter/highlight support (`ShowLineNumbers`, `HighlightVisualLine`).
  - text input multiline editing mode.
  - tracked async list loader orchestration (`ReloadAsync`, `AppendAsync`) with stale-load cancellation.

### 4.2 Core Data Flow

1. Program starts and prepares terminal.
2. Initial size message emitted.
3. `model.Init()` command scheduled.
4. Input reader emits decoded messages (or console key events via `Console.ReadKey` fallback).
5. Event loop applies filter, handles internal control messages, calls `Update`.
6. Background size polling emits `WindowSizeMsg` when dimensions change.
7. Returned command gets scheduled.
8. `View` renders via active renderer.
9. Shutdown restores terminal state.

## 5. Concurrency Model

- Single consumer event loop via `Channel<IMessage>`.
- Commands execute asynchronously and enqueue returned messages.
- `Batch` executes concurrently.
- `Sequence` executes serially.
- Cancellation propagated through linked CTS.

## 6. Portability Strategy

### 6.1 Windows

- Enable VT input/output through `SetConsoleMode` flags.
- Preserve and restore original console modes.

### 6.2 Unix-like

- Use standard input/output streams and console metadata.
- If stdio is redirected but a controlling TTY exists, bind input/output to `/dev/tty`.
- Prefer direct termios (`tcgetattr/tcsetattr`) raw-mode setup on Unix.
- Enter `stty raw -echo` while the program runs, then restore the saved terminal state on shutdown.
- Probe terminal mode after setup; fallback to explicit `-icanon min 1 time 0 -echo` if needed.
- If raw mode is still unavailable, input path falls back to `Console.ReadKey(intercept: true)` for non-echo key handling.
- Program uses signal-assisted resize checks on Unix-like systems (`SIGWINCH`) plus interval-based polling as a cross-platform fallback.
- Renderer mode toggles (`focus`, `mouse`, `bracketed paste`, `synchronized updates`) are gated by a detected `TerminalCapabilityProfile` and can be overridden via `ProgramOptions.TerminalCapabilities`.
- On Unix-like systems, `TerminalCapabilityDetector` enriches environment heuristics with best-effort `infocmp -x` capability probing.

## 7. API Contracts (Phase 1)

- `IModel Init/Update/View`.
- `Command` returns optional `IMessage`.
- Messages include:
  - lifecycle: `QuitMsg`, `InterruptMsg`, `CommandErrorMsg`.
  - terminal: `WindowSizeMsg`.
  - input: `KeyPressMsg`, `KeyReleaseMsg`, `Paste*Msg`, `Focus*Msg`, `MouseMsg` + typed variants (`MouseClickMsg`, `MouseReleaseMsg`, `MouseMotionMsg`, `MouseWheelMsg`), `ModeReportMsg`, `TerminalCapabilitiesMsg` (startup + runtime refinements), `UnknownInputMsg`.
  - command meta: `BatchMsg`, `SequenceMsg`, `TickMsg`.

## 8. Error Handling

- Command exceptions converted to `CommandErrorMsg` by default.
- Interrupts map to `TeaProgramInterruptedException`.
- External cancellation maps to `OperationCanceledException`.

## 9. Test Plan

Behavior tests cover:

- init-command driven exit.
- external `Send` + quit behavior.
- `Batch` completion semantics.
- `Sequence` ordering.
- command timer (`Tick`) path.
- filter-based message suppression.
- deterministic renderer snapshots (first frame control sequences, incremental patches, reset teardown).
- styled rendering tests (SGR parsing, style-only diff patches).
- stateful widget tests (viewport scroll/wrap, text input edits/submission, list filter/paging, keymap help rendering).
- component tests (table/card primitives, chart rendering, component composer dispatch).
- UI-kit tests (layout helpers, border variants, tabs/table/forms/modal interactions).
- prebuilt widget tests (render + interaction contracts for label/button/input/textarea/list/table/progress/status/log/dialog/layout).
- protocol fixture tests (terminal-behavior fixtures for Ghostty/iTerm2/Apple Terminal style key/paste/focus sequences).
- integration tests (NUnit) for workspace UX contract (`:` enter command mode, `esc` exit, pane focus routing, and showcase modal/toast hotkeys).

## 10. Iteration Roadmap

Phase 1 (current):
- minimal runtime + decoder + renderer + tests + example.

Phase 2:
- richer ANSI/CSI parser coverage.
- improved renderer diff fidelity (cell-level).
- mouse + focus + clipboard protocol handling.

Phase 3:
- deeper capability negotiation (beyond mode-report probing).
- terminfo integration.
- performance tuning and profiling benchmarks.

## 11. Repo Profile Snapshot

- SDK pinned: `10.0.103` (`global.json`).
- TFM: `net10.0` for all projects.
- Solution entrypoint: `TeaSharp.slnx`.

## 12. Parity Tracking

- Bubble Tea parity tracking lives in `docs/parity-matrix.md`.
- Component drawing notes live in `docs/components.md`.
- Custom extension guide lives in `docs/custom-components.md`.
- ANSI style notes live in `docs/styles.md`.
- Stateful widget notes live in `docs/widgets.md`.
- Wizard scaffolding usage lives in `docs/wizard.md`.
