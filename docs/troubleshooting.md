---
sidebar_label: Troubleshooting
---

# Troubleshooting

Use this page when the framework compiles but the app or terminal behavior still feels wrong.

## `dotnet add package Tessera` cannot find the package

Check:

- you are using a `.NET 10` project
- your NuGet source can see the package
- the package ID is exactly `Tessera`

If you are evaluating from source before package publication reaches your feed, run the repo examples directly until the package is available in your environment.

## The terminal redraws badly or leaves the terminal in a weird state

Check:

- `AltScreen`
- your terminal choice
- whether the terminal supports the escape sequences you expect

Recommended first terminals:

- Ghostty
- iTerm2
- Windows Terminal
- macOS Terminal

If terminal-specific behavior is the problem, use [terminal-font-capability-matrix](/docs/terminal-font-capability-matrix).

## Mouse, hover, or click interaction does not work

Check both runtime and screen settings:

```csharp
runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
runtime.Screen = new ScreenOptions
{
    MouseTracking = MouseTrackingMode.AllMotion,
    EnableFocusReporting = true
};
```

Remember:

- pointer activation policy lives on `TesseraRuntimeOptions`
- mouse tracking lives on `ScreenOptions`

## Paste events are missing

Enable bracketed paste:

```csharp
runtime.Screen = new ScreenOptions
{
    EnableBracketedPaste = true
};
```

Then handle `Pasted`, `PasteStarted`, or `PasteEnded` if your app cares about those messages directly.

## Focus changes are not showing up

Enable focus reporting:

```csharp
runtime.Screen = new ScreenOptions
{
    EnableFocusReporting = true
};
```

Then use `FocusChanged` in `Update(...)` or inspect `ScreenContext.HasFocus` in `Build(...)`.

## Global hotkeys do not behave the way I expect

Important rule:

- key messages still continue to `Update(...)` even when a control handles them
- handled pointer and paste messages can be swallowed by controls

So global keyboard shortcuts belong in `Update(...)`.

## My app is getting dense and `Build(...)` is becoming unreadable

Refactor by screen regions instead of inventing a second architecture:

- keep one top-level `Screen.Build(...)`
- move header/body/footer/side regions into helper methods
- keep state transitions in `Update(...)`

Use [layout-and-screen-composition](/docs/layout-and-screen-composition) for the preferred composition style.

## I am not sure whether I need `Tessera.Core`

Most likely, you do not.

Stay on:

- `Tessera`
- `Tessera.Controls`
- `Tessera.Layout`
- `Tessera.Styles`

Reach for `Tessera.Core` only when you are intentionally working at the runtime layer.

## I need exact public names, not guidance

Open:

- [API Reference](/docs/api-reference)
- [public-api-inventory](/docs/public-api-inventory)

## Still stuck?

Read:

- [faq](/docs/faq)
- [support](/support)

If the issue is example-specific, compare against the starter ladder first and then the flagship apps.
