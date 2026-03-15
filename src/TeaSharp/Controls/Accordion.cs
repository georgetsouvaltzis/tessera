using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

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
        if (!IsFocused || message is not KeyPressed key)
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
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || _sections.Count == 0)
        {
            return;
        }

        var row = 0;
        for (var index = 0; index < _sections.Count && row < content.Height; index++)
        {
            var section = _sections[index];
            var selected = index == SelectedIndex ? "›" : " ";
            var marker = section.Expanded ? "▾" : "▸";
            canvas.WriteText(content.X, content.Y + row, $"{selected} {marker} {section.Title}", content.Width);
            row++;

            if (!section.Expanded)
            {
                continue;
            }

            for (var bodyIndex = 0; bodyIndex < section.BodyLines.Count && row < content.Height; bodyIndex++)
            {
                canvas.WriteText(content.X + 2, content.Y + row, section.BodyLines[bodyIndex], Math.Max(0, content.Width - 2));
                row++;
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Title.Length + 4;
        var height = 2;
        for (var index = 0; index < _sections.Count; index++)
        {
            var section = _sections[index];
            width = Math.Max(width, section.Title.Length + 4);
            height++;
            if (!section.Expanded)
            {
                continue;
            }

            for (var bodyIndex = 0; bodyIndex < section.BodyLines.Count; bodyIndex++)
            {
                width = Math.Max(width, section.BodyLines[bodyIndex].Length + 4);
                height++;
            }
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
