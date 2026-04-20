---
title: "DataGrid"
sidebar_label: "DataGrid"
---

# `DataGrid`

**Family:** Data & Inspection  
**Namespace:** `Tessera.Controls`

Use `DataGrid` when this interaction is the best match for your screen workflow.

## When to use

- You need record-heavy inspection with selection and sortable columns.
- The UI must handle dense tabular workflows.


## Minimal usage

```csharp
using Tessera.Controls;
using Tessera.Layout;

var grid = new DataGrid
{
    Title = "Orders",
    ShowHeader = true,
    PageSize = 50
};

grid.SetColumns(
    new DataGridColumn("Id", 10),
    new DataGridColumn("Owner", 18),
    new DataGridColumn("Status", 12));

grid.SetRows(new[]
{
    new[] { "risk_10443", "luca ramos", "escalated" },
    new[] { "risk_10448", "nina maric", "watch" }
});

return Screen.Build(window => window.Body(body => body.Fill(grid)));
```

## Common pitfalls

- Keep column count intentional; avoid unreadable ultra-wide grids.
- Push expensive data refresh to effects/background workflows, not per-frame render.


## Public properties

| Property | Type |
| --- | --- |
| `Border` | `BorderStyle` |
| `BorderStyleText` | `TesseraStyle` |
| `ColumnSeparatorText` | `string` |
| `DisabledStyle` | `TesseraStyle` |
| `FocusedBorderStyleText` | `TesseraStyle` |
| `FocusedTitleStyle` | `TesseraStyle` |
| `FocusMarker` | `string` |
| `HeaderStyle` | `TesseraStyle` |
| `HoveredCellStyle` | `TesseraStyle` |
| `HoveredRowStyle` | `TesseraStyle` |
| `IsDisabled` | `bool` |
| `IsFocused` | `bool` |
| `IsReadOnly` | `bool` |
| `MutedRowPredicate` | `Func<int, IReadOnlyList<string>, bool>?` |
| `MutedStyle` | `TesseraStyle` |
| `Padding` | `Thickness` |
| `PageSize` | `int` |
| `RowStyle` | `TesseraStyle` |
| `SelectedCellStyle` | `TesseraStyle` |
| `SelectedRowStyle` | `TesseraStyle` |
| `ShowFocusMarker` | `bool` |
| `ShowHeader` | `bool` |
| `SortAscendingMarker` | `string` |
| `SortColumnIndex` | `int` |
| `SortDescending` | `bool` |
| `SortDescendingMarker` | `string` |
| `Title` | `string` |
| `TitleStyle` | `TesseraStyle` |

## Public events

| Event | Type |
| --- | --- |
| `SortRequested` | `EventHandler<DataGridSortRequestedEventArgs>?` |

## Related docs

- [Widget Reference](/docs/widget-reference)
- [Widgets Overview](/docs/controls-overview)
- [Public API Inventory](/docs/public-api-inventory)
