using TeaSharp.Core.Messages;

namespace TeaSharp.Internal;

internal static class TeaMessageAdapter
{
    internal sealed record CoreMessageEnvelope(Message Message) : global::TeaSharp.Core.Abstractions.IMessage;

    public static Message ToPublic(global::TeaSharp.Core.Abstractions.IMessage message)
    {
        return message switch
        {
            CoreMessageEnvelope envelope => envelope.Message,
            KeyPressMsg key => new KeyPressed((Key)key.Code, key.Text, (ModifierKeys)key.Modifiers, key.IsRepeat),
            KeyReleaseMsg key => new KeyReleased((Key)key.Code, key.Text, (ModifierKeys)key.Modifiers),
            WindowSizeMsg size => new WindowResized(size.Width, size.Height),
            MouseMsg mouse => new PointerInput((PointerEventKind)mouse.EventType, (PointerButton)mouse.Button, mouse.X, mouse.Y, (ModifierKeys)mouse.Modifiers),
            PasteStartMsg => new PasteStarted(),
            PasteEndMsg => new PasteEnded(),
            PasteMsg paste => new Pasted(paste.Content),
            FocusInMsg => new FocusChanged(true),
            FocusOutMsg => new FocusChanged(false),
            EffectErrorMsg error => new Faulted(error.Exception),
            _ => new ExternalMessage(message),
        };
    }

    public static global::TeaSharp.Core.Abstractions.IMessage ToCore(Message message)
    {
        return message switch
        {
            KeyPressed key => new KeyPressMsg((KeyCode)key.Key, key.Text, (global::TeaSharp.Core.Messages.KeyModifiers)key.Modifiers, key.IsRepeat),
            KeyReleased key => new KeyReleaseMsg((KeyCode)key.Key, key.Text, (global::TeaSharp.Core.Messages.KeyModifiers)key.Modifiers),
            WindowResized size => new WindowSizeMsg(size.Width, size.Height),
            PointerInput pointer => pointer.Kind switch
            {
                PointerEventKind.Release => new MouseReleaseMsg((global::TeaSharp.Core.Messages.MouseButton)pointer.Button, pointer.X, pointer.Y, (global::TeaSharp.Core.Messages.KeyModifiers)pointer.Modifiers),
                PointerEventKind.Motion => new MouseMotionMsg((global::TeaSharp.Core.Messages.MouseButton)pointer.Button, pointer.X, pointer.Y, (global::TeaSharp.Core.Messages.KeyModifiers)pointer.Modifiers),
                PointerEventKind.Wheel => new MouseWheelMsg((global::TeaSharp.Core.Messages.MouseButton)pointer.Button, pointer.X, pointer.Y, (global::TeaSharp.Core.Messages.KeyModifiers)pointer.Modifiers),
                _ => new MouseClickMsg((global::TeaSharp.Core.Messages.MouseButton)pointer.Button, pointer.X, pointer.Y, (global::TeaSharp.Core.Messages.KeyModifiers)pointer.Modifiers),
            },
            PasteStarted => new PasteStartMsg(),
            PasteEnded => new PasteEndMsg(),
            Pasted pasted => new PasteMsg(pasted.Content),
            FocusChanged { IsFocused: true } => new FocusInMsg(),
            FocusChanged => new FocusOutMsg(),
            Faulted faulted => new EffectErrorMsg(faulted.Exception),
            ExternalMessage { Raw: global::TeaSharp.Core.Abstractions.IMessage raw } => raw,
            _ => new CoreMessageEnvelope(message),
        };
    }
}
