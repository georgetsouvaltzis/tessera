using TeaSharp.Components;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class InputRouterTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Components_InputRouter_SystemScopeCanBypassCapturedModal", InputRouter_SystemScopeCanBypassCapturedModal);
        yield return new TestCase("Components_InputRouter_CaptureScopeStopsLowerScopesWhenUnhandled", InputRouter_CaptureScopeStopsLowerScopesWhenUnhandled);
        yield return new TestCase("Components_InputRouter_FocusedScopeCanSuppressGlobalCharacterShortcuts", InputRouter_FocusedScopeCanSuppressGlobalCharacterShortcuts);
        yield return new TestCase("Components_InputRouter_FocusedScopeDoesNotSuppressGlobalNavigationKeys", InputRouter_FocusedScopeDoesNotSuppressGlobalNavigationKeys);
    }

    private static Task InputRouter_SystemScopeCanBypassCapturedModal()
    {
        var calls = new List<string>();
        var router = new InputRouter()
            .AddScope("system", InputScopeKind.System, static () => true, key =>
            {
                calls.Add("system");
                return key.Modifiers.HasFlag(KeyModifiers.Ctrl)
                    ? InputRouteResult.FromCommand(Tea.Cmd.Quit)
                    : InputRouteResult.NotHandled;
            })
            .AddScope("modal", InputScopeKind.Modal, static () => true, key =>
            {
                calls.Add("modal");
                return InputRouteResult.NotHandled;
            }, InputScopeBehavior.CaptureWhileActive);

        var result = router.Route(new KeyPressMsg(KeyCode.Character, "c", KeyModifiers.Ctrl));

        TestAssert.True(result.Handled, "System scope should handle emergency shortcuts before modal capture.");
        TestAssert.True(result.Command is not null, "System scope should be able to return a command.");
        TestAssert.Equal(1, calls.Count, "Modal scope should not run after a system hit.");
        TestAssert.Equal("system", calls[0], "System scope should run first.");
        return Task.CompletedTask;
    }

    private static Task InputRouter_CaptureScopeStopsLowerScopesWhenUnhandled()
    {
        var calls = new List<string>();
        var router = new InputRouter()
            .AddScope("modal", InputScopeKind.Modal, static () => true, key =>
            {
                calls.Add("modal");
                return InputRouteResult.NotHandled;
            }, InputScopeBehavior.CaptureWhileActive)
            .AddScope("global", InputScopeKind.Global, static () => true, key =>
            {
                calls.Add("global");
                return InputRouteResult.HandledWithoutCommand;
            });

        var result = router.Route(new KeyPressMsg(KeyCode.Character, "q"));

        TestAssert.True(result.Handled, "Capture scopes should consume keys even when their handlers do not match them.");
        TestAssert.Equal(1, calls.Count, "Lower scopes should not run after an active capture scope.");
        TestAssert.Equal("modal", calls[0], "Capture scope should be the only scope invoked.");
        return Task.CompletedTask;
    }

    private static Task InputRouter_FocusedScopeCanSuppressGlobalCharacterShortcuts()
    {
        var calls = new List<string>();
        var router = new InputRouter()
            .AddScope(
                "focused",
                InputScopeKind.FocusedRegion,
                static () => true,
                key =>
                {
                    calls.Add("focused");
                    return InputRouteResult.NotHandled;
                },
                blocksGlobalShortcuts: key => key.Modifiers == KeyModifiers.None && key.Code == KeyCode.Character)
            .AddScope("global", InputScopeKind.Global, static () => true, key =>
            {
                calls.Add("global");
                return InputRouteResult.HandledWithoutCommand;
            });

        var result = router.Route(new KeyPressMsg(KeyCode.Character, "q"));

        TestAssert.True(!result.Handled, "Suppressed global character shortcuts should not fall through when focused text entry wants to keep them local.");
        TestAssert.Equal(1, calls.Count, "Global scope should be skipped when focused scope blocks character shortcuts.");
        TestAssert.Equal("focused", calls[0], "Focused scope should evaluate before global shortcuts.");
        return Task.CompletedTask;
    }

    private static Task InputRouter_FocusedScopeDoesNotSuppressGlobalNavigationKeys()
    {
        var calls = new List<string>();
        var router = new InputRouter()
            .AddScope(
                "focused",
                InputScopeKind.FocusedRegion,
                static () => true,
                key =>
                {
                    calls.Add("focused");
                    return InputRouteResult.NotHandled;
                },
                blocksGlobalShortcuts: key => key.Modifiers == KeyModifiers.None && key.Code == KeyCode.Character)
            .AddScope("global", InputScopeKind.Global, static () => true, key =>
            {
                calls.Add("global");
                return InputRouteResult.HandledWithoutCommand;
            });

        var result = router.Route(new KeyPressMsg(KeyCode.Tab));

        TestAssert.True(result.Handled, "Navigation keys should still fall through to global policy when focused scope only blocks text shortcuts.");
        TestAssert.Equal(2, calls.Count, "Global scope should still run for non-blocked keys.");
        TestAssert.Equal("focused", calls[0], "Focused scope should run before global scope.");
        TestAssert.Equal("global", calls[1], "Global scope should receive non-blocked navigation keys.");
        return Task.CompletedTask;
    }
}
