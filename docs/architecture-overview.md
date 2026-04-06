# TeaSharp Architecture Overview

This document is the shortest contributor-facing map of the TeaSharp codebase.

## Product Layers

### 1. Default Public Layer

Normal app authors should live here:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`
- `TeaSharp.Styles`

This is the public product identity.

### 2. Advanced Hosting Layer

Use this only when you need custom runtime seams:

- `TeaSharp.Hosting`

This layer remains public, but it is not the beginner path.

### 3. Low-Level Runtime Layer

- `TeaSharp.Core`

This is the engine-adjacent layer. It should not leak into normal onboarding, README snippets, or flagship public examples.

## App Lifecycle

The default app contract is:

1. `Initialize()` can return the first effect
2. `Update(Message)` applies state transitions and side effects
3. `Build(ScreenContext)` returns the next screen tree

Built-in controls route handled input before `Update(...)` sees the remaining messages.

## Composition Model

TeaSharp composes screens explicitly:

- `Screen.Build(...)`
- controls from `TeaSharp.Controls`
- layouts from `TeaSharp.Layout`

The framework should read like explicit screen composition in C#, not like a custom nested DSL.

## Theming Model

TeaSharp theming is semantic-token based:

- global theme
- control-type defaults
- instance overrides
- state overrides

The full token and override map lives in [theme-system.md](theme-system.md).

## Where To Work

- public API and controls: `src/TeaSharp`
- low-level runtime/engine work: `src/TeaSharp.Core`
- regression and contract tests: `tests/TeaSharp.Tests`
- runtime/integration flows: `tests/TeaSharp.IntegrationTests`
- public examples: `examples`
- product and contributor docs: `docs`

## Contributor Guidance

When changing public behavior:

1. update tests
2. update public docs in the same slice
3. keep the default public path simpler, not more clever
4. avoid moving public guidance toward `TeaSharp.Core`
