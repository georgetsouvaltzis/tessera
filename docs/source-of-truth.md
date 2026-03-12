# TeaSharp API Source Of Truth

## Purpose

This document is the maintainer contract for TeaSharp's pre-public redesign.

Use it as the decision gate for:

- API design
- naming
- examples
- docs
- extension points
- internalization/removals

If a change conflicts with this document, the change is wrong unless this document is deliberately updated first.

## Product Position

TeaSharp is pre-public.
Breaking changes are allowed.

TeaSharp is not:

- a compatibility-first wrapper over the existing engine
- a Terminal.Gui clone
- a Spectre.Console clone
- a Bubble Tea port
- a Flutter/React-for-terminal clone

TeaSharp should be:

- a .NET-native framework for building state-driven terminal applications
- small on the public path
- strongly typed
- explicit
- extensible without leaking internals

## Non-Negotiable Agreements

### 1. Pre-Public Freedom

- optimize for the right long-term API
- remove or rename aggressively when needed
- do not preserve bad concepts for compatibility alone

### 2. TeaSharp-Owned Startup

TeaSharp should own startup.

Preferred entrypoints:

- `Tea.RunAsync(...)`
- `Tea.CreateBuilder()`
- `TeaApplicationBuilder`
- `TeaApplication`
- `TeaRuntimeOptions`

The framework must not take a hard dependency on Generic Host as its identity.

### 3. Small Default Surface

