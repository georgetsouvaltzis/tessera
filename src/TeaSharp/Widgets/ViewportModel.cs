using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Widgets;

public sealed class ViewportModel
{
    private readonly List<string> _sourceLines = [];
    private readonly List<string> _visualLinesCache = [];
    private int _maxVisualWidth;
    private bool _visualCacheDirty = true;

    public int Width { get; private set; } = 1;

    public int Height { get; private set; } = 1;

    public int XOffset { get; private set; }

    public int YOffset { get; private set; }

    public bool Wrap { get; private set; }

    public bool ShowLineNumbers { get; set; }

    public int? HighlightVisualLine { get; set; }

    public void Resize(int width, int height)
    {
        var previousWidth = Width;
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
        if (Wrap && previousWidth != Width)
        {
            _visualCacheDirty = true;
        }

        ClampOffsets();
    }

    public void SetWrap(bool wrap)
    {
        if (Wrap == wrap)
        {
            return;
        }

        Wrap = wrap;
        _visualCacheDirty = true;
        ClampOffsets();
    }

    public void SetContent(string content)
    {
        SetLines(ViewportLineFormatter.NormalizeContentLines(content));
    }

    public void SetLines(IEnumerable<string> lines)
    {
        _sourceLines.Clear();
        _sourceLines.AddRange(lines);
        if (_sourceLines.Count == 0)
        {
            _sourceLines.Add(string.Empty);
        }

        _visualCacheDirty = true;
        ClampOffsets();
    }

    public void AppendLine(string line)
    {
        _sourceLines.Add(line.Replace('\n', ' ').Replace('\r', ' '));
        _visualCacheDirty = true;
        ClampOffsets();
    }

    public void Clear()
    {
        _sourceLines.Clear();
        _visualLinesCache.Clear();
        _maxVisualWidth = 0;
        _visualCacheDirty = false;
        XOffset = 0;
        YOffset = 0;
    }

    public void ScrollBy(int deltaY, int deltaX = 0)
    {
        YOffset += deltaY;
        if (!Wrap)
        {
            XOffset += deltaX;
        }

        ClampOffsets();
    }

    public void ScrollToTop()
    {
        YOffset = 0;
        ClampOffsets();
    }

    public void ScrollToBottom()
    {
        YOffset = Math.Max(0, GetVisualLines().Count - Height);
        ClampOffsets();
    }

    public bool Update(IMessage message, ViewportKeyMap? keyMap = null)
    {
        keyMap ??= ViewportKeyMap.Default;
        var beforeX = XOffset;
        var beforeY = YOffset;

        if (message is KeyPressMsg key)
        {
            if (keyMap.Up.Matches(key)) ScrollBy(-1);
            else if (keyMap.Down.Matches(key)) ScrollBy(1);
            else if (keyMap.PageUp.Matches(key)) ScrollBy(-Height);
            else if (keyMap.PageDown.Matches(key)) ScrollBy(Height);
            else if (keyMap.Home.Matches(key)) ScrollToTop();
            else if (keyMap.End.Matches(key)) ScrollToBottom();
            else if (keyMap.Left.Matches(key)) ScrollBy(0, -2);
            else if (keyMap.Right.Matches(key)) ScrollBy(0, 2);
        }
        else if (message is MouseWheelMsg wheel)
        {
            if (wheel.Button == MouseButton.WheelUp) ScrollBy(-3);
            else if (wheel.Button == MouseButton.WheelDown) ScrollBy(3);
        }

        return beforeX != XOffset || beforeY != YOffset;
    }

    public IReadOnlyList<string> RenderLines()
    {
        var visualLines = GetVisualLines();
        if (visualLines.Count == 0)
        {
            return [string.Empty];
        }

        var start = Math.Clamp(YOffset, 0, Math.Max(0, visualLines.Count - 1));
        var max = Math.Min(Height, visualLines.Count - start);
        if (max <= 0)
        {
            return [string.Empty];
        }

        var rendered = new List<string>(max);
        var lineNumberWidth = ViewportLineFormatter.ComputeLineNumberWidth(ShowLineNumbers, visualLines.Count);
        for (var i = 0; i < max; i++)
        {
            var visualIndex = start + i;
            var line = visualLines[visualIndex];
            var clipped = ViewportLineFormatter.ClipLine(line, Wrap, Width, XOffset, ShowLineNumbers, lineNumberWidth);
            rendered.Add(ViewportLineFormatter.DecorateLine(clipped, ShowLineNumbers, HighlightVisualLine, visualIndex, lineNumberWidth, Width));
        }

        return rendered;
    }

    private IReadOnlyList<string> GetVisualLines()
    {
        if (_visualCacheDirty)
        {
            ViewportVisualLineBuilder.Build(_sourceLines, Wrap, Width, _visualLinesCache, out _maxVisualWidth);
            _visualCacheDirty = false;
        }

        return _visualLinesCache;
    }

    private void ClampOffsets()
    {
        var lines = GetVisualLines();
        var maxY = Math.Max(0, lines.Count - Height);
        YOffset = Math.Clamp(YOffset, 0, maxY);

        if (Wrap)
        {
            XOffset = 0;
            return;
        }

        var lineNumberWidth = ViewportLineFormatter.ComputeLineNumberWidth(ShowLineNumbers, lines.Count);
        var visibleWidth = ShowLineNumbers ? Math.Max(0, Width - (lineNumberWidth + 2)) : Width;
        var maxX = Math.Max(0, _maxVisualWidth - visibleWidth);
        XOffset = Math.Clamp(XOffset, 0, maxX);
    }
}
