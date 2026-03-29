---
title: Control Overrides
---

TeaSharp exposes typed visual hooks instead of forcing raw ANSI strings everywhere.

## Common override patterns

- `TitleStyle` / `FocusedTitleStyle`
- `BorderStyleText` / `FocusedBorderStyleText`
- `FocusMarker` / `ShowFocusMarker`
- row, value, selected, and hovered styles per control family

## Example

```csharp
var choice = new Choice
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
    Glyphs = new DropdownGlyphSet("▾", "▴", ">", "✓"),
};
```

Use theme defaults first, then add instance-level polish where the app needs it.
