using System.ComponentModel;
using TeaSharp.Internal;
using TeaSharp.Widgets.Internal;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ViewportModel
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
        _sourceLines.Add(ViewportLineFormatter.NormalizeInlineLine(line ?? string.Empty));
        _visualCacheDirty = true;
        ClampOffsets();
    }

    public void AppendRawLine(string line)
    {
        _sourceLines.Add(line ?? string.Empty);
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

    public bool Update(global::TeaSharp.Core.Abstractions.IMessage message, ViewportKeyMap? keyMap = null)
    {
        return Update(TeaMessageAdapter.ToPublic(message), keyMap);
    }

    public bool Update(Message message, ViewportKeyMap? keyMap = null)
    {
        keyMap ??= ViewportKeyMap.Default;
        var beforeX = XOffset;
        var beforeY = YOffset;

        if (message is KeyPressed key)
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
        else if (message is PointerInput { Kind: PointerEventKind.Wheel } wheel)
        {
            if (wheel.Button == PointerButton.WheelUp) ScrollBy(-3);
            else if (wheel.Button == PointerButton.WheelDown) ScrollBy(3);
        }

        return beforeX != XOffset || beforeY != YOffset;
    }

    public IReadOnlyList<string> RenderLines()
    {
        return ViewportRenderer.RenderLines(
            GetVisualLines(),
            Width,
            Height,
            XOffset,
            YOffset,
            Wrap,
            ShowLineNumbers,
            HighlightVisualLine);
    }

    private List<string> GetVisualLines()
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
        YOffset = ViewportOffsets.ClampY(YOffset, lines.Count, Height);
        XOffset = ViewportOffsets.ClampX(Wrap, ShowLineNumbers, XOffset, Width, lines.Count, _maxVisualWidth);
    }
}
