namespace TeaSharp.Controls;

internal interface IFocusRequestSource
{
    bool TryConsumeFocusRequest(out long order);
}
