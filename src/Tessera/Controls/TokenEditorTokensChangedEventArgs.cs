namespace Tessera.Controls;

/// <summary>
///     Provides previous/current token snapshots for <see cref="TokenEditor.TokensChanged" />.
/// </summary>
public sealed class TokenEditorTokensChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a token collection change payload.
    /// </summary>
    /// <param name="previousTokens">Token values before the change.</param>
    /// <param name="tokens">Token values after the change.</param>
    public TokenEditorTokensChangedEventArgs(IReadOnlyList<TokenItem> previousTokens, IReadOnlyList<TokenItem> tokens)
    {
        PreviousTokens = Clone(previousTokens);
        Tokens = Clone(tokens);
    }

    /// <summary>
    ///     Gets token values before the change.
    /// </summary>
    public IReadOnlyList<TokenItem> PreviousTokens { get; }

    /// <summary>
    ///     Gets current token values.
    /// </summary>
    public IReadOnlyList<TokenItem> Tokens { get; }

    private static TokenItem[] Clone(IReadOnlyList<TokenItem> tokens)
    {
        var clone = new TokenItem[tokens.Count];
        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];
            clone[index] = new TokenItem(token.Value, token.IsDisabled);
        }

        return clone;
    }
}
