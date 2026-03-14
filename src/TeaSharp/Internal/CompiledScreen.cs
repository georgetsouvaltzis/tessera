using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Layout;

namespace TeaSharp.Internal;

internal interface ICompiledScreenInteraction
{
    bool Handle(Message message);
}

internal sealed record ScreenRenderResult(ScreenOutput Output, ICompiledScreenInteraction? Interaction);

internal readonly record struct ScreenContent(string? Text, LayoutNode? Layout);

internal interface IScreenCompiler
{
    ScreenRenderResult Compile(ScreenContent content, ScreenContext context, ScreenOptions options);
}

internal static class ScreenCompilationFactory
{
    public static IScreenCompiler CreateDefault() => new LegacyScreenCompiler();
}

internal sealed class LegacyCompiledScreen : ICompiledScreenInteraction
{
    private readonly ScreenComposer _screen;

    public LegacyCompiledScreen(ScreenComposer screen)
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

internal sealed class LegacyScreenCompiler : IScreenCompiler
{
    public ScreenRenderResult Compile(ScreenContent content, ScreenContext context, ScreenOptions options)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (content.Layout is not null)
        {
            return CompileLayout(content.Layout, context, options);
        }

        var output = new ScreenOutput(ScreenFrame.From(content.Text ?? string.Empty))
        {
            Terminal = options.ToTerminalOutput(),
        };

        return new ScreenRenderResult(output, null);
    }

    private static ScreenRenderResult CompileLayout(LayoutNode layout, ScreenContext context, ScreenOptions options)
    {
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

        return new ScreenRenderResult(output, new LegacyCompiledScreen(screen));
    }
}
