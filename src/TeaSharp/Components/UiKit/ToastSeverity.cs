using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

internal enum ToastSeverity
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
}
