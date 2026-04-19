---
sidebar_label: Installation
---

# Install And Prerequisites

This page gets you from an empty console app to a Tessera-ready project.

## What you need

- `.NET 10.0.103`
- a terminal with reliable ANSI/CSI support
  - Ghostty
  - iTerm2
  - Windows Terminal
  - macOS Terminal

Tessera is library-first. You do **not** need ASP.NET hosting, a DI container, or Generic Host wiring for the normal path.

## Create a new project

```bash
dotnet new console -n MyApp -f net10.0
cd MyApp
```

## Add Tessera

```bash
dotnet add package Tessera
```

That is the only package most app authors should need to start.

:::tip Advanced package
`Tessera.Core` is the low-level runtime layer. Keep it out of the beginner path unless you are intentionally working on advanced runtime seams.
:::

## Default namespaces

Most app code should start with:

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;
```

Add `using Tessera.Styles;` when theme, color, or style work starts to matter.

## Recommended runtime defaults

Most real apps want these screen/runtime basics:

- alternate screen enabled
- window title set
- focus reporting enabled
- bracketed paste enabled
- mouse tracking enabled
- single-click activation for pointer-friendly apps

The configured startup sample in [first-app.md](first-app.md) includes those defaults.

## Verify your environment

Before you build your own app, make sure:

- `dotnet --version` reports `10.0.103`
- your terminal can redraw cleanly without tearing or broken cursor state
- copy/paste, focus changes, and pointer motion work in your environment

If any of those feel unreliable, check [terminal-font-capability-matrix.md](terminal-font-capability-matrix.md) and [troubleshooting.md](troubleshooting.md).

## Next step

Build the sample in [first-app.md](first-app.md). That page explains the minimal public app shape in one file.
