using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.MusicDeck;

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
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle TrackStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ArtistStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ChipStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SummaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ProgressStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

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
        WriteLine(canvas, content, 2, $"{Render(ChipStyle, $"[{SceneChip}]")} {Render(ChipStyle, $"[{DeviceChip}]")} {Render(ChipStyle, $"[{RoomChip}]")}");
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

    private static string Render(TeaStyle style, string text) => style.IsEmpty || string.IsNullOrEmpty(text) ? text : style.Render(text);
}
