using System.ComponentModel;

namespace TeaSharp.Core.Abstractions;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IScreen
{
    Effect? Init();
    Effect? Update(IMessage message);
    ScreenOutput Render();
}
