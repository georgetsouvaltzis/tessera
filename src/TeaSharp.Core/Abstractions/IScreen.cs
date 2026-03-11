namespace TeaSharp.Core.Abstractions;

public interface IScreen
{
    Effect? Init();
    Effect? Update(IMessage message);
    ScreenOutput Render();
}
