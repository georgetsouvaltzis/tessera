using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class TableApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly IReadOnlyList<IReadOnlyList<string>> SeedRows =
    [
        ["api", "healthy", "iad"],
        ["worker", "warning", "sfo"],
        ["billing", "healthy", "dub"],
        ["search", "degraded", "sin"],
        ["queue", "healthy", "fra"],
        ["edge", "warning", "gru"],
        ["cache", "healthy", "yyz"],
        ["cron", "degraded", "lhr"],
    ];

    private readonly Table _table = new("Service", "State", "Region")
    {
        Title = "Table",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        PageSize = 4,
        IsFocused = true,
    };

    private readonly StatusBar _status = new();

    private bool _styleAlt;
    private int _selectionChanges;
    private int _rowCount;
    private string _statusText = "widget-only proof: page/sort, pointer selection, api row mutation";

    public TableApp()
    {
        Seed();
        ApplyTheme();
        _table.SelectionChanged += (_, args) =>
        {
            _selectionChanges++;
            var previous = args.PreviousItem is null ? "-" : args.PreviousItem[0];
            var current = args.SelectedItem is null ? "-" : args.SelectedItem[0];
            _statusText = $"selection {previous}->{current}";
        };
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('q', ModifierKeys.Ctrl) || key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.IsCharacter('r', ModifierKeys.Ctrl))
        {
            Seed();
            _statusText = "api SetRows(seed): page reset";
            return null;
        }

        if (key.IsCharacter('a', ModifierKeys.Ctrl))
        {
            _table.AddRow(["ingest", "warning", "ord"]);
            _rowCount++;
            _statusText = "api AddRow(ingest,warning,ord)";
            return null;
        }

        if (key.IsCharacter('w', ModifierKeys.Ctrl))
        {
            _table.ReplaceRow(1, ["worker", "degraded", "sfo"]);
            _statusText = "api ReplaceRow(1, worker,degraded,sfo)";
            return null;
        }

        if (key.IsCharacter('x', ModifierKeys.Ctrl))
        {
            if (_rowCount > 0)
            {
                _table.RemoveRowAt(_rowCount - 1);
                _statusText = $"api RemoveRowAt({_rowCount - 1})";
                _rowCount--;
            }

            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            _table.ClearRows();
            _rowCount = 0;
            _statusText = "api ClearRows(): empty table";
            return null;
        }

        if (key.IsCharacter('g', ModifierKeys.Ctrl))
        {
            var changed = _table.SetSelectedIndex(99);
            _statusText = $"api SetSelectedIndex(99)={changed}";
            return null;
        }

        if (key.IsCharacter('t', ModifierKeys.Ctrl))
        {
            _styleAlt = !_styleAlt;
            ApplyTheme();
            _statusText = _styleAlt ? "style=alt" : "style=default";
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        UpdateFooter();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(new CenterLayout
            {
                Content = _table,
                Width = Math.Min(72, Math.Max(54, context.Width - 4)),
                Height = Math.Min(12, Math.Max(9, context.Height - 4)),
            });
            window.Footer(1, _status);
        });
    }

    private void Seed()
    {
        _table.ClearRows();
        _table.SetRows(SeedRows);
        _rowCount = SeedRows.Count;
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _table, _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        _table.TitleStyle = theme.Text.Primary;
        _table.FocusedTitleStyle = focusedBorder.WithBold();
        _table.BorderStyleText = theme.Border.Strong;
        _table.FocusedBorderStyleText = focusedBorder;
        _table.HeaderStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175)).WithBold()
            : theme.Text.Primary.WithBold();
        _table.RowStyle = theme.Text.Primary;
        _table.HoveredRowStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 220, 235)).WithUnderline()
            : theme.Accent.Secondary.WithUnderline();
        _table.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
    }

    private void UpdateFooter()
    {
        var selected = _table.TryGetSelectedRow(out var row) && row is not null
            ? $"{row[0]}/{row[1]}/{row[2]}"
            : "-";

        _status.LeftText =
            $"page={_table.PageIndex} sort={_table.SortColumn}:{(_table.SortDescending ? "desc" : "asc")} sel={selected} rows={_rowCount} sch={_selectionChanges}";
        _status.RightText =
            $"{_statusText} | click row select click header sort c next-col s desc [ prev-page ] next-page wheel page ^G select(99) ^A add ^W replace ^X remove-last ^E clear ^R seed ^T style ^C quit";
    }
}
