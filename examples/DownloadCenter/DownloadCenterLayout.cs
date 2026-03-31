using TeaSharp.Layout;

namespace TeaSharp.Examples.DownloadCenter;

internal sealed partial class DownloadCenterApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        window.Header(
            context.Width < 132 ? 12 : 8,
            header => header.Column(column =>
            {
                column.Fixed(context.Width < 132 ? 6 : 5, top => top.Row(row =>
                {
                    row.Weighted(3, _hero);
                    row.Weighted(1, _lanePulse);
                    row.Weighted(1, _pipePulse);
                    row.Weighted(1, _retryPulse);
                }));
                column.Fill(actions => actions.Row(row =>
                {
                    row.Weighted(1, _pauseButton);
                    row.Weighted(1, _retryButton);
                    row.Weighted(1, _boostButton);
                    row.Weighted(1, _purgeButton);
                }));
            }));
    }

    private void ConfigureBody(ContentBuilder body, ScreenContext context)
    {
        var rightWidth = Math.Clamp(context.Width / 3, 34, 42);
        body.Column(column =>
        {
            column.Fixed(13, top => top.Row(row =>
            {
                row.Weighted(3, _queue);
                row.Fixed(rightWidth, right => right.Column(stack =>
                {
                    stack.Fixed(7, _selectionCard);
                    stack.Fixed(4, _progress);
                    stack.Fill(_runbook);
                }));
            }));

            column.Fixed(10, trends => trends.Row(row =>
            {
                row.Weighted(1, _throughputChart);
                row.Weighted(1, _retryChart);
            }));

            column.Fill(_feed);
        });
    }
}
