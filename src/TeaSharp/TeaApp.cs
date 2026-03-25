using TeaSharp.Internal;
using TeaSharp.Styles;

namespace TeaSharp;

/// <summary>
/// Defines the state, message handling, and screen composition contract for a TeaSharp application.
/// </summary>
/// <remarks>
/// The runtime calls <see cref="Initialize"/> once before the first render, <see cref="Update"/> for
/// application-level messages, and <see cref="Build"/> whenever the screen must be re-rendered. Input
/// routes through focused controls first. <see cref="KeyPressed"/> and <see cref="KeyReleased"/> always
/// continue to <see cref="Update"/> even when a control handles them. Handled pointer/paste messages can be
/// swallowed by controls. Use <see cref="Update"/> for global hotkeys, <see cref="Post"/> when a control
/// event should flow back through the application message pipeline, and <see cref="RequestEffect"/> when
/// app logic needs to trigger runtime work directly.
/// </remarks>
public abstract class TeaApp
{
    private ScreenContext _context = new();
    private ScreenOptions _runtimeScreenOptions = ScreenOptions.Empty;
    private TeaTheme? _runtimeTheme;
    private TeaThemeOverrides? _runtimeThemeOverrides;
    private readonly IScreenCompiler _screenCompiler = ScreenCompilationFactory.CreateDefault();
    private ICompiledScreenInteraction? _interactiveScreen;
    private readonly List<TeaEffect> _pendingEffects = [];
    private PointerActivationPolicy _pointerActivationPolicy = PointerActivationPolicy.DoubleClick;
    private TimeSpan _doubleClickTimeout = TimeSpan.FromMilliseconds(450);
    private int _doubleClickSlop = 1;
    private DateTimeOffset _lastCompletedPointerReleaseUtc = DateTimeOffset.MinValue;
    private int _lastCompletedPointerX;
    private int _lastCompletedPointerY;
    private PointerButton _lastCompletedPointerButton = PointerButton.None;
    private int _lastCompletedPointerClickCount;
    private bool _hasLastCompletedPointerClick;
    private bool _hasPendingPointerPress;
    private int _pendingPointerPressX;
    private int _pendingPointerPressY;
    private PointerButton _pendingPointerPressButton = PointerButton.None;
    private int _pendingPointerPressClickCount;

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
        ApplyRuntimeThemeContext();
    }

    internal void ConfigureRuntimeOptions(TeaRuntimeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _runtimeScreenOptions = options.Screen ?? ScreenOptions.Empty;
        ConfigureRuntimeTheme(options.Theme);
        ConfigureRuntimeThemeOverrides(options.ThemeOverrides);
        _pointerActivationPolicy = options.PointerActivationPolicy;
        _doubleClickTimeout = options.DoubleClickTimeout < TimeSpan.Zero ? TimeSpan.Zero : options.DoubleClickTimeout;
        _doubleClickSlop = Math.Max(0, options.DoubleClickSlop);
        ResetPointerClickTracking();
    }

    internal void ConfigureRuntimeTheme(TeaTheme? theme)
    {
        _runtimeTheme = theme;
        ApplyRuntimeThemeContext();
    }

    internal void ConfigureRuntimeThemeOverrides(TeaThemeOverrides? overrides)
    {
        _runtimeThemeOverrides = overrides;
        ApplyRuntimeThemeContext();
    }

    internal TeaEffect? InitializeRuntime() => Initialize();

    internal TeaEffect? UpdateRuntime(Message mapped)
    {
        var routed = TeaPeriodicEffectMessage.TryUnwrap(mapped, out var periodic);
        routed = NormalizePointerInputForRuntime(routed);
        var nextPeriodic = periodic is null
            ? null
            : TeaEffects.Periodic(periodic.Interval, periodic.Factory);

        switch (routed)
        {
            case WindowResized resized:
                _context = _context with { Width = resized.Width, Height = resized.Height };
                break;
            case FocusChanged focus:
                _context = _context with { HasFocus = focus.IsFocused };
                break;
        }

        var handledInput = _interactiveScreen?.Handle(routed) ?? false;
        var effect =
            handledInput && ShouldSkipAppUpdateForHandledInput(routed)
                ? null
                : Update(routed);

        var withPeriodic = CombineEffects(effect, nextPeriodic);
        return CombineEffects(withPeriodic, DrainRequestedEffects());
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

    private static bool ShouldSkipAppUpdateForHandledInput(Message message)
    {
        if (message is KeyPressed or KeyReleased)
        {
            return false;
        }

        return IsUserInputMessage(message);
    }

    private Message NormalizePointerInputForRuntime(Message message)
    {
        if (message is not PointerInput pointer)
        {
            return message;
        }

        var normalized = NormalizeClickCount(pointer);
        if (_pointerActivationPolicy != PointerActivationPolicy.DoubleClick)
        {
            return normalized;
        }

        if (normalized is { Kind: PointerEventKind.Press, Button: PointerButton.Left } && normalized.ClickCount < 2)
        {
            return normalized with
            {
                Kind = PointerEventKind.Motion,
                Button = PointerButton.None,
                ClickCount = 0,
            };
        }

        return normalized;
    }

    private PointerInput NormalizeClickCount(PointerInput pointer)
    {
        if (pointer.Kind == PointerEventKind.Press)
        {
            if (pointer.Button == PointerButton.None)
            {
                return pointer;
            }

            var now = DateTimeOffset.UtcNow;
            var clickCount = ResolvePressClickCount(pointer, now);
            TrackPendingPress(pointer, clickCount);
            return clickCount == pointer.ClickCount
                ? pointer
                : pointer with { ClickCount = clickCount };
        }

        if (pointer.Kind == PointerEventKind.Release)
        {
            CompletePendingClickCycle(pointer);
        }

        return pointer;
    }

    private int ResolvePressClickCount(PointerInput pointer, DateTimeOffset now)
    {
        if (!_hasLastCompletedPointerClick)
        {
            return 1;
        }

        if (pointer.Button != _lastCompletedPointerButton)
        {
            return 1;
        }

        if (now - _lastCompletedPointerReleaseUtc > _doubleClickTimeout)
        {
            return 1;
        }

        if (Math.Abs(pointer.X - _lastCompletedPointerX) > _doubleClickSlop
            || Math.Abs(pointer.Y - _lastCompletedPointerY) > _doubleClickSlop)
        {
            return 1;
        }

        return Math.Max(2, _lastCompletedPointerClickCount + 1);
    }

    private void TrackPendingPress(PointerInput pointer, int clickCount)
    {
        _hasPendingPointerPress = true;
        _pendingPointerPressX = pointer.X;
        _pendingPointerPressY = pointer.Y;
        _pendingPointerPressButton = pointer.Button;
        _pendingPointerPressClickCount = clickCount;
    }

    private void CompletePendingClickCycle(PointerInput pointer)
    {
        if (!_hasPendingPointerPress)
        {
            return;
        }

        if (pointer.Button == _pendingPointerPressButton)
        {
            _hasLastCompletedPointerClick = true;
            _lastCompletedPointerReleaseUtc = DateTimeOffset.UtcNow;
            _lastCompletedPointerX = _pendingPointerPressX;
            _lastCompletedPointerY = _pendingPointerPressY;
            _lastCompletedPointerButton = _pendingPointerPressButton;
            _lastCompletedPointerClickCount = _pendingPointerPressClickCount;
        }

        _hasPendingPointerPress = false;
        _pendingPointerPressX = 0;
        _pendingPointerPressY = 0;
        _pendingPointerPressButton = PointerButton.None;
        _pendingPointerPressClickCount = 0;
    }

    private void ResetPointerClickTracking()
    {
        _hasLastCompletedPointerClick = false;
        _lastCompletedPointerReleaseUtc = DateTimeOffset.MinValue;
        _lastCompletedPointerX = 0;
        _lastCompletedPointerY = 0;
        _lastCompletedPointerButton = PointerButton.None;
        _lastCompletedPointerClickCount = 0;
        _hasPendingPointerPress = false;
        _pendingPointerPressX = 0;
        _pendingPointerPressY = 0;
        _pendingPointerPressButton = PointerButton.None;
        _pendingPointerPressClickCount = 0;
    }

    private void ApplyRuntimeThemeContext()
    {
        _context = _context with
        {
            Theme = _runtimeTheme,
            ThemeOverrides = _runtimeThemeOverrides,
        };
    }
}
