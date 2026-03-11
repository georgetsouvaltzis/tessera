using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition;

public interface IStatefulComponent : ICanvasComponent
{
    bool Update(IMessage message);
}

