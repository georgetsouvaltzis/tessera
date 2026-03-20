# Terminal Compatibility Evidence (Public V1)

Date: 2026-03-20

## Scope

This evidence covers the Public V1 terminal-font compatibility matrix targets:
- Ghostty
- iTerm2
- WezTerm
- Kitty
- Windows Terminal

## Verification Method

Method split used in this repo:
- real-host evidence: Ghostty
- deterministic tests + official specs: iTerm2, WezTerm, Kitty, Windows Terminal

Explicit host statement:
- iTerm2/WezTerm/Kitty/Windows Terminal binaries are not installed on this host.
- Verification for those terminals uses deterministic tests and official terminal specs.
- Ghostty has host evidence.

Host probe evidence:
- `echo $TERM` -> `xterm-ghostty`
- `command -v ghostty` -> `/Applications/Ghostty.app/Contents/MacOS/ghostty`
- `command -v wezterm|kitty|iterm2|wt` -> no result

## Evidence Matrix

| Terminal | Method | Evidence in repo | Result |
| --- | --- | --- | --- |
| Ghostty | Real host + deterministic tests | Host TERM evidence above; benchmark environment records `terminal: xterm-ghostty` in [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md); detector assertions in `TerminalCapabilityDetectorTests` (`ghostty` disables OSC 50). | Supported verification path present. |
| iTerm2 | Deterministic tests + official spec | `TerminalCapabilityDetectorTests` asserts iTerm2 profile support + OSC 50 disabled; renderer asserts `OSC 1337;SetProfile` lane and suppresses OSC 50 in `RendererBehaviorTests`; spec refs in [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md). | Capability-gated behavior verified. |
| WezTerm | Deterministic tests + official spec | `TerminalCapabilityDetectorTests` asserts OSC 50 disabled; spec refs in [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md). | Safe no-op fallback verified. |
| Kitty | Deterministic tests + official spec | `TerminalCapabilityDetectorTests` asserts OSC 50 disabled and no iTerm2 profile support; spec refs in [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md). | Safe no-op fallback verified. |
| Windows Terminal | Deterministic tests + official spec | `TerminalCapabilityDetectorTests` asserts OSC 50 disabled; spec refs in [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md). | Safe no-op fallback verified. |

## Behavior Contract Verified

- Capability gates are explicit:
  - `SupportsOsc50FontRequests`
  - `SupportsIterm2ProfileRequests`
- Renderer behavior is deterministic:
  - emits OSC 50 only when capability allows and profile lane is not preferred
  - prefers iTerm2 profile lane when supported and requested
  - sanitizes control characters for profile/font requests

## Supporting Test Coverage

- `tests/TeaSharp.Tests/TerminalCapabilityDetectorTests.cs`
- `tests/TeaSharp.Tests/RendererBehaviorTests.cs`
- `tests/TeaSharp.Tests/ScreenOptionsAdapterTests.cs`

