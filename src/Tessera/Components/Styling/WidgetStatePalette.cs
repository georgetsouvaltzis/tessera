using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Tessera.Components.Styling.Internal;
using Tessera.Styles;

namespace Tessera.Components.Styling;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class WidgetStatePalette
{
    private readonly Dictionary<WidgetVisualState, WidgetStateAppearance> _appearances = [];
    private WidgetStatePalette? _parent;

    public IReadOnlyList<WidgetVisualState> Priority { get; set; } = DefaultPriority;

    public WidgetStatePalette? Parent
    {
        get => _parent;
        set
        {
            WidgetStatePaletteHierarchy.EnsureNoCycle(this, value);
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
        WidgetVisualState.FilteredOut
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
        set => _appearances[state] = value;
    }

    public bool TryGet(WidgetVisualState state, [NotNullWhen(true)] out WidgetStateAppearance? appearance)
    {
        return _appearances.TryGetValue(state, out appearance);
    }

    public bool TryGetInherited(WidgetVisualState state, [NotNullWhen(true)] out WidgetStateAppearance? appearance)
    {
        var hierarchy = WidgetStatePaletteHierarchy.BuildRootFirst(this);
        return WidgetStatePaletteResolver.TryResolveAppearance(
            state,
            hierarchy,
            static (palette, visualState) => palette.ResolveAppearance(visualState),
            out appearance);
    }

    public void Set(WidgetVisualState state, WidgetStateAppearance appearance)
    {
        _appearances[state] = appearance;
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
        var hierarchy = WidgetStatePaletteHierarchy.BuildRootFirst(this);
        return WidgetStatePaletteResolver.Render(
            text,
            activeStates,
            Priority,
            hierarchy,
            static (palette, visualState) => palette.ResolveAppearance(visualState));
    }

    public static WidgetStatePalette CreateDefault()
    {
        var palette = new WidgetStatePalette();
        palette.Set(WidgetVisualState.Focused, new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithBold() });
        palette.Set(WidgetVisualState.Cursor,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithInverse() });
        palette.Set(WidgetVisualState.Hovered,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithUnderline() });
        palette.Set(WidgetVisualState.Selected,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithUnderline() });
        palette.Set(WidgetVisualState.Disabled, new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithDim() });
        palette.Set(WidgetVisualState.ReadOnly,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithDim(), Suffix = " (ro)" });
        palette.Set(WidgetVisualState.Loading,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithItalic(), Suffix = " …" });
        palette.Set(WidgetVisualState.Success,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen) });
        palette.Set(WidgetVisualState.Warning,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightYellow) });
        palette.Set(WidgetVisualState.Error,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightRed) });
        palette.Set(WidgetVisualState.Empty, new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithDim() });
        palette.Set(WidgetVisualState.Completed,
            new WidgetStateAppearance
            {
                TextStyle = TesseraStyle.Empty.WithStrikethrough().WithDim(),
                Prefix = "[x] "
            });
        palette.Set(WidgetVisualState.Marked, new WidgetStateAppearance { Prefix = "[*] " });
        palette.Set(WidgetVisualState.New, new WidgetStateAppearance { Prefix = "• " });
        palette.Set(WidgetVisualState.Stale,
            new WidgetStateAppearance { TextStyle = TesseraStyle.Empty.WithDim(), Prefix = "⌛ " });
        return palette;
    }

    internal WidgetStateAppearance? ResolveAppearance(WidgetVisualState state)
    {
        return _appearances.TryGetValue(state, out var appearance)
            ? appearance
            : null;
    }
}
