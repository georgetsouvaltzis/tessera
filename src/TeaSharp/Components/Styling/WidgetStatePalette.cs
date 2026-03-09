using TeaSharp.Styles;

namespace TeaSharp.Components;

public sealed class WidgetStatePalette
{
    private readonly Dictionary<WidgetVisualState, WidgetStateAppearance> _appearances = [];
    private WidgetStatePalette? _parent;

    public IReadOnlyList<WidgetVisualState> Priority { get; set; } = DefaultPriority;

    public WidgetStatePalette? Parent
    {
        get => _parent;
        set
        {
            EnsureNoCycle(value);
            _parent = value;
        }
    }

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

    public bool TryGetInherited(WidgetVisualState state, out WidgetStateAppearance appearance)
    {
        var hierarchy = BuildHierarchyRootFirst();
        return TryResolveAppearance(state, hierarchy, out appearance);
    }

    public void Set(WidgetVisualState state, WidgetStateAppearance appearance)
    {
        _appearances[state] = appearance ?? new WidgetStateAppearance();
    }

    public void Clear(WidgetVisualState state)
    {
        _appearances.Remove(state);
    }

    public WidgetStatePalette InheritFrom(WidgetStatePalette? parent)
    {
        Parent = parent;
        return this;
    }

    public string Render(string text, params WidgetVisualState[] activeStates)
    {
        return Render(text, activeStates.AsEnumerable());
    }

    public string Render(string text, IEnumerable<WidgetVisualState> activeStates)
    {
        var source = text ?? string.Empty;
        var active = new HashSet<WidgetVisualState>(activeStates);
        var hierarchy = BuildHierarchyRootFirst();

        var style = TeaStyle.Empty;
        var upper = false;
        var prefix = string.Empty;
        var suffix = string.Empty;

        if (TryResolveAppearance(WidgetVisualState.Default, hierarchy, out var defaults))
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

            if (!TryResolveAppearance(state, hierarchy, out var appearance))
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

    private static bool TryResolveAppearance(
        WidgetVisualState state,
        IReadOnlyList<WidgetStatePalette> hierarchy,
        out WidgetStateAppearance appearance)
    {
        var found = false;
        var style = TeaStyle.Empty;
        var upper = false;
        var prefix = string.Empty;
        var suffix = string.Empty;

        for (var i = 0; i < hierarchy.Count; i++)
        {
            if (!hierarchy[i]._appearances.TryGetValue(state, out var local))
            {
                continue;
            }

            found = true;
            style = style.Merge(local.TextStyle);
            upper |= local.Uppercase;
            if (!string.IsNullOrEmpty(local.Prefix))
            {
                prefix = local.Prefix;
            }

            if (!string.IsNullOrEmpty(local.Suffix))
            {
                suffix = local.Suffix;
            }
        }

        if (!found)
        {
            appearance = null!;
            return false;
        }

        appearance = new WidgetStateAppearance
        {
            TextStyle = style,
            Uppercase = upper,
            Prefix = prefix,
            Suffix = suffix,
        };
        return true;
    }

    private IReadOnlyList<WidgetStatePalette> BuildHierarchyRootFirst()
    {
        var chain = new List<WidgetStatePalette>(4);
        var seen = new HashSet<WidgetStatePalette>();

        var current = this;
        while (current is not null)
        {
            if (!seen.Add(current))
            {
                throw new InvalidOperationException("WidgetStatePalette parent cycle detected.");
            }

            chain.Add(current);
            current = current._parent;
        }

        chain.Reverse();
        return chain;
    }

    private void EnsureNoCycle(WidgetStatePalette? candidateParent)
    {
        var current = candidateParent;
        while (current is not null)
        {
            if (ReferenceEquals(current, this))
            {
                throw new InvalidOperationException("WidgetStatePalette cannot inherit from itself (cycle detected).");
            }

            current = current._parent;
        }
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
        palette.Set(WidgetVisualState.Hovered, new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithUnderline(),
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
