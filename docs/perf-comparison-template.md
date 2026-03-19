# TUI Performance Comparison Template

Use this template to compare TeaSharp against another TUI under a strict, repeatable protocol.

## Protocol (Strict)

1. Run all measurements on the **same machine** (same CPU governor, same power mode).
2. Use the **same terminal** and fixed dimensions (for example `120x40`).
3. Use the exact same **scenario** inputs and data volume for each framework.
4. Run the same binary mode (`Release`) and same iteration count.
5. Execute warmup runs before measured runs.
6. Use the same run count for each scenario (recommended: `N=20` measured runs).
7. Report results with the same format: `median`, `p95`, `alloc`.

## Scenarios

- Scenario ID: `TODO_SCENARIO_ID`
- Description: `TODO describe render/update interaction flow`
- Input fixture: `TODO path to fixture data`
- Terminal settings: `TODO TERM + COLS/ROWS`
- Timing method: `TODO command/tool used`

## Result Table (Fill In)

| Scenario | Framework | Runs | Median (ms) | p95 (ms) | alloc (MB) | Notes |
|---|---|---:|---:|---:|---:|---|
| `TODO_SCENARIO_ID` | TeaSharp | `TODO` | `TODO` | `TODO` | `TODO` | `TODO` |
| `TODO_SCENARIO_ID` | `TODO_COMPETITOR` | `TODO` | `TODO` | `TODO` | `TODO` | `TODO` |

## Reporting Rules

- Do not mix data from different terminals or machines.
- Include raw run outputs in appendices or artifacts.
- Mark any deviation from this template explicitly as `protocol-break`.
