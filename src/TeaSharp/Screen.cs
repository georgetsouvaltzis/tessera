using TeaSharp.Components.Primitives;

namespace TeaSharp;

public sealed record Screen(string Content)
{
    public ScreenOptions Options { get; init; } = ScreenOptions.Empty;

    public static Screen Empty { get; } = new(string.Empty);

    public static Screen From(string content) => new(content);

    public static Screen From(Canvas canvas) => new(canvas.Render());

    internal global::TeaSharp.Core.Abstractions.ScreenOutput ToCore(ScreenOptions defaults)
    {
        return new global::TeaSharp.Core.Abstractions.ScreenOutput(
            global::TeaSharp.Core.Abstractions.ScreenFrame.From(Content))
        {
            Terminal = defaults.Merge(Options).ToTerminalOutput(),
        };
    }
}
