using TeaSharp.Layout;

namespace TeaSharp.Examples.DataWorkbench;

internal sealed partial class DataWorkbenchApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        var compactHeight = context.Height < 34;
        if (context.Width < 120)
        {
            window.Header(compactHeight ? 10 : 12, header => header.Column(column =>
            {
                column.Fixed(compactHeight ? 5 : 6, _header);
                column.Fixed(3, stats => stats.Row(row =>
                {
                    row.Weighted(1, _slicePulse);
                    row.Weighted(1, _velocityPulse);
                    row.Weighted(1, _comparePulse);
                }));
                column.Fill(nav => nav.Row(row =>
                {
                    row.Weighted(3, _pageTabs);
                    row.Weighted(1, _citrineButton);
                    row.Weighted(1, _cobaltButton);
                    row.Weighted(1, _emberButton);
                }));
            }));
            return;
        }

        window.Header(compactHeight ? 8 : 10, header => header.Column(column =>
        {
            column.Fixed(compactHeight ? 5 : 6, top => top.Row(row =>
            {
                row.Weighted(4, _header);
                row.Weighted(1, _slicePulse);
                row.Weighted(1, _velocityPulse);
                row.Weighted(1, _comparePulse);
            }));
            column.Fill(nav => nav.Row(row =>
            {
                row.Weighted(4, _pageTabs);
                row.Weighted(1, _citrineButton);
                row.Weighted(1, _cobaltButton);
                row.Weighted(1, _emberButton);
            }));
        }));
    }

    private void ConfigureBody(ContentBuilder body, ScreenContext context)
    {
        var railWidth = Math.Clamp(context.Width / 6, 22, 26);
        var rightWidth = Math.Clamp(context.Width / 3, 30, 38);
        var outputHeight = context.Height < 32 ? 5 : 9;

        body.Column(column =>
        {
            column.Fill(top =>
            {
                switch (_page)
                {
                    case DataWorkbenchPage.Compare:
                        ConfigureComparePage(top, railWidth, rightWidth);
                        break;
                    case DataWorkbenchPage.History:
                        ConfigureHistoryPage(top, railWidth, rightWidth);
                        break;
                    case DataWorkbenchPage.Saved:
                        ConfigureSavedPage(top, railWidth, rightWidth);
                        break;
                    default:
                        ConfigureExplorePage(top, railWidth, rightWidth);
                        break;
                }
            });

            if (_page != DataWorkbenchPage.History)
            {
                column.Fixed(outputHeight, _output);
            }
        });
    }

    private void ConfigureExplorePage(ContentBuilder body, int railWidth, int rightWidth)
    {
        body.Row(row =>
        {
            row.Fixed(railWidth, _sourceRail);
            row.Weighted(3, center => center.Column(column =>
            {
                column.Auto(_search);
                column.Fixed(5, _query);
                column.Auto(actions => actions.Row(actionRow =>
                {
                    actionRow.Weighted(1, _runButton);
                    actionRow.Weighted(1, _pinButton);
                    actionRow.Weighted(1, _saveButton);
                    actionRow.Weighted(1, _exportButton);
                    actionRow.Weighted(1, _clearButton);
                }));
                column.Fill(_results);
            }));
            row.Fixed(rightWidth, right => right.Column(column =>
            {
                column.Auto(_inspectTabs);
                switch (_inspectTabs.SelectedItem?.Id)
                {
                    case "json":
                        column.Fill(_jsonView);
                        break;
                    case "trace":
                        column.Fill(_traceView);
                        break;
                    default:
                        column.Fill(_profileView);
                        break;
                }
            }));
        });
    }

    private void ConfigureComparePage(ContentBuilder body, int railWidth, int rightWidth)
    {
        body.Row(row =>
        {
            row.Fixed(railWidth, _sourceRail);
            row.Weighted(4, center => center.Column(column =>
            {
                column.Fixed(6, _compareSummary);
                column.Fill(pair => pair.Row(compareRow =>
                {
                    compareRow.Weighted(1, _compareLeft);
                    compareRow.Weighted(1, _compareRight);
                }));
            }));
            row.Fixed(rightWidth, right => right.Column(column =>
            {
                column.Auto(actions => actions.Row(actionRow =>
                {
                    actionRow.Weighted(1, _pinButton);
                    actionRow.Weighted(1, _saveButton);
                }));
                column.Fill(_traceView);
            }));
        });
    }

    private void ConfigureHistoryPage(ContentBuilder body, int railWidth, int rightWidth)
    {
        body.Row(row =>
        {
            row.Fixed(railWidth, _sourceRail);
            row.Weighted(2, center => center.Column(column =>
            {
                column.Fixed(6, _query);
                column.Fill(_activity);
            }));
            row.Fixed(rightWidth, right => right.Column(column =>
            {
                column.Fixed(6, _savedPreview);
                column.Fill(_output);
            }));
        });
    }

    private void ConfigureSavedPage(ContentBuilder body, int railWidth, int rightWidth)
    {
        body.Row(row =>
        {
            row.Fixed(railWidth, _sourceRail);
            row.Fixed(28, _savedViews);
            row.Weighted(2, center => center.Column(column =>
            {
                column.Fixed(6, _savedPreview);
                column.Fill(_savedRunbook);
            }));
            row.Fixed(rightWidth, right => right.Column(column =>
            {
                column.Fixed(6, _compareSummary);
                column.Fill(_activity);
            }));
        });
    }
}
