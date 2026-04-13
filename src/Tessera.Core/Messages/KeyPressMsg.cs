using System.Text;
using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
///     Represents a key press received from the terminal.
/// </summary>
/// <param name="Code">The logical key code.</param>
/// <param name="Text">The text payload produced by the key, when any.</param>
/// <param name="Modifiers">The active modifier keys.</param>
/// <param name="IsRepeat">Whether the key press is an auto-repeat event.</param>
public sealed record KeyPressMsg(
    KeyCode Code,
    string Text = "",
    KeyModifiers Modifiers = KeyModifiers.None,
    bool IsRepeat = false) : IMessage
{
    /// <summary>
    ///     Checks whether the message matches a specific key code and modifier set.
    /// </summary>
    /// <param name="code">The expected key code.</param>
    /// <param name="modifiers">The expected modifiers.</param>
    /// <returns><see langword="true" /> when the key and modifiers match.</returns>
    public bool Is(KeyCode code, KeyModifiers modifiers = KeyModifiers.None)
    {
        return Code == code && Modifiers == modifiers;
    }

    /// <summary>
    ///     Checks whether the key press represents a specific character.
    /// </summary>
    /// <param name="character">The expected character.</param>
    /// <param name="ignoreCase">Whether comparison should ignore casing.</param>
    /// <returns><see langword="true" /> when the key press matches the character.</returns>
    public bool IsCharacter(char character, bool ignoreCase = true)
    {
        if (Code != KeyCode.Character || Text.Length != 1)
        {
            return false;
        }

        var value = Text[0];
        return ignoreCase
            ? char.ToLowerInvariant(value) == char.ToLowerInvariant(character)
            : value == character;
    }

    /// <summary>
    ///     Checks whether the key press represents a specific character with a specific modifier set.
    /// </summary>
    /// <param name="character">The expected character.</param>
    /// <param name="modifiers">The expected modifiers.</param>
    /// <param name="ignoreCase">Whether comparison should ignore casing.</param>
    /// <returns><see langword="true" /> when the key press matches.</returns>
    public bool IsCharacter(char character, KeyModifiers modifiers, bool ignoreCase = true)
    {
        return Modifiers == modifiers && IsCharacter(character, ignoreCase);
    }

    /// <summary>
    ///     Attempts to parse the key press as a decimal digit.
    /// </summary>
    /// <param name="oneBasedDigit">Receives the parsed digit when successful.</param>
    /// <returns><see langword="true" /> when the key press contains a single digit.</returns>
    public bool TryGetDigit(out int oneBasedDigit)
    {
        oneBasedDigit = 0;
        if (Code != KeyCode.Character || Text.Length != 1)
        {
            return false;
        }

        var c = Text[0];
        if (!char.IsDigit(c))
        {
            return false;
        }

        oneBasedDigit = c - '0';
        return true;
    }

    /// <summary>
    ///     Formats the key press as a normalized keystroke string.
    /// </summary>
    /// <returns>A lowercase keystroke representation such as <c>ctrl+c</c>.</returns>
    public string Keystroke()
    {
        var parts = new List<string>(4);
        if (Modifiers.HasFlag(KeyModifiers.Ctrl))
        {
            parts.Add("ctrl");
        }

        if (Modifiers.HasFlag(KeyModifiers.Alt))
        {
            parts.Add("alt");
        }

        if (Modifiers.HasFlag(KeyModifiers.Shift))
        {
            parts.Add("shift");
        }

        if (Modifiers.HasFlag(KeyModifiers.Meta))
        {
            parts.Add("meta");
        }

        var key = Code == KeyCode.Character
            ? Text
            : Code.ToString().ToLowerInvariant();

        parts.Add(string.IsNullOrEmpty(key) ? "unknown" : key);

        var sb = new StringBuilder();
        for (var i = 0; i < parts.Count; i++)
        {
            if (i > 0)
            {
                sb.Append('+');
            }

            sb.Append(parts[i]);
        }

        return sb.ToString();
    }
}
