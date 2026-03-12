using TeaSharp.Internal;
using System.ComponentModel;

namespace TeaSharp;

public abstract class TeaApp : global::TeaSharp.Core.Abstractions.IScreen
{
    private ScreenContext _context = new();
    private ScreenOptions _runtimeScreenOptions = ScreenOptions.Empty;
    private CompiledScreen? _interactiveScreen;
    private bool _inputHandled;
    private readonly List<TeaEffect> _pendingEffects = [];

    public ScreenContext Context => _context;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool InputHandled => _inputHandled;

    public virtual ScreenOptions DefaultScreenOptions => ScreenOptions.Empty;

    public virtual TeaEffect? Initialize() => null;

    public abstract TeaEffect? Update(Message message);

    public abstract Screen Build(ScreenContext context);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool HandleScreenInput(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return _interactiveScreen?.Handle(message) ?? false;
    }

    protected void RequestEffect(TeaEffect? effect)
    {
        if (effect is not null)
        {
            _pendingEffects.Add(effect);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual TeaEffect? UpdateHandledInput(Message message) => null;

    internal void ConfigureRuntimeScreen(ScreenOptions screenOptions)
    {
        _runtimeScreenOptions = screenOptions ?? ScreenOptions.Empty;
    }

    global::TeaSharp.Core.Abstractions.Effect? global::TeaSharp.Core.Abstractions.IScreen.Init()
    {
        return TeaEffectAdapter.ToCore(Initialize());
    }

    global::TeaSharp.Core.Abstractions.Effect? global::TeaSharp.Core.Abstractions.IScreen.Update(global::TeaSharp.Core.Abstractions.IMessage message)
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

    global::TeaSharp.Core.Abstractions.ScreenOutput global::TeaSharp.Core.Abstractions.IScreen.Render()
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
