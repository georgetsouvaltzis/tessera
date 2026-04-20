---
title: "SearchBox"
sidebar_label: "SearchBox"
---

# `SearchBox`

**Family:** Navigation & Workflow  
**Namespace:** `Tessera.Controls`

Use `SearchBox` when this interaction is the best match for your screen workflow.

## When to use

- Inline search with explicit next/prev match navigation.
- You need query state as a first-class part of the shell.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var search = new SearchBox
{
    Title = "Search slice",
    Placeholder = "entity, owner, region",
    MatchCount = 3,
    CurrentMatch = 1
};

var status = new StatusBar { LeftText = "Search ready" };
search.QueryChanged += (_, e) => status.LeftText = $"Query: {e.Query}";
search.NavigationRequested += (_, e) => status.LeftText = $"Navigate: {e.Direction}";

return Screen.Build(window =>
{
    window.Footer(1, status);
    window.Body(body => body.Center(search, width: 68, height: 4));
});
```

## Common pitfalls

- Debounce expensive searches; avoid blocking UI loop per keystroke.
- Show match count and current index so navigation is predictable.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `CurrentMatchIndex` | `int?` |
| `DisabledNavigationLabelStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MatchCounterStyle` | `TesseraStyle` |
| `MatchHighlightStyle` | `TesseraStyle` |
| `NavigationLabelStyle` | `TesseraStyle` |
| `NextLabel` | `string` |
| `Padding` | `Thickness` |
| `PlaceholderTextStyle` | `TesseraStyle` |
| `PreviousLabel` | `string` |
| `ShowFocusMarker` | `bool` |
| `ShowNavigationLabels` | `bool` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |
| `ValueTextStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `NavigationRequested` | `EventHandler<SearchBoxNavigationRequestedEventArgs>?` |
| `QueryChanged` | `EventHandler<SearchBoxQueryChangedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
