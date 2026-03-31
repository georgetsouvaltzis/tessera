using TeaSharp.Layout;

namespace TeaSharp.Examples.MusicDeck;

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
                    buttons.Weighted(1, _backButton);
                    buttons.Weighted(1, _playPauseButton);
                    buttons.Weighted(1, _nextButton);
                    buttons.Weighted(1, _detailButton);
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
