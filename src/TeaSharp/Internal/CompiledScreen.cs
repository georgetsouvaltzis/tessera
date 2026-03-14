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
    public static IScreenCompiler CreateDefault() => new TeaSceneCompiler();
}
