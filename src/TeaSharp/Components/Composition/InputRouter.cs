using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public sealed class InputRouter
{
    private readonly List<RegisteredScope> _scopes = [];
    private int _nextOrder;

    public IReadOnlyList<InputScope> Scopes => _scopes
        .OrderBy(static entry => entry.Scope.Kind)
        .ThenBy(static entry => entry.Order)
        .Select(static entry => entry.Scope)
        .ToArray();

    public InputRouter AddScope(InputScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        _scopes.Add(new RegisteredScope(scope, _nextOrder++));
        return this;
    }

    public InputRouter AddScope(
        string id,
        InputScopeKind kind,
        Func<bool> isActive,
        Func<KeyPressMsg, InputRouteResult> handleKey,
        InputScopeBehavior behavior = InputScopeBehavior.ContinueWhenUnhandled,
        Func<KeyPressMsg, bool>? blocksGlobalShortcuts = null)
    {
        return AddScope(new InputScope(id, kind, isActive, handleKey, behavior, blocksGlobalShortcuts));
    }

    public void Clear()
    {
        _scopes.Clear();
        _nextOrder = 0;
    }

    public InputRouteResult Route(KeyPressMsg key)
    {
        var blockGlobal = false;
        foreach (var entry in _scopes.OrderBy(static entry => entry.Scope.Kind).ThenBy(static entry => entry.Order))
        {
            var scope = entry.Scope;
            if (!scope.IsActive())
            {
                continue;
            }

            if (blockGlobal && scope.Kind == InputScopeKind.Global)
            {
                continue;
            }

            var result = scope.HandleKey(key);
            if (result.Handled)
            {
                return result;
            }

            if (scope.BlocksGlobalShortcuts?.Invoke(key) == true)
            {
                blockGlobal = true;
            }

            if (scope.Behavior == InputScopeBehavior.CaptureWhileActive)
            {
                return InputRouteResult.HandledWithoutCommand;
            }
        }

        return InputRouteResult.NotHandled;
    }

    private readonly record struct RegisteredScope(InputScope Scope, int Order);
}
