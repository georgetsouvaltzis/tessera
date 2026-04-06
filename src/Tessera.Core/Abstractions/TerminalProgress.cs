namespace Tessera.Core.Abstractions;

public readonly record struct TerminalProgress(
    TerminalProgressState State,
    int Value);
