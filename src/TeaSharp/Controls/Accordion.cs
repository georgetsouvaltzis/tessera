using LegacyAccordionSection = TeaSharp.Components.UiKit.AccordionSection;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Controls;

public sealed class Accordion : Control
{
    private readonly AccordionComponent _component = new();
    private readonly List<AccordionSection> _sections = [];

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public int SelectedIndex => _component.SelectedIndex;

    public IReadOnlyList<AccordionSection> Sections => _sections;

    public void SetSections(IEnumerable<AccordionSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        _sections.Clear();
        _sections.AddRange(sections);
        _component.SetSections(_sections.Select(static section => new LegacyAccordionSection(section.Title, section.BodyLines, section.Expanded)));
    }

    public bool MoveNext() => ControlForwarder.Forward(_component, new KeyPressed(Key.Down));

    public bool MovePrevious() => ControlForwarder.Forward(_component, new KeyPressed(Key.Up));

    public bool ToggleSelected() => ControlForwarder.Forward(_component, new KeyPressed(Key.Enter));

    public override bool Handle(Message message)
    {
        return IsFocused && ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
