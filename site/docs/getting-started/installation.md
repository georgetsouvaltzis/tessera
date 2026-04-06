---
title: Installation
---

Tessera currently targets `.NET 10`.

## Prerequisites

- .NET SDK `10.0.103` or later in the same feature band
- a CSI-capable terminal for richer pointer, focus, and paste behavior

## Add the package

```bash
dotnet add package Tessera
```

## Verify the environment

```bash
dotnet --version
```

For the current repo, the pinned SDK is `10.0.103`.

## Terminal notes

Tessera works best when the terminal supports modern input reporting:

- focus reporting
- bracketed paste
- mouse tracking
- ANSI styling

Legacy terminals can still run simpler apps, but advanced interaction may be unavailable.
