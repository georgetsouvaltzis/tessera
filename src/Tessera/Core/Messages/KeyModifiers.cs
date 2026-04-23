namespace Tessera.Core.Messages;

/// <summary>
///     Represents modifier keys held during keyboard or mouse input.
/// </summary>
[Flags]
public enum KeyModifiers
{
    /// <summary>No modifiers are active.</summary>
    None = 0,

    /// <summary>The Shift modifier is active.</summary>
    Shift = 1 << 0,

    /// <summary>The Alt modifier is active.</summary>
    Alt = 1 << 1,

    /// <summary>The Control modifier is active.</summary>
    Ctrl = 1 << 2,

    /// <summary>The Meta or Command modifier is active.</summary>
    Meta = 1 << 3
}
