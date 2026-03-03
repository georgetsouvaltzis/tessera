# TeaSharp - Software Design Document (SDD)

## 1. Overview
TeaSharp is a cross-platform .NET 10 terminal UI stack inspired by Bubble Tea + Ultraviolet.

Single repository. Layered packages:

- `TeaSharp.Core`: terminal primitives + program runtime.
- `TeaSharp`: ergonomic framework surface over core.
- `TeaSharp.Examples`: runnable sample apps.
- `TeaSharp.Tests`: behavioral regression suite.

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

3. `Input`
- `EventDecoder`: bytes -> typed messages.
- `TerminalReader`: stream pump + decoder loop.

4. `Rendering`
- `IProgramRenderer` abstraction.
- `AnsiDiffRenderer`: minimal line-diff ANSI renderer.
- `NullRenderer`: test/daemon mode.

5. `Terminal`
- `ITerminalAdapter` abstraction.
- `ConsoleTerminalAdapter`: OS integration and console mode management.

6. `Commands`
- `Commands` static helpers: `Quit`, `Interrupt`, `Batch`, `Sequence`, `Tick`, `Every`.

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
- Program includes interval-based terminal size polling for consistent cross-platform resize updates.

## 7. API Contracts (Phase 1)

- `IModel Init/Update/View`.
- `Command` returns optional `IMessage`.
- Messages include:
  - lifecycle: `QuitMsg`, `InterruptMsg`, `CommandErrorMsg`.
  - terminal: `WindowSizeMsg`.
  - input: `KeyPressMsg`, `KeyReleaseMsg`, `Paste*Msg`, `Focus*Msg`, `UnknownInputMsg`.
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

## 10. Iteration Roadmap

Phase 1 (current):
- minimal runtime + decoder + renderer + tests + example.

Phase 2:
- richer ANSI/CSI parser coverage.
- improved renderer diff fidelity (cell-level).
- mouse + focus + clipboard protocol handling.

Phase 3:
- capability negotiation.
- terminfo integration.
- performance tuning and profiling benchmarks.

## 11. Repo Profile Snapshot

- SDK pinned: `10.0.103` (`global.json`).
- TFM: `net10.0` for all projects.
- Solution entrypoint: `TeaSharp.slnx`.

## 12. Parity Tracking

- Bubble Tea parity tracking lives in `docs/parity-matrix.md`.
