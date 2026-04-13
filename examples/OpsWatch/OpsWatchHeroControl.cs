using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.OpsWatch;

internal sealed class OpsWatchHeroControl : Control
{
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "OpsWatch";

    public string ClockText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string FleetText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string ModeText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string RouteText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string PressureText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string CrewText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string CommandText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public BorderStyle Border { get; set; } = BorderStyle.Rounded;

    public Thickness Padding { get; set; } = Thickness.All(1);

    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    public TesseraStyle ClockStyle { get; set; } = TesseraStyle.Empty;

    public TesseraStyle BadgeStyle { get; set; } = TesseraStyle.Empty;

    public TesseraStyle MetaStyle { get; set; } = TesseraStyle.Empty;

    public TesseraStyle CommandStyle { get; set; } = TesseraStyle.Empty;

    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, null, Border, ResolveBorderStyle());
        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        WriteLine(canvas, content, 0,
            $"{ApplyStyle(Title.ToUpperInvariant(), TitleStyle)}  {ApplyStyle(ClockText, ClockStyle)}");
        WriteLine(canvas, content, 1,
            $"{ApplyStyle($"[{FleetText}]", BadgeStyle)} {ApplyStyle($"[{ModeText}]", BadgeStyle)} {ApplyStyle($"[{RouteText}]", BadgeStyle)}");
        WriteLine(canvas, content, 2, $"{ApplyStyle(PressureText, MetaStyle)}  {ApplyStyle(CrewText, MetaStyle)}");
        WriteLine(canvas, content, 3, ApplyStyle(CommandText, CommandStyle));
    }

    private static void WriteLine(Canvas canvas, Rect content, int row, string text)
    {
        if (row >= content.Height)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y + row, text, content.Width);
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);
    }
}
