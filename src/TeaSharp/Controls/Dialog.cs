using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a modal dialog surface with accept and dismiss actions.
/// </summary>
public sealed class Dialog : Control
{
    private readonly DialogComponent _component = new();

    /// <summary>
    /// Occurs when the dialog is accepted.
    /// </summary>
    public event EventHandler? Accepted
    {
        add => _component.Accepted += value;
        remove => _component.Accepted -= value;
    }

    /// <summary>
    /// Occurs when the dialog is dismissed.
    /// </summary>
    public event EventHandler? Dismissed
    {
        add => _component.Dismissed += value;
        remove => _component.Dismissed -= value;
    }

    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the dialog body lines.
    /// </summary>
    public IReadOnlyList<string> BodyLines
    {
        get => _component.BodyLines;
        set => _component.BodyLines = value ?? Array.Empty<string>();
    }

    /// <summary>
    /// Gets or sets the dialog border style.
    /// </summary>
    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    /// <summary>
    /// Gets or sets the inner padding applied to the dialog body.
    /// </summary>
    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is visible.
    /// </summary>
    public bool IsVisible
    {
        get => _component.IsVisible;
        set => _component.IsVisible = value;
    }

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

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
    }

    /// <summary>
    /// Hides the dialog.
    /// </summary>
    public void Hide()
    {
        IsVisible = false;
    }

    /// <summary>
    /// Attempts to consume a pending dialog result from the wrapped legacy component.
    /// </summary>
    /// <param name="result">Receives the consumed result when available.</param>
    /// <returns><see langword="true"/> when a result was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeResult(out DialogResult result)
    {
        if (_component.TryConsumeResult(out var current))
        {
            result = current switch
            {
                global::TeaSharp.Components.Prebuilt.DialogResult.Accepted => DialogResult.Accepted,
                global::TeaSharp.Components.Prebuilt.DialogResult.Dismissed => DialogResult.Dismissed,
                _ => DialogResult.None,
            };
            return true;
        }

        result = DialogResult.None;
        return false;
    }

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
