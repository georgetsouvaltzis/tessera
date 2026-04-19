# Getting Started With Tessera

This is the primary public onboarding path for Tessera.

## One-line install

```bash
dotnet add package Tessera
```

Then continue immediately to [Your First App](/docs/first-app).

## Recommended order

1. [Introduction](/docs/overview)
2. [Installation](/docs/install-and-prerequisites)
3. [Your First App](/docs/first-app)
4. [Starter Examples](/docs/examples)
5. [Flagship Evaluation](/docs/showcase)

## Which project should you start with

- first contact: `examples/HelloWorld`
- first interactive form: `examples/CounterForm`
- first denser shell: `examples/WorkspaceApp`
- product-pressure validation: `examples/GitConsole`, `examples/OpsWatch`, `examples/DataWorkbench`

If you are new to Tessera, follow [Beginner Track](/docs/beginner-track).  
If your team already validated the basics, move to [Advanced Track](/docs/advanced-track).

## Core model in one minute

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;

var app = TesseraApplication.CreateBuilder()
    .UseApp<MyApp>()
    .Build();

await app.RunAsync();
```

- derive app behavior from `TesseraApp`
- render screens with `Screen.Build(...)`
- handle state transitions in `Update(Message)`
- return effects only when needed

## What to read next by question

- "How do I get running quickly?" -> [Installation](/docs/install-and-prerequisites), [Your First App](/docs/first-app)
- "Which widgets should I use?" -> [Widget Reference](/docs/widget-reference), [Widgets Overview](/docs/controls-overview)
- "How do I structure real screens?" -> [App Model](/docs/app-model), [Screen & Layout](/docs/layout-and-screen-composition)
- "How do I configure runtime/theming?" -> [Runtime & Screen Options](/docs/runtime-and-screen-options), [Theme System](/docs/theme-system)
- "How do I solve common integration tasks?" -> [Recipes](/docs/recipes), [Troubleshooting](/docs/troubleshooting)
- "Where are exact API names?" -> [API Reference](/docs/api-reference), [Public API Inventory](/docs/public-api-inventory)
