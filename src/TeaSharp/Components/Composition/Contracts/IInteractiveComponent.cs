using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public interface IInteractiveComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent;

