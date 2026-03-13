using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IInteractiveComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent;
