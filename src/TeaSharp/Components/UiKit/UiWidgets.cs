using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

internal static class UiWidgets
{
    public static void DrawBreadcrumb(Canvas canvas, Rect rect, IReadOnlyList<string> segments, string separator = " / ")
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var text = string.Join(separator, segments);
        canvas.WriteText(clipped.X, clipped.Y, text, clipped.Width);
    }

    public static void DrawStatusBar(Canvas canvas, Rect rect, string leftText, string rightText)
    {
        DrawStatusBar(canvas, rect, leftText, rightText, new UiTheme());
    }

    public static void DrawStatusBar(Canvas canvas, Rect rect, string leftText, string rightText, UiTheme theme)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var row = new string(theme.StatusFill, Math.Max(0, clipped.Width)).ToCharArray();
        CopyToRow(row, 0, leftText);
        var rightStart = Math.Max(0, clipped.Width - rightText.Length);
        CopyToRow(row, rightStart, rightText);
        canvas.WriteText(clipped.X, clipped.Y, new string(row), clipped.Width);
    }

    public static void DrawTimeline(Canvas canvas, Rect rect, IReadOnlyList<TimelineEntry> entries, string title = "Timeline")
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, entries.Count);
        for (var i = 0; i < rows; i++)
        {
            var entry = entries[i];
            var marker = i == rows - 1 ? "└" : "├";
            var line = $"{marker} {entry.Time} {entry.Text}";
            canvas.WriteText(content.X, content.Y + i, line, content.Width);
        }
    }

    public static void DrawTree(Canvas canvas, Rect rect, IReadOnlyList<TreeNode> nodes, string title = "Tree")
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, nodes.Count);
        for (var i = 0; i < rows; i++)
        {
            var node = nodes[i];
            var indent = new string(' ', Math.Max(0, node.Depth * 2));
            var prefix = node.Selected ? "› " : "  ";
            canvas.WriteText(content.X, content.Y + i, prefix + indent + node.Label, content.Width);
        }
    }

    public static void DrawCalendar(Canvas canvas, Rect rect, DateTime date, string title = "Calendar")
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || content.Height < 3)
        {
            return;
        }

        var first = new DateTime(date.Year, date.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var days = DateTime.DaysInMonth(date.Year, date.Month);

        canvas.WriteText(content.X, content.Y, first.ToString("MMMM yyyy", CultureInfo.InvariantCulture), content.Width);
        canvas.WriteText(content.X, content.Y + 1, "Mo Tu We Th Fr Sa Su", content.Width);

        var day = 1;
        var row = 2;
        while (day <= days && row < content.Height)
        {
            var line = new char[Math.Min(content.Width, 20)];
            Array.Fill(line, ' ');
            for (var col = 0; col < 7 && day <= days; col++)
            {
                if (row == 2 && col < startOffset)
                {
                    continue;
                }

                var index = col * 3;
                if (index + 1 >= line.Length)
                {
                    break;
                }

                var text = day.ToString(CultureInfo.InvariantCulture).PadLeft(2);
                line[index] = text[0];
                line[index + 1] = text[1];
                day++;
            }

            canvas.WriteText(content.X, content.Y + row, new string(line), content.Width);
            row++;
        }
    }

    public static void DrawSkeleton(Canvas canvas, Rect rect, string title = "Loading")
    {
        DrawSkeleton(canvas, rect, title, new UiTheme());
    }

    public static void DrawSkeleton(Canvas canvas, Rect rect, string title, UiTheme theme)
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        for (var row = 0; row < content.Height; row++)
        {
            var ch = row % 2 == 0 ? theme.SkeletonEvenFill : theme.SkeletonOddFill;
            canvas.DrawHorizontalLine(content.X, content.Y + row, content.Width, ch);
        }
    }

    private static void CopyToRow(char[] row, int start, string text)
    {
        if (row.Length == 0 || string.IsNullOrEmpty(text) || start >= row.Length)
        {
            return;
        }

        var index = Math.Max(0, start);
        for (var i = 0; i < text.Length && index < row.Length; i++, index++)
        {
            row[index] = text[i];
        }
    }
}
