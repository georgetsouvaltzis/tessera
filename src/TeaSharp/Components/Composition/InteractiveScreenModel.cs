using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public abstract class InteractiveScreenModel : IModel
{
    protected ScreenComposer Screen { get; } = new();

    protected InputRouter InputRouter { get; } = new();

    protected ScreenRegionKey? FocusedRegionKey => Screen.FocusedRegionKey;

    protected bool HasScreen => Screen.Regions.Count > 0;

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

    protected bool RouteFocusedMessage(IMessage message)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.Update(message);
    }

    protected void RenderScreen(Canvas canvas)
    {
        RebuildScreen();
        Screen.Render(canvas);
    }

    protected bool SetFocus(ScreenRegionKey regionKey)
    {
        EnsureScreen();
        return CanBuildScreen && Screen.SetFocus(regionKey);
    }

    protected bool FocusNext()
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusNext();
    }

    protected bool FocusPrevious()
    {
        EnsureScreen();
        return CanBuildScreen && Screen.FocusPrevious();
    }

    protected bool TryGetBounds(ScreenRegionKey regionKey, out Rect bounds)
    {
        EnsureScreen();
        if (!CanBuildScreen)
        {
            bounds = default;
            return false;
        }

        return Screen.TryGetBounds(regionKey, out bounds);
    }

    public abstract Command? Init();

    public abstract Command? Update(IMessage message);

    public abstract View View();
}
