---
title: Messages and Effects
---

Tessera uses typed runtime messages and explicit effects.

## Common messages

- `KeyPressed`
- `KeyReleased`
- `WindowResized`
- `PointerInput`
- `FocusChanged`
- `Pasted`

## Common effects

- `TesseraEffects.Quit`
- `TesseraEffects.Interrupt`
- `TesseraEffects.Emit(...)`
- `TesseraEffects.Batch(...)`
- `TesseraEffects.Sequence(...)`
- `TesseraEffects.Periodic(...)`

## Practical split

- use messages for state transitions
- use effects for runtime work
- use control events when a control owns the interaction
