using TeaSharp.Layout;

namespace TeaSharp.Examples.OpsWatch;

internal sealed partial class OpsWatchApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        if (context.Width < 128)
        {
            window.Header(
                18,
                header => header.Column(column =>
                {
                    column.Fixed(6, _hero);
                    column.Fixed(5, cards => cards.Row(row =>
                    {
                        row.Weighted(1, _fleetPulse);
                        row.Weighted(1, _trafficPulse);
                        row.Weighted(1, _routePulse);
                    }));
                    column.Fill(themes => themes.Row(row =>
                    {
                        row.Weighted(1, _veridianThemeButton);
                        row.Weighted(1, _tidalThemeButton);
                        row.Weighted(1, _redlineThemeButton);
                    }));
                }));
            return;
        }

        window.Header(
            11,
            header => header.Column(column =>
            {
                column.Fixed(7, top => top.Row(row =>
                {
                    row.Weighted(4, _hero);
                    row.Weighted(1, _fleetPulse);
                    row.Weighted(1, _trafficPulse);
                    row.Weighted(1, _routePulse);
                }));
                column.Fill(themes => themes.Row(row =>
                {
                    row.Weighted(1, _veridianThemeButton);
                    row.Weighted(1, _tidalThemeButton);
                    row.Weighted(1, _redlineThemeButton);
                }));
            }));
    }

    private void ConfigureBody(ContentBuilder body, ScreenContext context)
    {
        var railWidth = Math.Clamp(context.Width / 6, 18, 22);
        var actionWidth = Math.Clamp(context.Width / 4, 34, 40);

        body.Column(column =>
        {
            column.Fixed(12, top => top.Row(row =>
            {
                row.Weighted(1, deck => deck.Column(stack =>
                {
                    stack.Fixed(4, _cpuCard);
                    stack.Fill(_cpuSpark);
                }));
                row.Weighted(1, deck => deck.Column(stack =>
                {
                    stack.Fixed(4, _memoryCard);
                    stack.Fill(_memorySpark);
                }));
                row.Weighted(1, deck => deck.Column(stack =>
                {
                    stack.Fixed(4, _networkCard);
                    stack.Fill(_networkSpark);
                }));
                row.Weighted(1, deck => deck.Column(stack =>
                {
                    stack.Fixed(4, _diskCard);
                    stack.Fill(_diskSpark);
                }));
            }));

            column.Fill(middle => middle.Row(row =>
            {
                row.Fixed(railWidth, _fleetRail);
                row.Weighted(3, _healthBoard);
                row.Fixed(actionWidth, right => right.Column(stack =>
                {
                    stack.Fixed(8, _focusStats);
                    stack.Fixed(7, _focusSummary);
                    stack.Fixed(4, _cpuBullet);
                    stack.Fixed(4, _memoryBullet);
                    stack.Fixed(4, _networkBullet);
                    stack.Fixed(4, _diskBullet);
                    stack.Fixed(5, actions => actions.Row(buttons =>
                    {
                        buttons.Weighted(1, _restartButton);
                        buttons.Weighted(1, _drainButton);
                    }));
                    stack.Fixed(5, actions => actions.Row(buttons =>
                    {
                        buttons.Weighted(1, _muteButton);
                        buttons.Weighted(1, _scaleButton);
                    }));
                    stack.Fixed(5, actions => actions.Row(buttons =>
                    {
                        buttons.Weighted(1, _inspectButton);
                        buttons.Weighted(1, _failoverButton);
                    }));
                    stack.Fixed(5, _ackButton);
                    stack.Fill(_runbook);
                }));
            }));

            column.Fixed(11, _feed);
        });
    }
}
