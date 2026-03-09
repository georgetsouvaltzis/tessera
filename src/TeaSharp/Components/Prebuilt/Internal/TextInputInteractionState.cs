namespace TeaSharp.Components.Internal;

internal readonly record struct TextInputInteractionState(
    bool WasCancelled,
    string LastCancelledValue,
    int CancelCount,
    string LastSubmittedValue,
    int SubmitCount,
    bool Handled);
