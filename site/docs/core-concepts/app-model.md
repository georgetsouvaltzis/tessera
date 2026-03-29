---
title: App Model
---

TeaSharp keeps the default app shape small.

## Core contract

- derive from `TeaApp`
- optionally return startup work from `Initialize()`
- handle messages in `Update(Message)`
- render the current frame from `Build(ScreenContext)`

## Design intent

- normal apps stay in `TeaSharp`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`
- built-in controls handle their own interaction first
- global hotkeys and app state transitions stay in `Update(...)`
- advanced hosting seams exist, but they are not the onboarding path

## Practical rule

If you can build the app without importing `TeaSharp.Core.*`, you are on the intended public path.
