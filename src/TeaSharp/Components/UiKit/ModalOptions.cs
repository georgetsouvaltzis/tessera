namespace TeaSharp.Components.UiKit;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="ModalComponent"/>.
/// </summary>
public sealed record ModalOptions(
    string Title = "Modal",
    bool Visible = false,
    BorderStyle BorderStyle = BorderStyle.Rounded,
    IReadOnlyList<string>? Lines = null,
    UiTheme? Theme = null);
