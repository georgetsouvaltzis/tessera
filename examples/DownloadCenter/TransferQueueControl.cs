using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.DownloadCenter;

internal sealed class TransferQueueControl : Control
{
    private readonly List<DownloadQueueSection> _sections = [];

    public string Title { get; set; } = "Transfer Queue";
    public string FocusMarker { get; set; } = "◆";
    public bool ShowFocusMarker { get; set; } = true;
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public string SelectedId { get; set; } = string.Empty;
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle SectionStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle MetaStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ActiveStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle RetryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle CompleteStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle QueuedStyle { get; set; } = TeaStyle.Empty;

    public void SetSections(IEnumerable<DownloadQueueSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);
        _sections.Clear();
        _sections.AddRange(sections);
    }

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

        var title = FormatTitle();
        if (!string.IsNullOrEmpty(title))
        {
            canvas.WriteText(clipped.X + 2, clipped.Y, ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle), Math.Max(0, clipped.Width - 4));
        }

        var y = content.Y;
        foreach (var section in _sections)
        {
            if (y >= content.Bottom)
            {
                break;
            }

            var header = $"{section.Title.ToUpperInvariant()}  {section.Count:00}";
            canvas.WriteText(content.X, y++, ApplyStyle(header, SectionStyle), content.Width);
            foreach (var item in section.Items)
            {
                if (y >= content.Bottom)
                {
                    break;
                }

                var left = $"{ResolveMarker(item)} {item.Title}";
                var right = $"{item.ProgressText}  {item.EtaText}";
                var row = ComposeRow(left, right, content.Width);
                canvas.WriteText(content.X, y, ApplyStyle(row, ResolveItemStyle(item)), content.Width);
                y++;
                if (y >= content.Bottom)
                {
                    break;
                }

                var meta = $"   {item.Source}  {item.StateLabel}";
                canvas.WriteText(content.X, y++, ApplyStyle(meta, MetaStyle.Merge(ResolveTone(item))), content.Width);
            }
        }
    }

    private string FormatTitle()
    {
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private string FormatTitleForMeasure()
    {
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private TeaStyle ResolveBorderStyle()
    {
        return IsFocused
            ? BorderStyleText.Merge(FocusedBorderStyleText)
            : BorderStyleText;
    }

    private TeaStyle ResolveItemStyle(DownloadQueueItem item)
    {
        return item.Id == SelectedId
            ? SelectedItemStyle
            : ItemStyle.Merge(ResolveTone(item));
    }

    private TeaStyle ResolveTone(DownloadQueueItem item)
    {
        return item.Phase switch
        {
            DownloadJobPhase.Active or DownloadJobPhase.Verifying => ActiveStyle,
            DownloadJobPhase.Retrying or DownloadJobPhase.Failed => RetryStyle,
            DownloadJobPhase.Completed => CompleteStyle,
            _ => QueuedStyle,
        };
    }

    private static string ResolveMarker(DownloadQueueItem item)
    {
        return item.Phase switch
        {
            DownloadJobPhase.Active => "⇣",
            DownloadJobPhase.Verifying => "◌",
            DownloadJobPhase.Retrying => "↺",
            DownloadJobPhase.Failed => "!",
            DownloadJobPhase.Completed => "✓",
            DownloadJobPhase.Paused => "‖",
            _ => "·",
        };
    }

    private static string ComposeRow(string left, string right, int width)
    {
        if (width <= 0)
        {
            return string.Empty;
        }

        var gap = 2;
        var leftWidth = Math.Max(0, width - right.Length - gap);
        if (leftWidth < left.Length)
        {
            left = left[..Math.Max(0, leftWidth)];
        }

        return left.PadRight(Math.Max(0, width - right.Length)) + right;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);
    }
}

internal readonly record struct DownloadQueueSection(string Title, int Count, IReadOnlyList<DownloadQueueItem> Items);

internal readonly record struct DownloadQueueItem(
    string Id,
    string Title,
    string Source,
    string ProgressText,
    string EtaText,
    string StateLabel,
    DownloadJobPhase Phase);
