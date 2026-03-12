using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Components.Composition;
using TeaSharp.Internal;
using TeaSharp.Layout;

namespace TeaSharp;

public sealed class Screen
{
    private readonly string? _text;
    private readonly LayoutNode? _layout;

    private Screen(string? text = null, LayoutNode? layout = null)
    {
        _text = text;
        _layout = layout;
    }

    public ScreenOptions Options { get; init; } = ScreenOptions.Empty;

    public static Screen Empty { get; } = new(string.Empty);

    public static Screen From(string content) => new(content ?? string.Empty);

    public static Screen From(Canvas canvas) => new(canvas.Render());

    public static Screen From(LayoutNode layout) => new(layout: layout ?? throw new ArgumentNullException(nameof(layout)));

    public static Screen From(ICanvasComponent component) =>
        new(layout: new ComponentLayout(component ?? throw new ArgumentNullException(nameof(component))));

    internal ScreenRenderResult Compile(ScreenContext context, ScreenOptions defaults)
    {
        if (_layout is not null)
        {
            return ScreenCompiler.Compile(_layout, context, defaults.Merge(Options));
        }

        var output = new ScreenOutput(ScreenFrame.From(_text ?? string.Empty))
        {
            Terminal = defaults.Merge(Options).ToTerminalOutput(),
        };

        return new ScreenRenderResult(output, null);
    }
}
