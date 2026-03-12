using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Layout;

namespace TeaSharp.Internal;

internal sealed class CompiledScreen
{
    private readonly ScreenComposer _screen;

    public CompiledScreen(ScreenComposer screen)
    {
        _screen = screen ?? throw new ArgumentNullException(nameof(screen));
    }

    public bool Handle(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message is KeyPressed key)
        {
            if (key.Is(Key.Tab, ModifierKeys.Shift))
            {
                return _screen.FocusPrevious();
            }

            if (key.Is(Key.Tab))
            {
                return _screen.FocusNext();
            }
        }

        return _screen.Update(TeaMessageAdapter.ToCore(message));
    }
}

internal sealed record ScreenRenderResult(ScreenOutput Output, CompiledScreen? Interaction);

internal static class ScreenCompiler
{
    public static ScreenRenderResult Compile(LayoutNode layout, ScreenContext context, ScreenOptions options)
    {
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(context);

        var canvas = context.CreateCanvas(CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var screen = new ScreenComposer();
        screen.BeginFrame();
        layout.Compose(screen, canvas.Bounds, "root");
        screen.CompleteFrame();
        screen.Render(canvas);

        var output = new ScreenOutput(ScreenFrame.From(canvas.Render()))
        {
            Terminal = options.ToTerminalOutput(),
        };

        return new ScreenRenderResult(output, new CompiledScreen(screen));
    }
}
