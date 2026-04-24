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

- `.NET 10` SDK (from `global.json`)
- terminal with reliable ANSI/CSI support
  - Ghostty
  - iTerm2
  - Windows Terminal
  - macOS Terminal

:::tip Advanced runtime lane
`Tessera.Core` namespaces are the low-level runtime layer, shipped inside the same `Tessera` package. Most beginner apps stay in the primary `Tessera` namespaces.
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
