using System.ComponentModel;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Application;

/// <summary>
/// Runs a TeaSharp screen inside the runtime event loop.
/// </summary>
internal sealed class TeaProgram
{
    private readonly IScreen _screen;
    private readonly TeaRuntimeLoop _runtime;

    /// <summary>
    /// Initializes a program for the provided screen.
    /// </summary>
    /// <param name="initialScreen">The initial application screen.</param>
    /// <param name="options">Advanced runtime options.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal TeaProgram(IScreen initialScreen, TeaRuntimeLoopOptions? options = null)
    {
        _screen = initialScreen ?? throw new ArgumentNullException(nameof(initialScreen));
        _runtime = new TeaRuntimeLoop(_screen.Init, _screen.Update, _screen.Render, options);
    }

    internal TeaProgram(
        Func<Effect?>? initialize,
        Func<IMessage, Effect?> update,
        Func<ScreenOutput> render,
        TeaRuntimeLoopOptions? options = null)
        : this(new DelegateScreen(initialize, update, render), options)
    {
    }

    /// <summary>
    /// Gets the current application screen.
    /// </summary>
    internal IScreen Screen => _screen;

    /// <summary>
    /// Enqueues a message for delivery to the running program.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    internal void Send(IMessage message)
    {
        _runtime.Send(message);
    }

    /// <summary>
    /// Runs the program until it exits or the provided token is canceled.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels program execution.</param>
    /// <returns>The final application screen.</returns>
    internal async Task<IScreen> RunAsync(CancellationToken cancellationToken = default)
    {
        await _runtime.RunAsync(cancellationToken).ConfigureAwait(false);
        return _screen;
    }

    /// <summary>
    /// Requests program shutdown and waits for runtime cleanup to complete.
    /// </summary>
    /// <param name="kill">When <see langword="true"/>, forces terminal teardown without a graceful quit message.</param>
    /// <param name="cancellationToken">A token that cancels the stop operation.</param>
    internal Task StopAsync(bool kill = false, CancellationToken cancellationToken = default) =>
        _runtime.StopAsync(kill, cancellationToken);

    private sealed class DelegateScreen : IScreen
    {
        private readonly Func<Effect?>? _initialize;
        private readonly Func<IMessage, Effect?> _update;
        private readonly Func<ScreenOutput> _render;

        public DelegateScreen(
            Func<Effect?>? initialize,
            Func<IMessage, Effect?> update,
            Func<ScreenOutput> render)
        {
            _initialize = initialize;
            _update = update ?? throw new ArgumentNullException(nameof(update));
            _render = render ?? throw new ArgumentNullException(nameof(render));
        }

        public Effect? Init() => _initialize?.Invoke();

        public Effect? Update(IMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return _update(message);
        }

        public ScreenOutput Render() => _render();
    }
}
