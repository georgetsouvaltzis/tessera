using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using System.ComponentModel;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a modal dialog surface with accept and dismiss actions.
/// </summary>
public sealed class Dialog : Control
{
    private long _resultVersion;
    private long _consumedResultVersion;
    private List<string> _bodyLines = ["Confirm?"];

    /// <summary>
    /// Occurs when the dialog is accepted.
    /// </summary>
    public event EventHandler? Accepted;

    /// <summary>
    /// Occurs when the dialog is dismissed.
    /// </summary>
    public event EventHandler? Dismissed;

    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Dialog";

    /// <summary>
    /// Gets or sets the dialog body lines.
    /// </summary>
    public IReadOnlyList<string> BodyLines
    {
        get => _bodyLines;
        set => _bodyLines = [.. (value ?? ["Confirm?"])];
    }

    /// <summary>
    /// Gets or sets the dialog border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.Rounded;

    /// <summary>
    /// Gets or sets the inner padding applied to the dialog body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is visible.
    /// </summary>
    public bool IsVisible
    {
        get;
        set;
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    public DialogResult LastResult { get; private set; }

    /// <summary>
    /// Shows the dialog with the supplied title and body lines.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="lines">The body lines to display.</param>
    public void Show(string title, params string[] lines)
    {
        Title = title;
        BodyLines = lines;
        IsVisible = true;
        RequestFocus();
    }

    /// <summary>
    /// Hides the dialog.
    /// </summary>
    public void Hide()
    {
        IsVisible = false;
    }

    /// <summary>
    /// Attempts to consume the latest dialog result exactly once.
    /// </summary>
    /// <param name="result">Receives the consumed result when available.</param>
    /// <returns><see langword="true"/> when a result was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeResult(out DialogResult result)
    {
        if (_resultVersion == _consumedResultVersion)
        {
            result = DialogResult.None;
            return false;
        }

        _consumedResultVersion = _resultVersion;
        result = LastResult;
        return true;
    }

    public override bool Handle(Message message)
    {
        if (!IsVisible || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Escape))
        {
            return ApplyResult(DialogResult.Dismissed);
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            return ApplyResult(DialogResult.Accepted);
        }

        return false;
    }

    public override void Render(Canvas canvas, Rect rect)
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
                canvas.Set(x, y, '·');
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

        var rows = Math.Min(body.Height, _bodyLines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(body.X, body.Y + row, _bodyLines[row], body.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var longest = _bodyLines.Count == 0 ? 8 : _bodyLines.Max(ControlTextLayout.MeasureDisplayWidth);
        var width = Math.Max(Title.Length + 4, longest + Padding.Horizontal) + 2;
        var height = Math.Max(4, _bodyLines.Count + Padding.Vertical + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool ApplyResult(DialogResult result)
    {
        IsVisible = false;
        LastResult = result;
        _resultVersion++;
        if (result == DialogResult.Accepted)
        {
            Accepted?.Invoke(this, EventArgs.Empty);
        }
        else if (result == DialogResult.Dismissed)
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }

        return true;
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
