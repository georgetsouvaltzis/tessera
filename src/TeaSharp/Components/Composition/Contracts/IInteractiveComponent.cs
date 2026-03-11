using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition;

public interface IInteractiveComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent;

