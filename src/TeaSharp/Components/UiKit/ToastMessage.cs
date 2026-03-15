using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.UiKit;

internal readonly record struct ToastMessage(string Text, int TtlTicks = 80, ToastSeverity Severity = ToastSeverity.Info);
