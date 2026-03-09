using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public interface IFocusableComponent : ICanvasComponent
{
    bool Focused { get; set; }
}

