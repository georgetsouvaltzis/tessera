using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Advanced.Internal;

internal static class CommandPaletteLayout
{
    public static bool TryResolveModal(Rect bounds, out Rect modal, out Rect content)
    {
        modal = default;
        content = default;
        var clipped = bounds;
        if (clipped.IsEmpty || clipped.Width < 24 || clipped.Height < 6)
        {
            return false;
        }

        var modalWidth = Math.Min(clipped.Width - 2, Math.Max(24, clipped.Width * 2 / 3));
        var modalHeight = Math.Min(clipped.Height - 2, Math.Max(8, clipped.Height * 2 / 3));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        modal = new Rect(modalX, modalY, modalWidth, modalHeight);
        content = modal.Inset(1, 1);
        return !content.IsEmpty;
    }
}
