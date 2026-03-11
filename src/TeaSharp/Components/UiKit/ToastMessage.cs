namespace TeaSharp.Components.UiKit;

public readonly record struct ToastMessage(string Text, int TtlTicks = 80, ToastSeverity Severity = ToastSeverity.Info);
