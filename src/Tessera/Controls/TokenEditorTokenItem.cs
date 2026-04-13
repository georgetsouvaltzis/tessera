namespace Tessera.Controls;

/// <summary>
///     Represents a single token chip rendered by <see cref="TokenEditor" />.
/// </summary>
public sealed class TokenItem
{
    /// <summary>
    ///     Initializes a token item.
    /// </summary>
    /// <param name="value">Token text value.</param>
    /// <param name="isDisabled">Whether the token should render as disabled.</param>
    public TokenItem(string value, bool isDisabled = false)
    {
        Value = value;
        IsDisabled = isDisabled;
    }

    /// <summary>
    ///     Token text value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Whether this token renders with disabled styling.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value;
    }
}
