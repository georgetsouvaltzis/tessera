using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

public sealed class ProgramOptions
{
    public Func<IModel, IMessage, IMessage?>? Filter { get; init; }

    public int MaxFps { get; init; } = 60;

    public bool DisableRenderer { get; init; }

    public bool DisableInput { get; init; }

    public bool UseConsoleKeyEvents { get; init; } = true;

    public bool CatchCommandExceptions { get; init; } = true;

    public TimeSpan EscapeTimeout { get; init; } = TimeSpan.FromMilliseconds(50);

    public TimeSpan ResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(120);

    public IProgramRenderer? Renderer { get; init; }

    public ITerminalAdapter? Terminal { get; init; }
}
