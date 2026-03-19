# TeaSharp Public API Guidelines

This document is the implementation policy for a C#-first TeaSharp API.

## Audience And Layers

TeaSharp supports two intentional product layers:

- `TeaSharp`: primary app-authoring surface for most C# developers building TUIs.
- `TeaSharp.Core`: low-level product for expert/runtime-driven scenarios.

Advanced host seams (`TeaSharp.Hosting`) remain supported, but they are not the beginner path.

## C#-First Rules

Use familiar .NET patterns by default:

- explicit object models, object initializers, and strongly typed options
- `EventHandler` / `EventHandler<TEventArgs>` for control notifications
- async APIs with `Async` suffix and `CancellationToken` as the last optional parameter
- immutable message payloads (records) for application message flow
- `IAsyncDisposable` for runtime/terminal resources

Avoid framework-specific patterns when BCL conventions already solve the problem.

## Canonical Startup Pattern

Support two startup lanes:

- minimal: `await Tea.RunAsync(new App());`
- configured: `Tea.CreateBuilder().ConfigureServices(...).UseApp<TApp>().ConfigureRuntime(...).Build()`

`UseApp<TApp>()` should activate app types through DI so constructor injection works without forcing Generic Host.

## Canonical App Pattern

Default integration model for app code:

1. controls raise events
2. app posts domain messages with `Post(...)` when state changes should flow through the state machine
3. `Update(...)` applies state transitions and returns effects
4. `Build(...)` returns the next screen

For tiny demos, direct event mutation is acceptable. Production examples should prefer message-driven updates.

## Canonical Composition Pattern

Default composition path:

- `Screen.Build(...)` + `WindowBuilder`
- root controls from `TeaSharp.Controls`
- root layouts from `TeaSharp.Layout`

Alternative composition surfaces may remain public for advanced scenarios, but docs and starter examples should teach the default path first.

## Boundary Rules

- Normal app examples should not import `TeaSharp.Core.*`.
- Public docs should use `TeaSharp.Styles` (not legacy namespace names).
- Runtime knobs for advanced hosting should live under `TeaSharp.Hosting` discoverability, not the default path.

## Review Checklist

Before merging API/docs/example changes:

- Is the beginner path still one obvious path?
- Does this follow idiomatic C# and .NET conventions?
- Does this introduce a second equal-status integration style?
- Does this leak low-level runtime vocabulary into default app guidance?
