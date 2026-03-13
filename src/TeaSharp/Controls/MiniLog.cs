using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact scrolling log control.
/// </summary>
public sealed class MiniLog : Control
{
    private readonly MiniLogComponent _component;

    /// <summary>
    /// Creates a compact log with the provided entry capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained log lines.</param>
    public MiniLog(int capacity = 120)
    {
        _component = new MiniLogComponent(capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained log lines.
    /// </summary>
    public int Capacity => _component.Capacity;

    /// <summary>
    /// Gets or sets the log title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the retained log lines.
    /// </summary>
    public IReadOnlyList<string> Entries => _component.Entries;

    /// <summary>
    /// Appends one log line.
    /// </summary>
    /// <param name="line">The log line to append.</param>
    public void Append(string line) => _component.Append(line ?? string.Empty);

    /// <summary>
    /// Clears all retained log lines.
    /// </summary>
    public void Clear() => _component.Clear();

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
