using TeaSharp.Components.UiKit;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Prebuilt.Internal;

internal readonly record struct TextInputInteractionState(
    bool WasCancelled,
    string LastCancelledValue,
    int CancelCount,
    string LastSubmittedValue,
    int SubmitCount,
    bool Handled);
