namespace TeaSharp.Core.Abstractions;

public readonly record struct TerminalProgress(
    TerminalProgressState State,
    int Value);
