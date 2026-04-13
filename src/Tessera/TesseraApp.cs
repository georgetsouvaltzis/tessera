using Tessera.Internal;
using Tessera.Styles;

namespace Tessera;

/// <summary>
///     Defines the state, message handling, and screen composition contract for a Tessera application.
/// </summary>
/// <remarks>
///     The runtime calls <see cref="Initialize" /> once before the first render, <see cref="Update" /> for
///     application-level messages, and <see cref="Build" /> whenever the screen must be re-rendered. Input
///     routes through focused controls first. <see cref="KeyPressed" /> and <see cref="KeyReleased" /> always
///     continue to <see cref="Update" /> even when a control handles them. Handled pointer/paste messages can be
///     swallowed by controls. Use <see cref="Update" /> for global hotkeys, <see cref="Post" /> when a control
///     event should flow back through the application message pipeline, and <see cref="RequestEffect" /> when
///     app logic needs to trigger runtime work directly.
/// </remarks>
public abstract class TesseraApp
{
    private readonly List<TesseraEffect> _pendingEffects = [];
    private readonly IScreenCompiler _screenCompiler = ScreenCompilationFactory.CreateDefault();
    private int _doubleClickSlop = 1;
    private TimeSpan _doubleClickTimeout = TimeSpan.FromMilliseconds(450);
    private bool _hasLastCompletedPointerClick;
    private bool _hasPendingPointerPress;
    private ICompiledScreenInteraction? _interactiveScreen;
    private PointerButton _lastCompletedPointerButton = PointerButton.None;
    private int _lastCompletedPointerClickCount;
    private DateTimeOffset _lastCompletedPointerReleaseUtc = DateTimeOffset.MinValue;
    private int _lastCompletedPointerX;
    private int _lastCompletedPointerY;
    private PointerButton _pendingPointerPressButton = PointerButton.None;
    private int _pendingPointerPressClickCount;
    private int _pendingPointerPressX;
    private int _pendingPointerPressY;
    private PointerActivationPolicy _pointerActivationPolicy = PointerActivationPolicy.DoubleClick;
    private ScreenOptions _runtimeScreenOptions = ScreenOptions.Empty;
    private TesseraTheme? _runtimeTheme;
    private TesseraThemeOverrides? _runtimeThemeOverrides;

    /// <summary>
    ///     Gets the most recent screen context supplied by the runtime.
    /// </summary>
    public ScreenContext Context { get; private set; } = new();

    /// <summary>
    ///     Gets the default screen options applied to every screen built by this application.
    /// </summary>
    public virtual ScreenOptions DefaultScreenOptions => ScreenOptions.Empty;

    /// <summary>
    ///     Produces the initial effect that runs before the first render.
    /// </summary>
    public virtual TesseraEffect? Initialize()
    {
        return null;
    }

    /// <summary>
    ///     Updates application state in response to a message and optionally returns a follow-up effect.
    /// </summary>
    /// <param name="message">The message raised by input, runtime, or an effect.</param>
    /// <returns>The effect to schedule after the update, or <see langword="null" />.</returns>
    public abstract TesseraEffect? Update(Message message);

    /// <summary>
    ///     Builds the current screen from application state and runtime context.
    /// </summary>
    /// <param name="context">The current runtime context.</param>
    /// <returns>The screen to render.</returns>
    public abstract Screen Build(ScreenContext context);

    /// <summary>
    ///     Posts a message to flow back through <see cref="Update" /> after the current pass completes.
    ///     <para>
    ///         This method does not call <see cref="Update" /> immediately; the runtime processes the message on the next
    ///         pass.
    ///     </para>
    /// </summary>
    /// <param name="message">The message to post.</param>
    protected void Post(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        RequestEffect(TesseraEffects.Emit(message));
    }

    /// <summary>
    ///     Queues an effect to be emitted after the current update pass completes.
    /// </summary>
    /// <param name="effect">The effect to enqueue.</param>
    protected void RequestEffect(TesseraEffect? effect)
    {
        if (effect is not null)
        {
            _pendingEffects.Add(effect);
        }
    }

    internal void ConfigureRuntimeScreen(ScreenOptions screenOptions)
    {
        _runtimeScreenOptions = screenOptions;
        ApplyRuntimeThemeContext();
    }

    internal void ConfigureRuntimeOptions(TesseraRuntimeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _runtimeScreenOptions = options.Screen;
        ConfigureRuntimeTheme(options.Theme);
        ConfigureRuntimeThemeOverrides(options.ThemeOverrides);
        _pointerActivationPolicy = options.PointerActivationPolicy;
        _doubleClickTimeout = options.DoubleClickTimeout < TimeSpan.Zero ? TimeSpan.Zero : options.DoubleClickTimeout;
        _doubleClickSlop = Math.Max(0, options.DoubleClickSlop);
        ResetPointerClickTracking();
    }

    internal void ConfigureRuntimeTheme(TesseraTheme? theme)
    {
        _runtimeTheme = theme;
        ApplyRuntimeThemeContext();
    }

    internal void ConfigureRuntimeThemeOverrides(TesseraThemeOverrides? overrides)
    {
        _runtimeThemeOverrides = overrides;
        ApplyRuntimeThemeContext();
    }

    internal TesseraEffect? InitializeRuntime()
    {
        return Initialize();
    }

    internal TesseraEffect? UpdateRuntime(Message mapped)
    {
        var routed = TesseraPeriodicEffectMessage.TryUnwrap(mapped, out var periodic);
        routed = NormalizePointerInputForRuntime(routed);
        var nextPeriodic = periodic is null
            ? null
            : TesseraEffects.Periodic(periodic.Interval, periodic.Factory);

        switch (routed)
        {
            case WindowResized resized:
                Context = Context with { Width = resized.Width, Height = resized.Height };
                break;
            case FocusChanged focus:
                Context = Context with { HasFocus = focus.IsFocused };
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
        var rendered = Build(Context)
            .Compile(_screenCompiler, Context, _runtimeScreenOptions.Merge(DefaultScreenOptions));
        _interactiveScreen = rendered.Interaction;
        return rendered;
    }

    private TesseraEffect? DrainRequestedEffects()
    {
        if (_pendingEffects.Count == 0)
        {
            return null;
        }

        var pending = _pendingEffects.ToArray();
        _pendingEffects.Clear();
        return TesseraEffects.Batch(pending);
    }

    private static TesseraEffect? CombineEffects(TesseraEffect? first, TesseraEffect? second)
    {
        if (first is null)
        {
            return second;
        }

        return second is null
            ? first
            : TesseraEffects.Batch(first, second);
    }

    private static bool IsUserInputMessage(Message message)
    {
        return message is KeyPressed
            or KeyReleased
            or PointerInput
            or PasteStarted
            or PasteEnded
            or Pasted;
    }

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
            return normalized with { Kind = PointerEventKind.Motion, Button = PointerButton.None, ClickCount = 0 };
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

        if (IsReleaseCompletingPendingPress(pointer))
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

    private bool IsReleaseCompletingPendingPress(PointerInput pointer)
    {
        return pointer.Button == _pendingPointerPressButton
               || pointer.Button == PointerButton.None;
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
        Context = Context with { Theme = _runtimeTheme, ThemeOverrides = _runtimeThemeOverrides };
    }
}
