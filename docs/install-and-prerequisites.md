---
sidebar_label: Installation
---

# Install And Prerequisites

Install is intentionally small.

## Install package

```bash
dotnet add package Tessera
```

Most public apps only need this package to start.

## Optional project bootstrap

```bash
dotnet new console -n MyApp -f net10.0
cd MyApp
dotnet add package Tessera
```

## Environment requirements

- `.NET 10.0.103` SDK
- terminal with reliable ANSI/CSI support
  - Ghostty
  - iTerm2
  - Windows Terminal
  - macOS Terminal

:::tip Advanced package
`Tessera.Core` is the low-level runtime layer. Keep it out of the beginner path unless you are intentionally working on advanced runtime seams.
:::

## Default namespaces to import

Most app code should start with:

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;
```

Add `using Tessera.Styles;` when theme, color, or style work starts to matter.

## Next step

- starting from empty folder: [Quickstart (New App)](/docs/quickstart-new-app)
- integrating into an existing project: [Quickstart (Existing App)](/docs/quickstart-existing-app)
- then continue to [Your First App](/docs/first-app) and [Starter Examples](/docs/examples)
