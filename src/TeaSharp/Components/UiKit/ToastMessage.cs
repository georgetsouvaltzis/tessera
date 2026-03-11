using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
namespace TeaSharp.Components.UiKit;

public readonly record struct ToastMessage(string Text, int TtlTicks = 80, ToastSeverity Severity = ToastSeverity.Info);
