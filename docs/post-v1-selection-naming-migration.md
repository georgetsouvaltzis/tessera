# Post-V1 Selection Naming Migration Policy

## Purpose

Define a forward-only, non-breaking migration path from mixed selection payload names (`Current*` and `Selected*`) to one canonical naming convention.

This policy is intentionally explicit so V1 can ship without breaking consumers while still closing the naming gap.

## Canonical Contract

For list-like selection APIs, canonical public naming is:

- `PreviousIndex`
- `SelectedIndex`
- `PreviousItem` (or domain-specific typed equivalent)
- `SelectedItem` (or domain-specific typed equivalent)
- `SelectionChanged`

`Selected*` is the canonical term in docs, examples, and new API design.

## Phased Plan

| Phase | Policy | Breaking Risk |
|---|---|---|
| V1 | Keep compatibility names and aliases side by side (`Current*` + `Selected*` where present). Do not remove members. Mark `Selected*` as canonical in docs/spec/inventory. | None |
| V1.x | De-emphasize `Current*` in examples/docs/snippets and onboarding guidance. Keep runtime behavior and compatibility members intact. Add migration notes where relevant. | None |
| V2 | Apply planned cleanup after migration window: obsolete compatibility `Current*` members and/or remove them in the major-version cut according to final V2 API review. | Controlled major-version break |

## Migration Window Rule For V2 Cleanup

Before removing compatibility names in V2:

1. Keep `Selected*` aliases available throughout V1 and V1.x.
2. Publish migration guidance and examples that use only canonical `Selected*`.
3. Decide per type whether V2 should:
   - keep compatibility member with `[Obsolete]`, or
   - remove compatibility member in the major release.

Decision criteria:

- usage frequency in docs/examples and user reports
- ambiguity cost of dual naming
- maintenance cost and API clarity impact

## Current Scope Covered By This Policy

Event args with compatibility naming pressure include:

- `JsonTreeSelectionChangedEventArgs`
- `KeyValueListSelectionChangedEventArgs`
- `PropertyGridSelectionChangedEventArgs`
- `ValidationSelectionChangedEventArgs`
- `FileExplorerSelectionChangedEventArgs`
- `GroupedListSelectionChangedEventArgs<TGroup,TItem>`

The first four already have `Selected*` aliases and remain compatibility-bearing until V2 decisions.

## Non-Goals (For V1)

- No breaking renames/removals of public event payload members.
- No claim that all compatibility names are removed in current release lanes.
