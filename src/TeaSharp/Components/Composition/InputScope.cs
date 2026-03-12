using TeaSharp.Components.Primitives;
using TeaSharp.Core.Messages;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class InputScope
{
    public InputScope(
        string id,
        InputScopeKind kind,
        Func<bool> isActive,
        Func<KeyPressMsg, InputRouteResult> handleKey,
        InputScopeBehavior behavior = InputScopeBehavior.ContinueWhenUnhandled,
        Func<KeyPressMsg, bool>? blocksGlobalShortcuts = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(isActive);
        ArgumentNullException.ThrowIfNull(handleKey);

        Id = id;
        Kind = kind;
        IsActive = isActive;
        HandleKey = handleKey;
        Behavior = behavior;
        BlocksGlobalShortcuts = blocksGlobalShortcuts;
    }

    public string Id { get; }

    public InputScopeKind Kind { get; }

    public Func<bool> IsActive { get; }

    public Func<KeyPressMsg, InputRouteResult> HandleKey { get; }

    public InputScopeBehavior Behavior { get; }

    public Func<KeyPressMsg, bool>? BlocksGlobalShortcuts { get; }
}
