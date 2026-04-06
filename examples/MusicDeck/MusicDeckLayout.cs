using Tessera.Layout;

namespace Tessera.Examples.MusicDeck;

internal sealed partial class MusicDeckApp
{
    private void ConfigureBody(ContentBuilder body)
    {
        body.Row(row =>
        {
            row.Fixed(36, left => left.Column(column =>
            {
                column.Fill(_queue);
                column.Fixed(4, _sessionMeta);
            }));
            row.Fill(right => right.Column(column =>
            {
                column.Fixed(7, _nowPlaying);
                column.Fixed(4, _progress);
                column.Fixed(5, transport => transport.Row(buttons =>
                {
                    buttons.Fill(_transportLeftSpacer);
                    buttons.Fixed(11, _backButton, new Thickness(0, 0, 1, 0));
                    buttons.Fixed(12, _playPauseButton, new Thickness(0, 0, 1, 0));
                    buttons.Fixed(11, _nextButton, new Thickness(0, 0, 1, 0));
                    buttons.Fixed(12, _detailButton);
                    buttons.Fill(_transportRightSpacer);
                }));
                column.Fixed(4, stats => stats.Row(cards =>
                {
                    cards.Weighted(1, _deckStats);
                    cards.Weighted(1, _trackStats);
                }));
                column.Fill(_lyrics);
            }));
        });
    }
}
