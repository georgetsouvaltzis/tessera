using TeaSharp.Internal;
using System.ComponentModel;

namespace TeaSharp;

/// <summary>
/// Defines the state, message handling, and screen composition contract for a TeaSharp application.
/// </summary>
/// <remarks>
/// The runtime calls <see cref="Initialize"/> once before the first render, <see cref="Update"/> for
/// application-level messages, and <see cref="Build"/> whenever the screen must be re-rendered. Built-in
/// controls usually consume their own input before it reaches <see cref="Update"/>; use
/// <see cref="RequestEffect"/> or <see cref="UpdateHandledInput"/> only when a control interaction needs to
/// trigger runtime work.
/// </remarks>
public abstract class TeaApp
{
    private ScreenContext _context = new();
    private ScreenOptions _runtimeScreenOptions = ScreenOptions.Empty;
    private CompiledScreen? _interactiveScreen;
    private bool _inputHandled;
    private readonly List<TeaEffect> _pendingEffects = [];
    private global::TeaSharp.Core.Abstractions.IScreen? _runtimeScreen;

    /// <summary>
    /// Gets the most recent screen context supplied by the runtime.
    /// </summary>
    public ScreenContext Context => _context;

    /// <summary>
    /// Gets a value indicating whether the current input message was already consumed by the compiled screen tree.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool InputHandled => _inputHandled;

    /// <summary>
    /// Gets the default screen options applied to every screen built by this application.
    /// </summary>
    public virtual ScreenOptions DefaultScreenOptions => ScreenOptions.Empty;

    /// <summary>
    /// Produces the initial effect that runs before the first render.
    /// </summary>
    public virtual TeaEffect? Initialize() => null;

    /// <summary>
    /// Updates application state in response to a message and optionally returns a follow-up effect.
    /// </summary>
    /// <param name="message">The message raised by input, runtime, or an effect.</param>
    /// <returns>The effect to schedule after the update, or <see langword="null"/>.</returns>
    public abstract TeaEffect? Update(Message message);

    /// <summary>
    /// Builds the current screen from application state and runtime context.
    /// </summary>
    /// <param name="context">The current runtime context.</param>
    /// <returns>The screen to render.</returns>
    public abstract Screen Build(ScreenContext context);

    /// <summary>
    /// Routes an input message through the compiled screen tree.
    /// </summary>
    /// <param name="message">The input message to route.</param>
    /// <returns><see langword="true"/> when a control handled the message; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool HandleScreenInput(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return _interactiveScreen?.Handle(message) ?? false;
    }

    /// <summary>
    /// Queues an effect to be emitted after the current update pass completes.
    /// </summary>
    /// <param name="effect">The effect to enqueue.</param>
    protected void RequestEffect(TeaEffect? effect)
    {
        if (effect is not null)
        {
            _pendingEffects.Add(effect);
        }
    }

    /// <summary>
    /// Produces a follow-up effect for input already handled by the compiled screen tree.
    /// </summary>
    /// <param name="message">The handled input message.</param>
    /// <returns>The effect to schedule after the handled input, or <see langword="null"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual TeaEffect? UpdateHandledInput(Message message) => null;

    internal void ConfigureRuntimeScreen(ScreenOptions screenOptions)
    {
        _runtimeScreenOptions = screenOptions ?? ScreenOptions.Empty;
    }

    internal global::TeaSharp.Core.Abstractions.IScreen RuntimeScreen =>
        _runtimeScreen ??= new TeaAppRuntimeScreen(this);

    internal global::TeaSharp.Core.Abstractions.Effect? InitializeCore() =>
        TeaEffectAdapter.ToCore(Initialize());

    internal global::TeaSharp.Core.Abstractions.Effect? UpdateCore(global::TeaSharp.Core.Abstractions.IMessage message)
    {
        var mapped = TeaMessageAdapter.ToPublic(message);
        switch (mapped)
        {
            case WindowResized resized:
                _context = _context with { Width = resized.Width, Height = resized.Height };
                break;
            case FocusChanged focus:
                _context = _context with { HasFocus = focus.IsFocused };
                break;
        }

        _inputHandled = _interactiveScreen?.Handle(mapped) ?? false;
        var effect =
            _inputHandled && IsUserInputMessage(mapped)
                ? UpdateHandledInput(mapped)
                : Update(mapped);

        return TeaEffectAdapter.ToCore(CombineEffects(effect, DrainRequestedEffects()));
    }

    internal global::TeaSharp.Core.Abstractions.ScreenOutput RenderCore()
    {
        var rendered = Build(_context).Compile(_context, _runtimeScreenOptions.Merge(DefaultScreenOptions));
        _interactiveScreen = rendered.Interaction;
        return rendered.Output;
    }

    private TeaEffect? DrainRequestedEffects()
    {
        if (_pendingEffects.Count == 0)
        {
            return null;
        }

        var pending = _pendingEffects.ToArray();
        _pendingEffects.Clear();
        return TeaEffects.Batch(pending);
    }

    private static TeaEffect? CombineEffects(TeaEffect? first, TeaEffect? second)
    {
        if (first is null)
        {
            return second;
        }

        return second is null
            ? first
            : TeaEffects.Batch(first, second);
    }

    private static bool IsUserInputMessage(Message message) =>
        message is KeyPressed
            or KeyReleased
            or PointerInput
            or PasteStarted
            or PasteEnded
            or Pasted;
}
