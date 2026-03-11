using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt.Internal;

internal static class TextInputInteractionHandler
{
    public static TextInputInteractionState Update(
        TextInputModel input,
        IMessage message,
        TextInputKeyMap keyMap,
        KeyBinding cancelKey,
        bool clearOnCancel,
        bool clearOnSubmit)
    {
        if (message is KeyPressMsg key && cancelKey.Matches(key))
        {
            var lastCancelledValue = input.Value;
            if (clearOnCancel)
            {
                input.Clear();
            }

            return new TextInputInteractionState(
                WasCancelled: true,
                LastCancelledValue: lastCancelledValue,
                CancelCount: 1,
                LastSubmittedValue: string.Empty,
                SubmitCount: 0,
                Handled: true);
        }

        var result = input.Update(message, keyMap);
        if (!result.Submitted)
        {
            return new TextInputInteractionState(
                WasCancelled: false,
                LastCancelledValue: string.Empty,
                CancelCount: 0,
                LastSubmittedValue: string.Empty,
                SubmitCount: 0,
                Handled: result.Changed);
        }

        var lastSubmittedValue = input.Value;
        if (clearOnSubmit)
        {
            input.Clear();
        }

        return new TextInputInteractionState(
            WasCancelled: false,
            LastCancelledValue: string.Empty,
            CancelCount: 0,
            LastSubmittedValue: lastSubmittedValue,
            SubmitCount: 1,
            Handled: true);
    }
}
