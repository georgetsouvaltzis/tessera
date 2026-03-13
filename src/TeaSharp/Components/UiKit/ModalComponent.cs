using System.ComponentModel;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

/// <summary>
/// Renders a centered modal surface with a backdrop that fully occludes the underlying frame.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ModalComponent : ICanvasComponent
{
    public ModalComponent()
    {
        Theme = new UiTheme();
    }

    public ModalComponent(ModalOptions options)
    {
        Title = options.Title;
        IsVisible = options.IsVisible;
        Border = options.Border;
        Padding = options.Padding;
        BodyLines = options.BodyLines ?? ["(empty)"];
        Theme = options.Theme ?? new UiTheme();
    }

    public string Title { get; set; } = "Modal";

    public bool IsVisible { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.Rounded;

    public Thickness Padding { get; set; }

    public IReadOnlyList<string> BodyLines { get; set; } = ["(empty)"];

    public UiTheme Theme { get; set; }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
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
                // Modal backdrop must fully occlude underlying UI.
                canvas.Set(x, y, Theme.ModalBackdropFill);
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

        FillRect(canvas, modal, ' ');
        var body = FrameLayout.DrawFrameAndResolveContent(canvas, modal, Title, Border, Padding);
        if (body.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(body.Height, BodyLines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(body.X, body.Y + row, BodyLines[row], body.Width);
        }
    }

    private static void FillRect(Canvas canvas, Rect rect, char fill)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                canvas.Set(x, y, fill);
            }
        }
    }
}
