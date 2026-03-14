using TeaSharp.Core.Abstractions;

namespace TeaSharp.Internal;

internal interface ITeaAppRuntimeScreen
{
    Effect? Init();

    Effect? Update(IMessage message);

    ScreenOutput Render();
}

internal sealed class TeaAppRuntimeScreen : ITeaAppRuntimeScreen
{
    private readonly TeaApp _app;

    public TeaAppRuntimeScreen(TeaApp app)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
    }

    public Effect? Init() => _app.InitializeCore();

    public Effect? Update(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return _app.UpdateCore(message);
    }

    public ScreenOutput Render() => _app.RenderCore();
}
