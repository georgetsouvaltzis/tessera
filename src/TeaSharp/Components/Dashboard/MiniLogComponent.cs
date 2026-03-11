using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
namespace TeaSharp.Components.Dashboard;

public sealed class MiniLogComponent : ICanvasComponent
{
    private readonly List<string> _entries = [];

    public MiniLogComponent(int capacity = 120)
    {
        Capacity = Math.Max(1, capacity);
    }

    public int Capacity { get; }

    public string Title { get; set; } = "Mini Log";

    public IReadOnlyList<string> Entries => _entries;

    public void Append(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        var normalized = line
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var parts = normalized.Split('\n');
        foreach (var part in parts)
        {
            _entries.Add(part);
            if (_entries.Count > Capacity)
            {
                _entries.RemoveAt(0);
            }
        }
    }

    public void Clear()
    {
        _entries.Clear();
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty || _entries.Count == 0)
        {
            return;
        }

        var rows = Math.Min(content.Height, _entries.Count);
        var offset = Math.Max(0, _entries.Count - rows);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, _entries[offset + row], content.Width);
        }
    }
}
