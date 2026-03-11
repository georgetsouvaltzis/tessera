using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

/// <summary>
/// Renders a centered modal surface with a backdrop that fully occludes the underlying frame.
/// </summary>
public sealed class ModalComponent : ICanvasComponent
{
    public ModalComponent()
    {
        Theme = new UiTheme();
    }

    public ModalComponent(ModalOptions options)
    {
        Title = options.Title;
        Visible = options.Visible;
        BorderStyle = options.BorderStyle;
        Lines = options.Lines ?? ["(empty)"];
        Theme = options.Theme ?? new UiTheme();
    }

    public string Title { get; set; } = "Modal";

    public bool Visible { get; set; }

    public BorderStyle BorderStyle { get; set; } = BorderStyle.Rounded;

    public IReadOnlyList<string> Lines { get; set; } = ["(empty)"];

    public UiTheme Theme { get; set; }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!Visible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                // Modal backdrop must fully occlude underlying UI. Drawing only over
                // whitespace leaks previously rendered borders/text through the overlay.
                canvas.Set(x, y, (x + y) % 2 == 0 ? Theme.ModalBackdropFill : ' ');
            }
        }

        if (clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        var modalWidth = Math.Clamp(clipped.Width * 3 / 5, 4, Math.Max(4, clipped.Width - 2));
        var modalHeight = Math.Clamp(clipped.Height / 2, 4, Math.Max(4, clipped.Height - 2));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        var modal = new Rect(modalX, modalY, modalWidth, modalHeight);

        canvas.DrawBox(modal, Title, BorderStyle);
        var body = modal.Inset(1, 1);
        if (body.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(body.Height, Lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(body.X, body.Y + row, Lines[row], body.Width);
        }
    }
}
