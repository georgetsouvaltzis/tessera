using Tessera.Layout;

namespace Tessera.Examples.IncidentDesk;

internal sealed partial class IncidentDeskApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        if (context.Width < 120)
        {
            window.Header(
                10,
                header => header.Column(column =>
                {
                    column.Fixed(6, _hero);
                    column.Fill(cards => cards.Row(row =>
                    {
                        row.Weighted(1, _queuePulse);
                        row.Weighted(1, _escalationPulse);
                        row.Weighted(1, _crewPulse);
                    }));
                }));
            return;
        }

        window.Header(
            8,
            header => header.Row(row =>
            {
                row.Weighted(4, _hero);
                row.Weighted(1, _queuePulse);
                row.Weighted(1, _escalationPulse);
                row.Weighted(1, _crewPulse);
            }));
    }

    private void ConfigureBody(ContentBuilder body, ScreenContext context)
    {
        if (context.Width < 120)
        {
            body.Column(column =>
            {
                column.Fixed(10, top => top.Row(row =>
                {
                    row.Fixed(34, _queue);
                    row.Fill(_briefing);
                }));
                column.Fixed(12, middle => middle.Row(row =>
                {
                    row.Fixed(34, _responderCard);
                    row.Fill(right => right.Column(stack =>
                    {
                        stack.Fixed(6, actions => actions.Row(buttons =>
                        {
                            buttons.Weighted(1, _ackButton);
                            buttons.Weighted(1, _assignButton);
                            buttons.Weighted(1, _escalateButton);
                        }));
                        stack.Fixed(6, actions => actions.Row(buttons =>
                        {
                            buttons.Weighted(1, _resolveButton);
                            buttons.Weighted(1, _reopenButton);
                            buttons.Weighted(1, _syncButton);
                        }));
                        stack.Fill(_notes);
                    }));
                }));
                column.Fill(bottom => bottom.Row(row =>
                {
                    row.Weighted(3, _timeline);
                    row.Weighted(2, _logs);
                }));
            });
            return;
        }

        var leftWidth = Math.Clamp(context.Width / 4, 30, 36);
        var rightWidth = Math.Clamp(context.Width / 4, 32, 38);

        body.Column(column =>
        {
            column.Fill(top => top.Row(row =>
            {
                row.Fixed(leftWidth, _queue);
                row.Weighted(3, center => center.Column(stack =>
                {
                    stack.Fixed(9, _briefing);
                    stack.Fill(_timeline);
                }));
                row.Fixed(rightWidth, right => right.Column(stack =>
                {
                    stack.Fixed(8, _responderCard);
                    stack.Fill(_notes);
                    stack.Auto(actions => actions.Row(buttons =>
                    {
                        buttons.Weighted(1, _ackButton);
                        buttons.Weighted(1, _assignButton);
                        buttons.Weighted(1, _escalateButton);
                    }));
                    stack.Auto(actions => actions.Row(buttons =>
                    {
                        buttons.Weighted(1, _resolveButton);
                        buttons.Weighted(1, _reopenButton);
                        buttons.Weighted(1, _syncButton);
                    }));
                }));
            }));
            column.Fixed(9, _logs);
        });
    }
}
