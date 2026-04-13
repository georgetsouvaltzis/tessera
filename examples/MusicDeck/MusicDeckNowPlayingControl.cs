using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.MusicDeck;

internal sealed class MusicDeckNowPlayingControl : Control
{
    public string Title { get; set; } = "MusicDeck // Listening Room";
    public string TrackTitle { get; set; } = string.Empty;
    public string ArtistLine { get; set; } = string.Empty;
    public string ProgressLine { get; set; } = string.Empty;
    public string RemainingLine { get; set; } = string.Empty;
    public string SceneChip { get; set; } = string.Empty;
    public string DeviceChip { get; set; } = string.Empty;
    public string RoomChip { get; set; } = string.Empty;
    public string SummaryLine { get; set; } = string.Empty;
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle TrackStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ArtistStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ChipStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle SummaryStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ProgressStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, Render(TitleStyle, Title), Border, BorderStyleText);
        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        WriteLine(canvas, content, 0, Render(TrackStyle, TrackTitle));
        WriteLine(canvas, content, 1, Render(ArtistStyle, ArtistLine));
        WriteLine(canvas, content, 2,
            $"{Render(ChipStyle, $"[{SceneChip}]")} {Render(ChipStyle, $"[{DeviceChip}]")} {Render(ChipStyle, $"[{RoomChip}]")}");
        WriteLine(canvas, content, 3, $"{Render(ProgressStyle, ProgressLine)}  {Render(SummaryStyle, RemainingLine)}");
        WriteLine(canvas, content, 4, Render(SummaryStyle, SummaryLine));
    }

    private static void WriteLine(Canvas canvas, Rect content, int row, string text)
    {
        if (row >= content.Height)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + row, text, content.Width);
    }

    private static string Render(TesseraStyle style, string text)
    {
        return style.IsEmpty || string.IsNullOrEmpty(text) ? text : style.Render(text);
    }
}
