---
title: Messages and Effects
---

TeaSharp uses typed runtime messages and explicit effects.

## Common messages

- `KeyPressed`
- `KeyReleased`
- `WindowResized`
- `PointerInput`
- `FocusChanged`
- `Pasted`

## Common effects

- `TeaEffects.Quit`
- `TeaEffects.Interrupt`
- `TeaEffects.Emit(...)`
- `TeaEffects.Batch(...)`
- `TeaEffects.Sequence(...)`
- `TeaEffects.Periodic(...)`

## Practical split

- use messages for state transitions
- use effects for runtime work
- use control events when a control owns the interaction
