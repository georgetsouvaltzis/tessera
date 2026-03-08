using TeaSharp.Styles;

namespace TeaSharp.Components;

public enum WidgetVisualState
{
    Default = 0,
    Focused = 1,
    Hovered = 2,
    Active = 3,
    Selected = 4,
    Disabled = 5,
    ReadOnly = 6,
    Loading = 7,
    Success = 8,
    Warning = 9,
    Error = 10,
    Empty = 11,
    Editing = 12,
    Expanded = 13,
    Collapsed = 14,
    Checked = 15,
    Unchecked = 16,
    Indeterminate = 17,
    Dragging = 18,
    DropTarget = 19,
    Cursor = 20,
    Marked = 21,
    Completed = 22,
    FilteredOut = 23,
    New = 24,
    Stale = 25,
}

public sealed class WidgetStateAppearance
{
    public TeaStyle TextStyle { get; set; } = TeaStyle.Empty;

    public string Prefix { get; set; } = string.Empty;

    public string Suffix { get; set; } = string.Empty;

    public bool Uppercase { get; set; }
}

public sealed class WidgetStatePalette
{
    private readonly Dictionary<WidgetVisualState, WidgetStateAppearance> _appearances = [];

    public IReadOnlyList<WidgetVisualState> Priority { get; set; } = DefaultPriority;

    public static IReadOnlyList<WidgetVisualState> DefaultPriority { get; } =
    [
        WidgetVisualState.Disabled,
        WidgetVisualState.Error,
        WidgetVisualState.Warning,
        WidgetVisualState.Success,
        WidgetVisualState.Loading,
        WidgetVisualState.ReadOnly,
        WidgetVisualState.Active,
        WidgetVisualState.Focused,
        WidgetVisualState.Cursor,
        WidgetVisualState.Selected,
        WidgetVisualState.Hovered,
        WidgetVisualState.Editing,
        WidgetVisualState.Empty,
        WidgetVisualState.Completed,
        WidgetVisualState.Marked,
        WidgetVisualState.Checked,
        WidgetVisualState.Indeterminate,
        WidgetVisualState.Unchecked,
        WidgetVisualState.Expanded,
        WidgetVisualState.Collapsed,
        WidgetVisualState.Dragging,
        WidgetVisualState.DropTarget,
        WidgetVisualState.New,
        WidgetVisualState.Stale,
        WidgetVisualState.FilteredOut,
    ];

    public WidgetStateAppearance this[WidgetVisualState state]
    {
        get
        {
            if (!_appearances.TryGetValue(state, out var appearance))
            {
                appearance = new WidgetStateAppearance();
                _appearances[state] = appearance;
            }

            return appearance;
        }
        set => _appearances[state] = value ?? new WidgetStateAppearance();
    }

    public bool TryGet(WidgetVisualState state, out WidgetStateAppearance appearance)
    {
        return _appearances.TryGetValue(state, out appearance!);
    }

    public void Set(WidgetVisualState state, WidgetStateAppearance appearance)
    {
        _appearances[state] = appearance ?? new WidgetStateAppearance();
    }

    public void Clear(WidgetVisualState state)
    {
        _appearances.Remove(state);
    }

    public string Render(string text, params WidgetVisualState[] activeStates)
    {
        return Render(text, activeStates.AsEnumerable());
    }

    public string Render(string text, IEnumerable<WidgetVisualState> activeStates)
    {
        var source = text ?? string.Empty;
        var active = new HashSet<WidgetVisualState>(activeStates);

        var style = TeaStyle.Empty;
        var upper = false;
        var prefix = string.Empty;
        var suffix = string.Empty;

        if (_appearances.TryGetValue(WidgetVisualState.Default, out var defaults))
        {
            style = style.Merge(defaults.TextStyle);
            upper |= defaults.Uppercase;
            if (!string.IsNullOrEmpty(defaults.Prefix))
            {
                prefix = defaults.Prefix;
            }

            if (!string.IsNullOrEmpty(defaults.Suffix))
            {
                suffix = defaults.Suffix;
            }
        }

        foreach (var state in Priority)
        {
            if (!active.Contains(state))
            {
                continue;
            }

            if (!_appearances.TryGetValue(state, out var appearance))
            {
                continue;
            }

            style = style.Merge(appearance.TextStyle);
            upper |= appearance.Uppercase;
            if (string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(appearance.Prefix))
            {
                prefix = appearance.Prefix;
            }

            if (string.IsNullOrEmpty(suffix) && !string.IsNullOrEmpty(appearance.Suffix))
            {
                suffix = appearance.Suffix;
            }
        }

        if (upper)
        {
            source = source.ToUpperInvariant();
        }

        var composed = $"{prefix}{source}{suffix}";
        return style.Render(composed);
    }

    public static WidgetStatePalette CreateDefault()
    {
        var palette = new WidgetStatePalette();
        palette.Set(WidgetVisualState.Focused, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithBold(),
        });
        palette.Set(WidgetVisualState.Cursor, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithInverse(),
        });
        palette.Set(WidgetVisualState.Selected, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithUnderline(),
        });
        palette.Set(WidgetVisualState.Disabled, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithDim(),
        });
        palette.Set(WidgetVisualState.ReadOnly, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithDim(),
            Suffix = " (ro)",
        });
        palette.Set(WidgetVisualState.Loading, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithItalic(),
            Suffix = " …",
        });
        palette.Set(WidgetVisualState.Success, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
        });
        palette.Set(WidgetVisualState.Warning, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightYellow),
        });
        palette.Set(WidgetVisualState.Error, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightRed),
        });
        palette.Set(WidgetVisualState.Empty, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithDim(),
        });
        palette.Set(WidgetVisualState.Completed, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithStrikethrough().WithDim(),
            Prefix = "[x] ",
        });
        palette.Set(WidgetVisualState.Marked, new WidgetStateAppearance
        {
            Prefix = "[*] ",
        });
        palette.Set(WidgetVisualState.New, new WidgetStateAppearance
        {
            Prefix = "• ",
        });
        palette.Set(WidgetVisualState.Stale, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithDim(),
            Prefix = "⌛ ",
        });
        return palette;
    }
}
