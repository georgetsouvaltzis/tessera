---
sidebar_label: Screen & Layout
---

# Screen And Layout Composition

Tessera composes screens explicitly in C#. The default path is not a nested mini-language; it is ordinary code assembling a screen tree.

## The default path: `Screen.Build(...)`

For most apps, start with:

```csharp
return Screen.Build(window =>
{
    window.Padding(1);
    window.Footer(1, _status);
    window.Body(body => body.Center(_refreshButton, width: 24, height: 3));
});
```

That imperative builder is the normal shell-style composition path.

## Window sections

`Screen.Build(...)` gives you named regions:

- `Header(...)`
- `HeaderRow(...)`
- `Footer(...)`
- `Left(...)`
- `Right(...)`
- `Body(...)`
- `Overlay(...)`

This keeps larger screens shallow and readable.

## Common composition helpers

Inside `Body(...)`, `Header(...)`, or panel content, use the content builders:

- `Center(...)`
- `Panel(...)`
- `Row(...)`
- `Column(...)`
- `Use(...)`

Example:

```csharp
return Screen.Build(window =>
{
    window.Padding(1);
    window.Left(28, panel =>
    {
        panel.Title("Queues");
        panel.Border(BorderStyle.SingleLine);
        panel.Padding(1);
        panel.Content(content => content.Center(_queueButton, width: 20, height: 3));
    });
    window.Body(body => body.Row(row =>
    {
        row.Weighted(2, content => content.Center(_ordersButton, width: 24, height: 3));
        row.Weighted(1, content => content.Panel(panel =>
        {
            panel.Title("Inspector");
            panel.Border(BorderStyle.SingleLine);
            panel.Padding(1);
            panel.Content(detail => detail.Center(_detailsButton, width: 18, height: 3));
        }));
    }));
    window.Footer(1, _status);
});
```

## The main layout types

Tessera exposes these layout primitives on the public path:

- `WindowLayout`
  - default shell-style screen with named sections
- `RowLayout`
  - horizontal arrangement
- `ColumnLayout`
  - vertical arrangement
- `CenterLayout`
  - centered content with optional explicit width and height
- `PanelLayout`
  - grouped content with title, border, padding, and margin

Advanced composition types also exist:

- `OverlayLayout`
- `DockLayout`
- `SplitLayout`

Those are useful, but the imperative builder is the better default story for most app authors.

## Other ways to create a screen

`Screen` supports several entry points:

- `Screen.Build(...)`
  - best default for non-trivial app screens
- `Screen.From(control)`
  - good for a single root control
- `Screen.From(layout)`
  - useful when you are building layout objects directly
- `Screen.From(string)`
  - simplest text screen
- `Screen.From(canvas)`
  - direct drawing path

## When to use which approach

- `Screen.Build(...)`
  - default app shells, windows, workspaces, and multi-region screens
- `Screen.From(control)`
  - simple views where one control is the root
- `Screen.From(layout)`
  - layout-heavy code where object initializers are clearer than builders
- `Screen.From(canvas)`
  - advanced render-only scenarios

## Rule of thumb

If a screen is getting dense, prefer:

- named window sections
- one screen-level `Build(...)`
- smaller helper methods per region

That is easier to maintain than one giant nested tree.

## Next step

- lifecycle and message flow: [app-model.md](app-model.md)
- runtime configuration: [runtime-and-screen-options.md](runtime-and-screen-options.md)
- real examples: [examples.md](examples.md) and [showcase.md](showcase.md)
