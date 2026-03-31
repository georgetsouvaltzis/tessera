using TeaSharp.Layout;

namespace TeaSharp.Examples.GitConsole;

internal sealed partial class GitConsoleApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        if (context.Width < 110)
        {
            window.Header(
                8,
                header => header.Row(row =>
                {
                    row.Weighted(4, _repoHeader);
                    row.Weighted(2, _queueCard);
                }));
            return;
        }

        window.Header(
            8,
            header => header.Row(row =>
            {
                row.Weighted(3, _repoHeader);
                row.Weighted(1, _flowCard);
                row.Weighted(1, _queueCard);
                row.Weighted(1, _syncCard);
            }));
    }

    private void ConfigureBody(ContentBuilder body, ScreenContext context)
    {
        if (context.Width < 110)
        {
            body.Row(row =>
            {
                row.Fixed(24, left => left.Column(column =>
                {
                    column.Fixed(6, _scopeRail);
                    column.Fill(_worktree);
                }));
                row.Fill(right => right.Column(column =>
                {
                    column.Auto(_diffTabs);
                    column.Auto(_diffBriefing);
                    column.Fixed(5, _diff);
                    column.Fill(bottom => bottom.Row(bottomRow =>
                    {
                        bottomRow.Fixed(26, commit => commit.Column(commitColumn =>
                        {
                            commitColumn.Auto(_rightHeader);
                            commitColumn.Auto(_subjectInput);
                            commitColumn.Auto(actions => actions.Row(actionRow =>
                            {
                                actionRow.Weighted(1, _stageButton);
                                actionRow.Weighted(1, _discardButton);
                                actionRow.Weighted(1, _modeButton);
                            }));
                            commitColumn.Auto(actions => actions.Row(actionRow =>
                            {
                                actionRow.Weighted(1, _commitButton);
                                actionRow.Weighted(1, _syncButton);
                            }));
                        }));
                        bottomRow.Fill(_history);
                    }));
                }));
            });
            return;
        }

        var leftWidth = Math.Clamp(context.Width / 4, 24, 32);
        var rightWidth = Math.Clamp(context.Width / 3, 24, 36);

        body.Row(row =>
        {
            row.Fixed(leftWidth, content => content.Column(column =>
            {
                column.Fixed(8, _scopeRail);
                column.Fill(_worktree);
            }));
            row.Weighted(3, content => content.Column(column =>
            {
                column.Auto(_diffTabs);
                column.Auto(_diffBriefing);
                column.Fill(_diff);
                column.Auto(actions => actions.Row(actionRow =>
                {
                    actionRow.Weighted(1, _stageButton);
                    actionRow.Weighted(1, _discardButton);
                    actionRow.Weighted(1, _modeButton);
                }));
            }));
            row.Fixed(rightWidth, content => content.Column(column =>
            {
                column.Auto(_rightHeader);
                column.Auto(_subjectInput);
                column.Fixed(8, _notesInput);
                column.Auto(actions => actions.Row(actionRow =>
                {
                    actionRow.Weighted(1, _commitButton);
                    actionRow.Weighted(1, _syncButton);
                }));
                column.Fill(_history);
            }));
        });
    }
}
