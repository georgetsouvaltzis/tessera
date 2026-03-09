namespace TeaSharp.Core.Abstractions;

public interface IModel
{
    Command? Init();
    UpdateResult Update(IMessage message);
    View View();
}
