using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.TransitBoard;

internal sealed class TransitJourneyControl : Control
{
    public string Title { get; set; } = "Service Path";
    public TransitService? Service { get; set; }
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle DividerStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle PrimaryStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle SecondaryStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle AccentStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle MutedStyle { get; set; } = TesseraStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.WriteText(clipped.X, clipped.Y, Render(DividerStyle, "│"), 1);
        canvas.WriteText(clipped.X + 2, clipped.Y, Render(TitleStyle, Title), Math.Max(0, clipped.Width - 2));
        if (clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, Render(DividerStyle, new string('─', clipped.Width)), clipped.Width);
        }

        if (Service is null)
        {
            if (clipped.Height > 2)
            {
                canvas.WriteText(clipped.X + 2, clipped.Y + 2, Render(MutedStyle, "Select a service to view the station run."), Math.Max(0, clipped.Width - 2));
            }
            return;
        }

        var titleLine = $"{Render(AccentStyle, Service.RouteCode)} {Render(PrimaryStyle, Service.Destination)}";
        canvas.WriteText(clipped.X + 2, clipped.Y + 2, titleLine, Math.Max(0, clipped.Width - 2));

        var maxStops = Math.Max(0, clipped.Height - 4);
        for (var index = 0; index < Math.Min(Service.Calls.Count, maxStops); index++)
        {
            var call = Service.Calls[index];
            var marker = index == Service.Calls.Count - 1 ? "◆" : "•";
            var text = $"{Render(AccentStyle, marker)} {Render(SecondaryStyle, call.Time.ToString("HH:mm", CultureInfo.InvariantCulture))}  {Render(PrimaryStyle, call.Stop)}";
            canvas.WriteText(clipped.X + 2, clipped.Y + 3 + index, text, Math.Max(0, clipped.Width - 2));
        }
    }

    private static string Render(TesseraStyle style, string text) => style.IsEmpty ? text : style.Render(text);
}
