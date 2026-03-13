using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Represents an activatable push button.
/// </summary>
/// <remarks>
/// Use <see cref="Activated"/> as the normal interaction hook. The advanced polling surface remains available
/// only for transitional interop.
/// </remarks>
public sealed class Button : Control
{
    private readonly ButtonComponent _component = new();

    /// <summary>
    /// Occurs when the button is activated by input.
    /// </summary>
    public event EventHandler? Activated
    {
        add => _component.Pressed += value;
        remove => _component.Pressed -= value;
    }

    /// <summary>
    /// Gets or sets the button label.
    /// </summary>
    public string Text
    {
        get => _component.Label;
        set => _component.Label = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the optional secondary description shown with the button.
    /// </summary>
    public string? Description
    {
        get => _component.Description;
        set => _component.Description = value;
    }

    /// <summary>
    /// Gets or sets the button border style.
    /// </summary>
    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    /// <summary>
    /// Gets or sets the inner padding applied to the button body.
    /// </summary>
    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    /// <summary>
    /// Gets how many activations have been observed by the wrapped button component.
    /// </summary>
    public int ActivationCount => _component.PressCount;

    /// <summary>
    /// Gets a value indicating whether the button is currently pressed.
    /// </summary>
    public bool IsPressed => _component.IsPressed;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => !_component.Enabled;
        set => _component.Enabled = !value;
    }

    /// <summary>
    /// Attempts to consume a pending activation from the wrapped legacy component.
    /// </summary>
    /// <returns><see langword="true"/> when an activation was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeActivation() => _component.TryConsumePress();

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        return ControlForwarder.Forward(_component, message, bounds) || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
