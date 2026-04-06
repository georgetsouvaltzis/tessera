using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a multi-section control that expands and collapses one section at a time.
/// </summary>
public sealed class Accordion : Control
{
    private readonly List<AccordionSection> _sections = [];

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Accordion";

    /// <summary>
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the title focus marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style applied when the control is not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the title style applied when the control is focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the base style applied to section header rows.
    /// </summary>
    public TesseraStyle ItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into selected section rows.
    /// </summary>
    public TesseraStyle SelectedItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into expanded section rows.
    /// </summary>
    public TesseraStyle ExpandedItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to expanded body lines.
    /// </summary>
    public TesseraStyle BodyStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged when the control is disabled.
    /// </summary>
    public TesseraStyle DisabledItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the marker shown before the selected section.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "›";

    /// <summary>
    /// Gets or sets the marker shown before unselected sections.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets the marker shown for expanded sections.
    /// </summary>
    public string ExpandedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▾";

    /// <summary>
    /// Gets or sets the marker shown for collapsed sections.
    /// </summary>
    public string CollapsedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▸";

    public int SelectedIndex { get; private set; }

    public IReadOnlyList<AccordionSection> Sections => _sections;

    public void SetSections(IEnumerable<AccordionSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        _sections.Clear();
        _sections.AddRange(sections);
        if (SelectedIndex >= _sections.Count)
        {
            SelectedIndex = Math.Max(0, _sections.Count - 1);
        }
    }

    public bool MoveNext()
    {
        if (_sections.Count == 0)
        {
            return false;
        }

        var next = Math.Min(_sections.Count - 1, SelectedIndex + 1);
        if (next == SelectedIndex)
        {
            return false;
        }

        SelectedIndex = next;
        return true;
    }

    public bool MovePrevious()
    {
        if (_sections.Count == 0)
        {
            return false;
        }

        var previous = Math.Max(0, SelectedIndex - 1);
        if (previous == SelectedIndex)
        {
            return false;
        }

        SelectedIndex = previous;
        return true;
    }

    public bool ToggleSelected()
    {
        if (_sections.Count == 0)
        {
            return false;
        }

        var section = _sections[SelectedIndex];
        _sections[SelectedIndex] = section with { Expanded = !section.Expanded };
        return true;
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down))
        {
            return MoveNext();
        }

        if (key.Is(Key.Up))
        {
            return MovePrevious();
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            return ToggleSelected();
        }

        return false;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, RenderTitle());
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || _sections.Count == 0)
        {
            return;
        }

        var row = 0;
        for (var index = 0; index < _sections.Count && row < content.Height; index++)
        {
            var section = _sections[index];
            var selected = index == SelectedIndex ? SelectedMarker : UnselectedMarker;
            var marker = section.Expanded ? ExpandedMarker : CollapsedMarker;
            var line = $"{selected} {marker} {section.Title}";
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveSectionStyle(index, section.Expanded)), content.Width);
            row++;

            if (!section.Expanded)
            {
                continue;
            }

            for (var bodyIndex = 0; bodyIndex < section.BodyLines.Count && row < content.Height; bodyIndex++)
            {
                canvas.WriteText(
                    content.X + 2,
                    content.Y + row,
                    ApplyStyle(section.BodyLines[bodyIndex], ResolveBodyStyle()),
                    Math.Max(0, content.Width - 2));
                row++;
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(FormatTitleText(includeFocusMarkerWhenUnfocused: true)) + 4;
        var height = 2;
        var sectionPrefixWidth = Math.Max(
                ControlTextLayout.MeasureDisplayWidth(SelectedMarker),
                ControlTextLayout.MeasureDisplayWidth(UnselectedMarker))
            + 1
            + Math.Max(
                ControlTextLayout.MeasureDisplayWidth(ExpandedMarker),
                ControlTextLayout.MeasureDisplayWidth(CollapsedMarker))
            + 1;
        for (var index = 0; index < _sections.Count; index++)
        {
            var section = _sections[index];
            width = Math.Max(
                width,
                sectionPrefixWidth + ControlTextLayout.MeasureDisplayWidth(section.Title) + 2);
            height++;
            if (!section.Expanded)
            {
                continue;
            }

            for (var bodyIndex = 0; bodyIndex < section.BodyLines.Count; bodyIndex++)
            {
                width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(section.BodyLines[bodyIndex]) + 4);
                height++;
            }
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatTitleText(bool includeFocusMarkerWhenUnfocused = false)
    {
        if ((IsFocused || includeFocusMarkerWhenUnfocused) && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TesseraStyle ResolveSectionStyle(int index, bool expanded)
    {
        var style = ItemStyle;
        if (expanded)
        {
            style = style.Merge(ExpandedItemStyle);
        }

        if (index == SelectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private TesseraStyle ResolveBodyStyle()
    {
        var style = BodyStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