Normal apps should primarily live in:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`
- `TeaSharp.Styling`

Advanced/runtime seams belong away from the default path, under advanced namespaces such as `TeaSharp.Hosting`.

### 4. Normal Apps Must Not Learn Engine Vocabulary

The default app path must not require users to understand:

- `TeaSharp.Core.*`
- `IScreen`
- `InteractiveScreenModel`
- `ScreenComposer`
- `ComponentComposer`
- `InputRouter`
- `InputScope*`
- `ScreenRegionKey`
- terminal capability plumbing
- manual resize bookkeeping
- manual focus choreography

These may exist internally or as advanced escape hatches, but they are not the main story.

### 5. No Framework-Imposed App Architecture

TeaSharp must not imply:

- repository
- CQRS
- MVVM
- mediator
- unit of work

Those are application concerns.

### 6. Adaptability Stays

Custom widgets are a core requirement.

Extensibility must come from deliberate public contracts, not from leaked internals.

Default path:

- built-in controls
- simple composition
- typed messages/effects

Advanced path:

- custom controls/widgets
- low-level runtime seams
- advanced rendering and input behavior

## Public Design Rules

### 1. Intent First

The public API should describe:

- app state
- screen structure
- controls
- layout
- messages
- effects
- runtime/screen options

The public API should not primarily describe internal orchestration mechanics.

### 2. C#-Native Authoring

TeaSharp should feel like idiomatic C#, not like a foreign UI DSL.

Preferred tools:

- explicit object models
- object initializers
- `required` and `init` where appropriate
- small builders when they materially improve readability
- strong types over strings

### 3. No Dart/Flutter-Like Composition

This is a hard constraint.

Rejected patterns:

- `Rows(Auto(...), Fill(...))`
- `Columns(...)`
- nested static layout mini-languages
- deeply nested constructor trees as the primary authoring model

The ban is broader than static helpers.
If app composition reads like a declarative foreign mini-language instead of normal C#, it violates the agreement.

### 4. Prefer Explicit Screen Assembly

The default composition experience should move toward:

- screen objects with named sections/properties
- imperative screen builders
- object initializers with shallow composition depth

Acceptable shape:

- a screen/window object configured step by step
- named regions or sections owned by a builder object
- explicit composition statements that stay shallow

Unacceptable shape:

- large nested trees that force users to parse structure from indentation alone

### 5. Strong Typing Over Strings

Avoid stringly-typed primary models for:

- commands
- message kinds
- routing
- interaction modes
- semantic configuration

Strings are acceptable for user content, item labels, and optional identifiers, but not as the main control or interaction contract.

### 6. No Bool-Heavy Orchestration APIs

Public APIs should not expose complicated behavioral methods with many booleans or low-level knobs.

Prefer:

- typed options objects
- smaller object models
- enums
- separate focused abstractions

## Naming Rules

### 1. Simpler Name For Simpler Concept

- use short, descriptive nouns
- remove needless implementation suffixes from default-facing types

Examples:

- `ButtonComponent` -> `Button`
- `TextBlockComponent` -> `Label`
- `TextInputComponent` -> `TextInput`
- `TextAreaComponent` -> `TextArea`
- `DialogComponent` -> `Dialog`

### 2. One Naming System Per Family

Selection family must be coherent.

Examples:

- single-choice
- multi-choice
- searchable choice
- list browsing

Do not mix unrelated historical nouns for the same conceptual family.

### 3. No Ambiguous Or Colliding Names

Rejected examples:

- `UiKit.Layout` alongside `TeaSharp.Layout`
- namespace/type collisions
- multiple public nouns for the same beginner concept without clear role separation

### 4. Root Names Own The Main Story

If a root-level replacement exists, docs and examples should teach that first.

Legacy names may remain temporarily for migration or advanced scenarios, but they should not dominate discoverability.

### 5. One Concept, One Integration Style

Each control should have one primary integration model on the default path.

Avoid teaching multiple equal-status patterns such as:

- route input, then poll `TryConsume...`
- route input, then inspect state
- route input, then also subscribe to events

The default story must be singular and obvious.

## Layer Model

TeaSharp should have three clear layers.

### Layer 1. Default App Layer

Used by most consumers.

Includes:

- `TeaApp`
- `Tea`
- `TeaApplicationBuilder`
- `TeaRuntimeOptions`
- `Screen`
- `ScreenContext`
- root controls
- root layout model
- typed messages/effects

### Layer 2. Custom Control Layer

Used by advanced consumers writing widgets.

Includes:

- stable custom control contract
- render/input/layout contexts as needed
- enough hooks to build reusable controls without engine knowledge

### Layer 3. Hosting And Runtime Layer

Used rarely.

Includes:

- terminal adapters
- renderers
- decoder seams
- capability probes
- low-level runtime hooks

The deeper the layer, the farther it should be from the root namespace and from beginner docs.

## Custom Widget Contract

Design rule:

- a custom widget author should not need to understand the runtime engine

Implications:

- no required knowledge of screen region routing
- no required knowledge of input scopes
- no required knowledge of terminal protocols
- no required knowledge of internal focus engine mechanics

Extensibility should be explicit and stable.

## Documentation Rules

- the first example must teach the intended path
- starter docs must not teach advanced engine vocabulary
- examples must not contradict the source-of-truth API
- old-path examples, if kept, must be clearly marked advanced
- default docs/examples must stay inside `TeaSharp`, `TeaSharp.Controls`, `TeaSharp.Layout`, plus `TeaSharp.Components.Primitives` only when drawing primitives are required
- default docs/examples must not instantiate `*Component` types directly
- if an example needs advanced `*Component` types, the example must be marked advanced

## Current Drift To Correct

These are known deviations from the agreement and should be treated as active correction work.

### 1. The New Composition Path Is Still Too DSL-Like

Even after removing static helpers, the current default examples still rely heavily on nested layout object trees such as:

- `new DockLayout(new SplitLayout(new StackLayout(...)))`

That still violates the intent of the "no Dart-like composition" agreement.

### 2. Layout Authoring Still Leans On Tree Construction Instead Of Screen Assembly

The current root layout model is usable, but it is still more tree-oriented than builder-oriented.

The default authoring path should move toward shallower, more explicit screen construction.

### 3. The Stable Path Is Still Blurry

Some examples and docs still mix root controls with advanced `*Component` types.

That makes it unclear which path is:

- default
- advanced
- transitional only

This must be corrected in the examples and in IntelliSense-facing guidance.

### 4. Default Input Flow Still Feels Magical

The current `HandleScreenInput(...)` plus post-routing probing pattern is still framework knowledge.

If a new user has to learn routing order before they can use a button or input correctly, the default API is not finished.

### 5. Custom Widget Story Still Shows Engine DNA

Current examples of drift:

- `TeaApp : TeaSharp.Core.Abstractions.IScreen`
- `Control : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent`
- adapter-driven bridges visible in consumer-facing types

This is acceptable internally for now, but it should not remain the long-term consumer story.

### 6. Root Catalog Is Incomplete

Some important controls still require direct use of advanced `*Component` types.

That is acceptable temporarily, but the long-term path should reduce how often normal apps need advanced namespaces.

## Correction Plan

### Phase A. Freeze The Rules

- keep this document current
- reference it in future redesign work
- reject new examples or APIs that violate it

### Phase B. Replace Nested Screen Construction On The Default Path

Design and implement a more C#-native authoring model for normal apps.

Target direction:

- screen/window objects with named sections
- imperative builders for screen assembly
- shallow composition statements
- fewer giant nested layout expressions in examples

Possible forms:

- `ScreenBuilder`
- `Window`
- `ContentArea`
- `Sidebar`
- `Footer`
- `OverlayHost`

Exact API to be designed, but it must satisfy the non-negotiables above.

### Phase C. Remove Advanced Knobs From The Default Layout Surface

Default layout types should not expose advanced routing vocabulary such as:

- `regionKey`
- `focusable`
- `focusOnClick`
- `interceptsPointer`
- `layer`
- `onFocus`

These knobs should move behind advanced-only layers or advanced-only constructors/factories.

### Phase D. Rework The Default Input Model

Design a clearer default control integration story so users do not have to understand hidden routing order plus post-routing probing.

The long-term goal is:

- obvious control interaction semantics
- less framework magic in `Update(...)`
- less polling as the primary mental model

### Phase E. Rework Examples To Match The Corrected Model

- rewrite examples to stop teaching nested tree authoring as the main path
- keep advanced widget examples on the new startup model, but simplify their screen assembly too
- default examples must not instantiate advanced `*Component` types directly

### Phase F. Continue Narrowing Legacy Mechanism

- keep old composition/runtime APIs advanced-only
- remove additional leaked engine concepts when safe

### Phase G. Continue Promoting Root Controls

- move the most common advanced widgets behind clearer root-level names or wrappers where appropriate
- keep naming coherent across families

### Phase H. Decouple Consumer Contracts From Core Adapters

Over time, reduce visible consumer dependence on:

- `TeaSharp.Core.*`
- core interfaces implemented directly on root consumer types
- adapter-driven contracts visible in the root control story

Internal bridging is fine.
Consumer-facing design should not advertise the bridge.

## Review Checklist For Every API Change

Before accepting a public API or example change, ask:

1. Does this teach intent or mechanics?
2. Would a normal app author need to learn engine concepts here?
3. Does this read like normal C# or like a foreign DSL?
4. Is the naming the simplest clear naming available?
5. Is this the default layer, custom-control layer, or hosting layer?
6. Are we leaking an advanced concern into the root namespace?
7. If a new user copied this example, would they learn the right mental model?
8. Does this force users to learn hidden routing order?
9. Are we mixing root controls and advanced components in a beginner example?
10. Would a .NET developer feel they are writing normal C# app code, or TeaSharp's private engine language?

If any of these answers are wrong, redesign before merging.
