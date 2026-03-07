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
        _sourceLines.Clear();
        var normalized = content
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        _sourceLines.AddRange(normalized.Split('\n'));
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
            if (keyMap.Up.Matches(key))
            {
                ScrollBy(-1);
            }
            else if (keyMap.Down.Matches(key))
            {
                ScrollBy(1);
            }
            else if (keyMap.PageUp.Matches(key))
            {
                ScrollBy(-Height);
            }
            else if (keyMap.PageDown.Matches(key))
            {
                ScrollBy(Height);
            }
            else if (keyMap.Home.Matches(key))
            {
                ScrollToTop();
            }
            else if (keyMap.End.Matches(key))
            {
                ScrollToBottom();
            }
            else if (keyMap.Left.Matches(key))
            {
                ScrollBy(0, -2);
            }
            else if (keyMap.Right.Matches(key))
            {
                ScrollBy(0, 2);
            }
        }
        else if (message is MouseWheelMsg wheel)
        {
            if (wheel.Button == MouseButton.WheelUp)
            {
                ScrollBy(-3);
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                ScrollBy(3);
            }
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
        var lineNumberWidth = ShowLineNumbers
            ? Math.Max(2, (visualLines.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture).Length)
            : 0;
        for (var i = 0; i < max; i++)
        {
            var visualIndex = start + i;
            var line = visualLines[visualIndex];
            var clipped = ClipLine(line, lineNumberWidth);
            rendered.Add(DecorateLine(clipped, visualIndex, lineNumberWidth));
        }

        return rendered;
    }

    private string ClipLine(string line, int lineNumberWidth)
    {
        var availableWidth = ShowLineNumbers
            ? Math.Max(0, Width - (lineNumberWidth + 2))
            : Width;
        if (availableWidth <= 0)
        {
            return string.Empty;
        }

        if (Wrap)
        {
            return line.Length <= availableWidth
                ? line
                : line[..availableWidth];
        }

        if (XOffset >= line.Length)
        {
            return string.Empty;
        }

        if (XOffset == 0 && line.Length <= availableWidth)
        {
            return line;
        }

        var remaining = line.Length - XOffset;
        var length = Math.Min(availableWidth, remaining);
        return line.Substring(XOffset, length);
    }

    private string DecorateLine(string line, int visualIndex, int lineNumberWidth)
    {
        if (!ShowLineNumbers && HighlightVisualLine != visualIndex)
        {
            return line;
        }

        if (!ShowLineNumbers)
        {
            return HighlightVisualLine == visualIndex
                ? $"> {line}"
                : $"  {line}";
        }

        var lineNumber = (visualIndex + 1).ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(lineNumberWidth);
        var marker = HighlightVisualLine == visualIndex ? ">" : " ";
        var prefix = $"{lineNumber}{marker} ";
        if (prefix.Length >= Width)
        {
            return prefix[..Width];
        }

        var available = Width - prefix.Length;
        var clipped = line.Length <= available
            ? line
            : line[..available];
        return prefix + clipped;
    }

    private IReadOnlyList<string> GetVisualLines()
    {
        if (!_visualCacheDirty)
        {
            return _visualLinesCache;
        }

        _visualLinesCache.Clear();
        _maxVisualWidth = 0;

        if (_sourceLines.Count == 0)
        {
            _visualLinesCache.Add(string.Empty);
            _maxVisualWidth = 0;
            _visualCacheDirty = false;
            return _visualLinesCache;
        }

        if (!Wrap || Width <= 0)
        {
            foreach (var line in _sourceLines)
            {
                _visualLinesCache.Add(line);
                _maxVisualWidth = Math.Max(_maxVisualWidth, line.Length);
            }

            _visualCacheDirty = false;
            return _visualLinesCache;
        }

        foreach (var sourceLine in _sourceLines)
        {
            if (sourceLine.Length == 0)
            {
                _visualLinesCache.Add(string.Empty);
                continue;
            }

            for (var i = 0; i < sourceLine.Length; i += Width)
            {
                var length = Math.Min(Width, sourceLine.Length - i);
                _visualLinesCache.Add(sourceLine.Substring(i, length));
                _maxVisualWidth = Math.Max(_maxVisualWidth, length);
            }
        }

        _visualCacheDirty = false;
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

        var lineNumberWidth = ShowLineNumbers
            ? Math.Max(2, (lines.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture).Length)
            : 0;
        var visibleWidth = ShowLineNumbers
            ? Math.Max(0, Width - (lineNumberWidth + 2))
            : Width;
        var maxX = Math.Max(0, _maxVisualWidth - visibleWidth);
        XOffset = Math.Clamp(XOffset, 0, maxX);
    }
}
