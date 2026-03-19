using TeaSharp.Internal;
using TeaSharp.Styles;

namespace TeaSharp;

/// <summary>
/// Defines the state, message handling, and screen composition contract for a TeaSharp application.
/// </summary>
/// <remarks>
/// The runtime calls <see cref="Initialize"/> once before the first render, <see cref="Update"/> for
/// application-level messages, and <see cref="Build"/> whenever the screen must be re-rendered. Built-in
/// controls usually consume their own input before it reaches <see cref="Update"/>; use
/// <see cref="Post"/> when a control event should flow back through the application message pipeline, and
/// <see cref="RequestEffect"/> when app logic needs to trigger runtime work directly.
/// </remarks>
public abstract class TeaApp
{
    private ScreenContext _context = new();
    private ScreenOptions _runtimeScreenOptions = ScreenOptions.Empty;
    private TeaTheme? _runtimeTheme;
    private readonly IScreenCompiler _screenCompiler = ScreenCompilationFactory.CreateDefault();
    private ICompiledScreenInteraction? _interactiveScreen;
    private readonly List<TeaEffect> _pendingEffects = [];

    /// <summary>
    /// Gets the most recent screen context supplied by the runtime.
    /// </summary>
    public ScreenContext Context => _context;

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
    /// Posts a message to flow back through <see cref="Update"/> after the current pass completes.
    /// <para>This method does not call <see cref="Update"/> immediately; the runtime processes the message on the next pass.</para>
    /// </summary>
    /// <param name="message">The message to post.</param>
    protected void Post(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        RequestEffect(TeaEffects.Emit(message));
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

    internal void ConfigureRuntimeScreen(ScreenOptions screenOptions)
    {
        _runtimeScreenOptions = screenOptions ?? ScreenOptions.Empty;
        _context = _context with { Theme = _runtimeTheme };
    }

    internal void ConfigureRuntimeOptions(TeaRuntimeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _runtimeScreenOptions = options.Screen ?? ScreenOptions.Empty;
        ConfigureRuntimeTheme(options.Theme);
    }

    internal void ConfigureRuntimeTheme(TeaTheme? theme)
    {
        _runtimeTheme = theme;
        _context = _context with { Theme = _runtimeTheme };
    }

    internal TeaEffect? InitializeRuntime() => Initialize();

    internal TeaEffect? UpdateRuntime(Message mapped)
    {
        switch (mapped)
        {
            case WindowResized resized:
                _context = _context with { Width = resized.Width, Height = resized.Height };
                break;
            case FocusChanged focus:
                _context = _context with { HasFocus = focus.IsFocused };
                break;
        }

        var handledInput = _interactiveScreen?.Handle(mapped) ?? false;
        var effect =
            handledInput && IsUserInputMessage(mapped)
                ? null
                : Update(mapped);

        return CombineEffects(effect, DrainRequestedEffects());
    }

    internal ScreenRenderResult RenderRuntime()
    {
        var rendered = Build(_context).Compile(_screenCompiler, _context, _runtimeScreenOptions.Merge(DefaultScreenOptions));
        _interactiveScreen = rendered.Interaction;
        return rendered;
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
