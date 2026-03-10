# TeaSharp Refactor Plan

## Goals

- make component composition explicit and testable
- remove reflection-driven behavior from public interaction paths
- split oversized modules by responsibility instead of by historical growth
- keep library consumers on a smaller, clearer surface area

## Phase 0: Baseline and API Inventory

Status: in progress

- inventory the public API into stable, advanced, and engine-detail tiers
- document the target stable surface before deeper package-boundary changes
- use the inventory to drive later internalization and namespace cleanup
- start separating stable app-hosting setup from advanced runtime wiring (`TeaProgramOptions` vs `ProgramOptions`)

## Phase 1: Interaction Contracts

Status: done

- introduce `IFocusableComponent`
- introduce `IInteractiveComponent`
- introduce `KeyboardRoutingMode`
- change `ComponentComposer` to focused-slot keyboard routing by default
- remove reflection-based focus discovery and focus mutation
- move `LayoutContainerComponent` into its own file and align its routing model with `ComponentComposer`

## Phase 2: Test Infrastructure

Status: done

- convert `tests/TeaSharp.Tests` from a custom executable harness into a real NUnit test project
- preserve existing case-based tests through a thin NUnit adapter
- remove unstable example-app coupling from the unit-test project
- add routing regressions for focused-only vs broadcast keyboard dispatch

## Phase 3: File Splitting

Status: next

- split `PrebuiltWidgets.cs` into one widget per file
- split `UiKit.cs` into layout, primitives, tables, forms, overlays
- split `AdvancedPrebuiltWidgets.cs` and `ProductivityPrebuiltWidgets.cs` by widget family
- keep the public namespace stable while reducing per-file responsibility

## Phase 4: API Simplification

Status: in progress

- add `*Options` records for high-churn widgets
- prefer small constructor overloads or static factory methods for common setups
- standardize shared knobs: focus, borders, interaction profile, key bindings, state palette
- move example-only composition code out of library-facing docs
- added `DropdownOptions` and `ComboboxOptions` plus example/test coverage to start normalizing constructor-driven setup
- clone `WidgetInteractionProfile` on component assignment/constructor ingress so shared defaults are safe and component state stays isolated

## Phase 5: Stable Integration Fixtures

Status: in progress

- replace tests coupled to mutable showcase/example programs with dedicated fixture apps
- keep integration fixtures intentionally small and stable
- let examples optimize for demonstration while fixtures optimize for regression coverage
- added a dedicated counter fixture app under `tests/` and rewired integration coverage away from `examples/TeaSharp.Examples`

## Phase 6: Package Quality

Status: in progress

- add package metadata for the shipping library projects
- emit XML documentation files for the public library surface
- document the stable entrypoints first so IntelliSense teaches the recommended path
- keep CLI/test projects out of pack output unless they are intentionally published
- make deterministic build/analyzer policy explicit, keep compiler/package warnings strict, and stage analyzer enforcement incrementally instead of flipping the full backlog to errors at once
