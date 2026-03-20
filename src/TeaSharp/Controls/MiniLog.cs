using TeaSharp.Components.Primitives;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact scrolling log control.
/// </summary>
public sealed class MiniLog : Control
{
    private readonly List<string> _entries = [];

    /// <summary>
    /// Creates a compact log with the provided entry capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained log lines.</param>
    public MiniLog(int capacity = 120)
    {
        Capacity = Math.Max(1, capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained log lines.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets or sets the log title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Mini Log";

    /// <summary>
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be rendered when focused.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style applied when the control is not focused.
    /// </summary>
    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style applied when the control is focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to each rendered log line.
    /// </summary>
    public TeaStyle EntryStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets the retained log lines.
    /// </summary>
    public IReadOnlyList<string> Entries => _entries;

    /// <summary>
    /// Appends one log line.
    /// </summary>
    /// <param name="line">The log line to append.</param>
    public void Append(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        AppendNormalizedLines(line);
    }

    private void AppendNormalizedLines(string line)
    {
        var start = 0;
        for (var index = 0; index < line.Length; index++)
        {
            var current = line[index];
            if (current is not ('\n' or '\r'))
            {
                continue;
            }

            AddEntry(line.AsSpan(start, index - start));
            if (current == '\r' && index + 1 < line.Length && line[index + 1] == '\n')
            {
                index++;
            }

            start = index + 1;
        }

        AddEntry(line.AsSpan(start));
    }

    private void AddEntry(ReadOnlySpan<char> entry)
    {
        _entries.Add(entry.ToString());
        if (_entries.Count > Capacity)
        {
            _entries.RemoveAt(0);
        }
    }

    /// <summary>
    /// Clears all retained log lines.
    /// </summary>
    public void Clear() => _entries.Clear();

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 3)
        {
            return;
        }

        var title = FormatTitle();
        if (!string.IsNullOrEmpty(title))
        {
            var titleStyle = IsFocused ? FocusedTitleStyle : TitleStyle;
            if (!titleStyle.IsEmpty)
            {
                title = titleStyle.Render(title);
            }
        }

        canvas.DrawBox(clipped, title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty || _entries.Count == 0)
        {
            return;
        }

        var rows = Math.Min(content.Height, _entries.Count);
        var offset = Math.Max(0, _entries.Count - rows);
        for (var row = 0; row < rows; row++)
        {
            var line = _entries[offset + row];
            if (!EntryStyle.IsEmpty)
            {
                line = EntryStyle.Render(line);
            }

            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, Title.Length + 4);
        for (var index = 0; index < _entries.Count; index++)
        {
            width = Math.Max(width, _entries[index].Length + 2);
        }

        var height = Math.Max(3, Math.Min(_entries.Count + 2, Capacity + 2));
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }
}
