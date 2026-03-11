using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

public sealed class AccordionComponent : IStatefulComponent
{
    private readonly List<AccordionSection> _sections = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Accordion";

    public KeyBinding NextSectionKey { get; set; } = new("down", "next section", "down");

    public KeyBinding PreviousSectionKey { get; set; } = new("up", "previous section", "up");

    public KeyBinding ToggleSectionKey { get; set; } = new("enter/space", "toggle section", "enter", "space");

    public void SetSections(IEnumerable<AccordionSection> sections)
    {
        _sections.Clear();
        _sections.AddRange(sections);
        if (SelectedIndex >= _sections.Count)
        {
            SelectedIndex = Math.Max(0, _sections.Count - 1);
        }
    }

    public bool Update(IMessage message)
    {
        if (_sections.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextSectionKey.Matches(key))
        {
            SelectedIndex = Math.Min(_sections.Count - 1, SelectedIndex + 1);
            return true;
        }

        if (PreviousSectionKey.Matches(key))
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
            return true;
        }

        if (ToggleSectionKey.Matches(key))
        {
            var section = _sections[SelectedIndex];
            _sections[SelectedIndex] = section with { Expanded = !section.Expanded };
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || _sections.Count == 0)
        {
            return;
        }

        var row = 0;
        for (var i = 0; i < _sections.Count && row < content.Height; i++)
        {
            var section = _sections[i];
            var selected = i == SelectedIndex ? "›" : " ";
            var marker = section.Expanded ? "▾" : "▸";
            canvas.WriteText(content.X, content.Y + row, $"{selected} {marker} {section.Title}", content.Width);
            row++;

            if (section.Expanded)
            {
                for (var j = 0; j < section.Lines.Count && row < content.Height; j++)
                {
                    canvas.WriteText(content.X + 2, content.Y + row, section.Lines[j], Math.Max(0, content.Width - 2));
                    row++;
                }
            }
        }
    }
}

