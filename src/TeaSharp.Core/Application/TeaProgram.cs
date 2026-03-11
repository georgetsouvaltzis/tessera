using System.ComponentModel;
using System.Threading.Channels;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

/// <summary>
/// Runs a TeaSharp model inside the runtime event loop.
/// </summary>
public sealed partial class TeaProgram
{
    private readonly ProgramOptions _options;
    private readonly Channel<IMessage> _messages;
    private readonly Channel<Command> _commands;
    private readonly object _stateLock = new();
    private readonly TeaCapabilityProbe _capabilityProbe = new();
    private readonly TeaProgramRuntimeState _runtime = new();
    private CancellationTokenSource? _cts;
    private bool _running;

    /// <summary>
    /// Initializes a program for the provided model.
    /// </summary>
    /// <param name="initialModel">The initial application model.</param>
    /// <param name="options">Advanced runtime options.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TeaProgram(IModel initialModel, ProgramOptions? options = null)
    {
        Model = initialModel ?? throw new ArgumentNullException(nameof(initialModel));
        _options = options ?? new ProgramOptions();
        _messages = Channel.CreateUnbounded<IMessage>();
        _commands = Channel.CreateUnbounded<Command>();
    }

    /// <summary>
    /// Gets the current application model.
    /// </summary>
    public IModel Model { get; private set; }

    /// <summary>
    /// Enqueues a message for delivery to the running program.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    public void Send(IMessage message)
    {
        if (message is not null)
        {
            _messages.Writer.TryWrite(message);
        }
    }

    /// <summary>
    /// Runs the program until it exits or the provided token is canceled.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels program execution.</param>
    /// <returns>The final application model.</returns>
    public Task<IModel> RunAsync(CancellationToken cancellationToken = default) =>
        RunProgramAsync(cancellationToken);

    /// <summary>
    /// Requests program shutdown and waits for runtime cleanup to complete.
    /// </summary>
    /// <param name="kill">When <see langword="true"/>, forces terminal teardown without a graceful quit message.</param>
    /// <param name="cancellationToken">A token that cancels the stop operation.</param>
    public Task StopAsync(bool kill = false, CancellationToken cancellationToken = default) =>
        StopProgramAsync(kill, cancellationToken);
}
