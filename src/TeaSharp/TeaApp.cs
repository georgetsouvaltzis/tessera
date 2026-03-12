using TeaSharp.Internal;
using System.ComponentModel;

namespace TeaSharp;

public abstract class TeaApp : global::TeaSharp.Core.Abstractions.IScreen
{
    private ScreenContext _context = new();
    private ScreenOptions _runtimeScreenOptions = ScreenOptions.Empty;
    private CompiledScreen? _interactiveScreen;
    private bool _inputHandled;

    public ScreenContext Context => _context;

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
        return TeaEffectAdapter.ToCore(Update(mapped));
    }

    global::TeaSharp.Core.Abstractions.ScreenOutput global::TeaSharp.Core.Abstractions.IScreen.Render()
    {
        var rendered = Build(_context).Compile(_context, _runtimeScreenOptions.Merge(DefaultScreenOptions));
        _interactiveScreen = rendered.Interaction;
        return rendered.Output;
    }
}
