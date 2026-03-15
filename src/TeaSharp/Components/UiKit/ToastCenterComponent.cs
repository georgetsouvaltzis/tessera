using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

internal sealed class ToastCenterComponent : IStatefulComponent
{
    private readonly List<ActiveToast> _toasts = [];

    public int MaxToasts { get; set; } = 3;

    public bool Update(IMessage message)
    {
        if (message is not TickMsg)
        {
            return false;
        }

        var changed = false;
        for (var i = _toasts.Count - 1; i >= 0; i--)
        {
            var toast = _toasts[i];
            toast.RemainingTicks--;
            if (toast.RemainingTicks <= 0)
            {
                _toasts.RemoveAt(i);
                changed = true;
                continue;
            }

            _toasts[i] = toast;
        }

        return changed;
    }

    public void Push(ToastMessage toast)
    {
        _toasts.Add(new ActiveToast(toast.Text, toast.TtlTicks, toast.Severity));
        while (_toasts.Count > Math.Max(1, MaxToasts))
        {
            _toasts.RemoveAt(0);
        }
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _toasts.Count == 0)
        {
            return;
        }

        var visible = Math.Min(_toasts.Count, clipped.Height / 3);
        for (var i = 0; i < visible; i++)
        {
            var toast = _toasts[_toasts.Count - visible + i];
            var rowTop = clipped.Y + (i * 3);
            var boxRect = new Rect(clipped.X, rowTop, clipped.Width, 3);
            var label = toast.Severity switch
            {
                ToastSeverity.Success => "OK",
                ToastSeverity.Warning => "WARN",
                ToastSeverity.Error => "ERR",
                _ => "INFO",
            };
            canvas.DrawBox(boxRect, label, BorderStyle.Rounded);
            var body = boxRect.Inset(1, 1);
            if (!body.IsEmpty)
            {
                canvas.WriteText(body.X, body.Y, toast.Text, body.Width);
            }
        }
    }

    private struct ActiveToast(string text, int remainingTicks, ToastSeverity severity)
    {
        public string Text = text;
        public int RemainingTicks = remainingTicks;
        public ToastSeverity Severity = severity;
    }
}
