---
sidebar_label: Introduction
title: Introduction
---

**NeonTUI** is a modern, declarative TUI (terminal user interface) library for .NET. It brings reactive, component-based UI to your console applications — think React, but rendered with Unicode glyphs and ANSI colors.

## Why NeonTUI?

- **Declarative components** — describe what you want, not how to draw it
- **First-class async** — built on `Task` and `IAsyncEnumerable`
- **Themed by default** — beautiful out of the box, fully customizable
- **Cross-platform** — works on Windows Terminal, iTerm, Alacritty, Kitty, GNOME Terminal

## At a glance

```csharp
using NeonTui;

var app = new App(new VStack {
    new Text("Hello, neon world!").Bold().Color(Color.Pink),
    new Button("Press me", () => Console.Beep()),
});

await app.RunAsync();
```

That's a complete, interactive program.
