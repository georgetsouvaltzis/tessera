using System.Globalization;
using System.Text;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable log tail view for streaming operational output.
/// </summary>
public sealed partial class LogTailPanel : Control
{
    private readonly List<LogEntry> _entries = [];
    private readonly List<string> _entryBodyCache = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private bool _entryCacheDirty;

    /// <summary>
    /// Occurs when selected row changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Log Tail";

    /// <summary>
    /// Gets or sets focus marker appended to <see cref="Title"/> when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focus marker is shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets text shown when no entries are present.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no logs)";

    /// <summary>
    /// Gets or sets marker for selected rows.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▸";

    /// <summary>
    /// Gets or sets marker for unselected rows.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets whether timestamps are rendered.
    /// </summary>
    public bool ShowTimestamp
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            _entryCacheDirty = true;
        }
    } = true;

    /// <summary>
    /// Gets or sets whether level tags are rendered.
    /// </summary>
    public bool ShowLevel
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            _entryCacheDirty = true;
        }
    } = true;

    /// <summary>
    /// Gets or sets whether source labels are rendered.
    /// </summary>
    public bool ShowSource
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            _entryCacheDirty = true;
        }
    } = true;

    /// <summary>
    /// Gets or sets whether appending should keep selection on newest row.
    /// </summary>
    public bool AutoFollow { get; set; } = true;

    /// <summary>
    /// Gets or sets maximum number of retained entries.
    /// </summary>
    public int MaxEntries
    {
        get;
        set => field = Math.Max(1, value);
    } = 1024;

    /// <summary>
    /// Gets or sets whether control-level error style is active.
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// Gets or sets title style while not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets base row style.
    /// </summary>
    public TeaStyle EntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TeaStyle HoveredEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TeaStyle FocusedSelectedEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into muted rows.
    /// </summary>
    public TeaStyle MutedEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets trace-level style.
    /// </summary>
    public TeaStyle TraceEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets debug-level style.
    /// </summary>
    public TeaStyle DebugEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets info-level style.
    /// </summary>
    public TeaStyle InfoEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets warning-level style.
    /// </summary>
    public TeaStyle WarningEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets error-level style.
    /// </summary>
    public TeaStyle ErrorEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets critical-level style.
    /// </summary>
    public TeaStyle CriticalEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when control is disabled.
    /// </summary>
    public TeaStyle DisabledEntryStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style applied while not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style merged while focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets current entry list.
    /// </summary>
    public IReadOnlyList<LogEntry> Entries => _entries;

    /// <summary>
    /// Gets current selected index.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets current selected entry.
    /// </summary>
    public LogEntry? SelectedEntry => _selectedIndex >= 0 && _selectedIndex < _entries.Count ? _entries[_selectedIndex] : null;

    /// <summary>
    /// Gets current entry count.
    /// </summary>
    public int Count => _entries.Count;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _entries.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_entries.Count - 1);
        }

        if (key.IsCharacter('c'))
        {
            Clear();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _entries.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1);
            }

            return false;
        }

        if (_entries.Count == 0)
        {
            return false;
        }

        var hovered = ComputeWindowStart(content.Height) + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _entries.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            var changed = SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            return changed;
        }

        return false;
    }

    private bool SetHoveredIndex(int hoveredIndex)
    {
        if (_hoveredIndex == hoveredIndex)
        {
            return false;
        }

        _hoveredIndex = hoveredIndex;
        return true;
    }

    private int ComputeWindowStart(int availableRows)
    {
        var rows = Math.Max(1, availableRows);
        if (_entries.Count <= rows)
        {
            return 0;
        }

        if (AutoFollow && _selectedIndex >= _entries.Count - 1)
        {
            return _entries.Count - rows;
        }

        var end = _entries.Count - rows;
        return Math.Clamp(_selectedIndex - rows + 1, 0, end);
    }

}
