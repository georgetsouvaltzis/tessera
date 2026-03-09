namespace TeaSharp.Core.Abstractions;

public interface IModel
{
    Command? Init();
    Command? Update(IMessage message);
    View View();
}
