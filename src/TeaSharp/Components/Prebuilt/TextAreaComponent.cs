using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class TextAreaComponent : IStatefulComponent, IFocusableComponent
{
    private readonly ViewportModel _viewport = new();
    private readonly TextInputModel _input = new() { Multiline = true };

    public TextAreaComponent()
    {
    }

    public TextAreaComponent(TextAreaOptions options)
        : this()
    {
        Title = options.Title;
        Focused = options.Focused;
        ShowBorder = options.ShowBorder;
        ShowLineNumbers = options.ShowLineNumbers;
        Wrap = options.Wrap;
        InputKeyMap = options.InputKeyMap ?? TextInputKeyMap.Default;
        ViewportKeyMap = options.ViewportKeyMap ?? ViewportKeyMap.Default;
        if (!string.IsNullOrEmpty(options.InitialValue))
        {
            SetValue(options.InitialValue);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public string Title { get; set; } = "Text Area";

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    public string Value => Input.Value;

    public void SetValue(string value)
    {
        Input.SetValue(value);
        SyncViewport();
    }

    public void Clear()
    {
        Input.Clear();
        SyncViewport();
    }

    public bool Update(IMessage message)
    {
        var changed = false;
        var update = _input.Update(message, InputKeyMap);
        if (update.Changed)
        {
            SyncViewport();
            changed = true;
        }

        if (_viewport.Update(message, ViewportKeyMap))
        {
            changed = true;
        }

        _viewport.HighlightVisualLine = CursorLineIndex();
        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        _viewport.Resize(content.Width, content.Height);
        _viewport.HighlightVisualLine = CursorLineIndex();
        SyncViewport();

        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private void SyncViewport()
    {
        _viewport.SetLines(_input.Value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n'));
    }

    private int CursorLineIndex()
    {
        if (_input.Cursor <= 0)
        {
            return 0;
        }

        var cursor = Math.Min(_input.Cursor, _input.Value.Length);
        var lines = 0;
        for (var i = 0; i < cursor; i++)
        {
            if (_input.Value[i] == '\n')
            {
                lines++;
            }
        }

        return lines;
    }

    private TextInputModel Input => _input;
}
