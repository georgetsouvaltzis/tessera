using TeaSharp.Layout;

namespace TeaSharp.Examples.DownloadCenter;

internal sealed partial class DownloadCenterApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        window.Header(
            context.Width < 132 ? 11 : 10,
            header => header.Column(column =>
            {
                column.Fixed(context.Width < 132 ? 6 : 5, top => top.Row(row =>
                {
                    row.Weighted(3, _hero);
                    row.Weighted(1, _lanePulse);
                    row.Weighted(1, _pipePulse);
                    row.Weighted(1, _retryPulse);
                }));
                column.Fixed(5, actions => actions.Row(row =>
                {
                    row.Fixed(18, _pauseButton, new Thickness(0, 0, 1, 0));
                    row.Fixed(16, _retryButton, new Thickness(0, 0, 1, 0));
                    row.Fixed(17, _boostButton, new Thickness(0, 0, 1, 0));
                    row.Fixed(16, _purgeButton);
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
