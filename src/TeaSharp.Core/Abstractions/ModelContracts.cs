using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Abstractions;

public interface IMessage;

public delegate ValueTask<IMessage?> Command(CancellationToken cancellationToken);

public interface IModel
{
    Command? Init();
    UpdateResult Update(IMessage message);
    View View();
}

public readonly record struct UpdateResult(IModel Model, Command? Command);

public enum MouseMode
{
    None = 0,
    CellMotion = 1,
    AllMotion = 2,
}

public readonly record struct View(string Content)
{
    public bool AltScreen { get; init; }
    public bool EnableBracketedPaste { get; init; }
    public bool EnableFocusReporting { get; init; }
    public bool EnableSynchronizedUpdates { get; init; }
    public MouseMode MouseMode { get; init; }
    public int? CursorX { get; init; }
    public int? CursorY { get; init; }
    public string? WindowTitle { get; init; }

    public static View From(string content) => new(content);
}
