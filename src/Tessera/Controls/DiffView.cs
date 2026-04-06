using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a scrollable line-level text diff viewer.
/// </summary>
public sealed class DiffView : Control
{
    private readonly List<DiffLineEntry> _entries = [];
    private int _selectedIndex;
    private int _scrollOffset;

    /// <summary>
    /// Gets or sets the optional title shown in the border.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Diff";

    /// <summary>
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be shown in the title.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style used when the control is not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the title style used when the control is focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the visual diff mode.
    /// </summary>
    public DiffViewMode Mode
    {
        get;
        set;
    } = DiffViewMode.Inline;

    /// <summary>
    /// Gets the computed line-level entries.
    /// </summary>
    public IReadOnlyList<DiffLineEntry> Entries => _entries;

    /// <summary>
    /// Gets the selected entry index.
    /// Returns <c>-1</c> when no entries are available.
    /// </summary>
    public int SelectedIndex => _entries.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets or sets the header row style.
    /// </summary>
    public TesseraStyle HeaderStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style for added entries.
    /// </summary>
    public TesseraStyle AddedLineStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style for removed entries.
    /// </summary>
    public TesseraStyle RemovedLineStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style for unchanged entries.
    /// </summary>
    public TesseraStyle UnchangedLineStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the selected entry.
    /// </summary>
    public TesseraStyle SelectedLineStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces old/new content and recalculates line-level entries.
    /// </summary>
    /// <param name="oldText">Original content.</param>
    /// <param name="newText">Updated content.</param>
    public void SetTexts(string oldText, string newText)
    {
        var oldLines = ControlTextLayout.SplitLines(oldText ?? string.Empty);
        var newLines = ControlTextLayout.SplitLines(newText ?? string.Empty);
        RebuildEntries(oldLines, newLines);
    }

    /// <summary>
    /// Toggles between <see cref="DiffViewMode.Inline"/> and <see cref="DiffViewMode.SideBySide"/>.
    /// </summary>
    public void ToggleMode()
    {
        Mode = Mode == DiffViewMode.Inline
            ? DiffViewMode.SideBySide
            : DiffViewMode.Inline;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || _entries.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return MoveSelection(-1);
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return MoveSelection(1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelection(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelection(_entries.Count - 1);
        }

        if (key.Is(Key.PageUp))
        {
            return MoveSelection(-10);
        }

        if (key.Is(Key.PageDown))
        {
            return MoveSelection(10);
        }

        if (key.Is(Key.Tab))
        {
            ToggleMode();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || _entries.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveSelection(1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1);
            }
        }

        if (pointer.Kind != PointerEventKind.Press
            || pointer.Button != PointerButton.Left
            || !content.Contains(pointer.X, pointer.Y))
        {
            return Handle(message);
        }

        RequestFocus();
        if (pointer.Y == content.Y)
        {
            return true;
        }

        var row = pointer.Y - content.Y - 1;
        if (row < 0)
        {
            return true;
        }

        var index = _scrollOffset + row;
        return SetSelection(index) || true;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None
            ? null
            : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var header = Mode == DiffViewMode.Inline ? "Old -> New" : "Old | New";
        canvas.WriteText(content.X, content.Y, ApplyStyle(header, HeaderStyle), content.Width);

        var rowCapacity = Math.Max(0, content.Height - 1);
        EnsureScrollVisible(rowCapacity);

        if (_entries.Count == 0 || rowCapacity == 0)
        {
            return;
        }

        for (var row = 0; row < rowCapacity; row++)
        {
            var index = _scrollOffset + row;
            if (index >= _entries.Count)
            {
                break;
            }

            var entry = _entries[index];
            var raw = Mode == DiffViewMode.Inline
                ? FormatInline(entry)
                : DiffViewHelpers.FormatSideBySide(entry, content.Width);
            var style = ResolveEntryStyle(entry.Kind, selected: index == _selectedIndex);
            canvas.WriteText(content.X, content.Y + row + 1, ApplyStyle(raw, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 24 + Padding.Horizontal;
        if (Border != BorderStyle.None)
        {
            width += 2;
        }

        var height = 6 + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool MoveSelection(int delta)
    {
        if (_entries.Count == 0 || delta == 0)
        {
            return false;
        }

        return SetSelection(_selectedIndex + delta);
    }

    private bool SetSelection(int index)
    {
        if (_entries.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _entries.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = clamped;
        return true;
    }

    private void EnsureScrollVisible(int rowCapacity)
    {
        if (rowCapacity <= 0 || _entries.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var maxOffset = Math.Max(0, _entries.Count - rowCapacity);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, maxOffset);
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + rowCapacity)
        {
            _scrollOffset = _selectedIndex - rowCapacity + 1;
        }
    }

    private void RebuildEntries(string[] oldLines, string[] newLines)
    {
        DiffViewHelpers.BuildEntries(oldLines, newLines, _entries);

        _selectedIndex = _entries.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _scrollOffset = 0;
    }

    private static string FormatInline(DiffLineEntry entry)
    {
        return entry.Kind switch
        {
            DiffLineKind.Added => $"+ {entry.NewLineNumber,4} {entry.NewText}",
            DiffLineKind.Removed => $"- {entry.OldLineNumber,4} {entry.OldText}",
            _ => $"  {entry.OldLineNumber,4} {entry.OldText}",
        };
    }

    private TesseraStyle ResolveEntryStyle(DiffLineKind kind, bool selected)
    {
        var style = kind switch
        {
            DiffLineKind.Added => AddedLineStyle,
            DiffLineKind.Removed => RemovedLineStyle,
            _ => UnchangedLineStyle,
        };

        return selected ? style.Merge(SelectedLineStyle) : style;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(title, style);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }
}
