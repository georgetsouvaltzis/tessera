---
sidebar_label: Recipes Overview
---

# Recipes Overview

These recipes are the shortest path from “I understand the concepts” to “I can assemble a real screen.”

Use them when you do not want a full example app, but you also do not want to reverse-engineer an advanced showcase first.

## Recipe lanes

| Recipe lane | Use it for | Page |
| --- | --- | --- |
| App shells | minimal startup, quit handling, shell framing, status bars | [recipes-app-shells.md](recipes-app-shells.md) |
| Effects and refresh | button-to-message flow, periodic updates, notifications | [recipes-effects-and-refresh.md](recipes-effects-and-refresh.md) |
| Data and workspaces | rails, grids, inspectors, denser record surfaces | [recipes-data-and-workspaces.md](recipes-data-and-workspaces.md) |

## How to use the recipes

1. start from the smallest recipe that matches your app shape
2. keep your app state in `TesseraApp`
3. promote local widget events into messages only when the screen starts getting real
4. move into the flagship examples only after the recipe feels coherent

## What these pages are not

These pages are not the complete API inventory.

For exact types, helper records, glyph sets, and options, use:

- [api-reference.mdx](api-reference.mdx)
- [public-api-inventory.md](public-api-inventory.md)
- [theme-system.md](theme-system.md)
