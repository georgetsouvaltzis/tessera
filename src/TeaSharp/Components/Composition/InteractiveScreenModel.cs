using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public abstract class InteractiveScreenModel : IModel
{
    protected ScreenComposer Screen { get; } = new();

    protected InputRouter InputRouter { get; } = new();

    protected ScreenRegionKey? FocusedRegionKey => Screen.FocusedRegionKey;

    protected abstract Rect GetBodyRect();

    protected abstract void ComposeScreen(Rect bodyRect);

    protected virtual ScreenRegionKey? PreferredFocusRegionKey => null;

    protected virtual bool CanBuildScreen => true;

    protected void EnsureScreen()
    {
        if (Screen.Regions.Count == 0 && CanBuildScreen)
        {
            RebuildScreen();
        }
    }

    protected void RebuildScreen()
    {
        if (!CanBuildScreen)
        {
            return;
        }

        Screen.BeginFrame();
        ComposeScreen(GetBodyRect());
        Screen.CompleteFrame(PreferredFocusRegionKey);
    }

    protected Command? RouteKey(KeyPressMsg key)
    {
        EnsureScreen();
        var routed = InputRouter.Route(key);
        return routed.Handled ? routed.Command : null;
    }

    protected bool RouteMouse(MouseMsg mouse)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.Update(mouse);
    }

    protected void RenderScreen(Canvas canvas)
    {
        RebuildScreen();
        Screen.Render(canvas);
    }

    public abstract Command? Init();

    public abstract Command? Update(IMessage message);

    public abstract View View();
}
