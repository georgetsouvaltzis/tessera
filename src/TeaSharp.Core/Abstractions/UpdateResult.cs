namespace TeaSharp.Core.Abstractions;

public readonly record struct UpdateResult(IModel Model, Command? Command);
