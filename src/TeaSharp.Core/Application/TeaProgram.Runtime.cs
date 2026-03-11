using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

public sealed partial class TeaProgram
{
    private Task? StartInputLoop(CancellationToken token)
    {
        if (_options.DisableInput || _runtime.Terminal is null || !_runtime.Terminal.IsInputInteractive)
        {
            return null;
        }

        if (_runtime.Terminal is ConsoleTerminalAdapter consoleTerminal
            && (_options.UseConsoleKeyEvents || !consoleTerminal.IsRawModeActive))
        {
            return Task.Run(() => ConsoleTerminalAdapter.StreamConsoleKeyEventsAsync(Send, token), token);
        }

        _runtime.Reader = new TerminalReader(_runtime.Terminal.Input, _options.EventDecoder ?? new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _runtime.Reader.StreamEventsAsync(Send, token), token);
    }

    private async Task RenderAsync(View view, CancellationToken token)
    {
        if (_runtime.Renderer is null)
        {
            return;
        }

        _runtime.LastRenderedView = view;
        _runtime.Renderer.Render(view);
        await _runtime.Renderer.FlushAsync(token).ConfigureAwait(false);
    }

    private async Task ShutdownAsync(bool kill, CancellationToken token)
    {
        _commands.Writer.TryComplete();
        _messages.Writer.TryComplete();

        if (_runtime.Renderer is not null)
        {
            if (!kill)
            {
                await _runtime.Renderer.ResetAsync(token).ConfigureAwait(false);
            }

            await _runtime.Renderer.DisposeAsync().ConfigureAwait(false);
            _runtime.Renderer = null;
        }

        if (_runtime.Terminal is not null)
        {
            await _runtime.Terminal.RestoreAsync(token).ConfigureAwait(false);
            await _runtime.Terminal.DisposeAsync().ConfigureAwait(false);
            _runtime.Terminal = null;
        }

        _runtime.Reader = null;
        _runtime.LastRenderedView = View.From(string.Empty);
    }
}
