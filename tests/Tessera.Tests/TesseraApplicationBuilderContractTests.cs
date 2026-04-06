namespace Tessera.Tests;

internal static class TesseraApplicationBuilderContractTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TesseraApplicationBuilder_UseAppGeneric_CreatesConfiguredAppType",
            UseAppGeneric_CreatesConfiguredAppType);
        yield return new TestCase(
            "TesseraApplicationBuilder_UseAppFactory_UsesFactoryResult",
            UseAppFactory_UsesFactoryResult);
        yield return new TestCase(
            "TesseraApplicationBuilder_UseAppInstance_RemainsSupported",
            UseAppInstance_RemainsSupported);
        yield return new TestCase(
            "TesseraApplicationBuilder_BuildWithoutUseApp_ThrowsClearError",
            BuildWithoutUseApp_ThrowsClearError);
    }

    private static Task UseAppGeneric_CreatesConfiguredAppType()
    {
        var application = TesseraApplication.CreateBuilder()
            .UseApp<FactoryApp>()
            .Build();

        TestAssert.True(application.App is FactoryApp, "UseApp<TApp>() should construct the configured app type.");
        return Task.CompletedTask;
    }

    private static Task UseAppFactory_UsesFactoryResult()
    {
        var created = new FactoryApp();
        var application = TesseraApplication.CreateBuilder()
            .UseApp(() => created)
            .Build();

        TestAssert.ReferenceSame(created, application.App, "UseApp(Func<TesseraApp>) should use the returned app instance.");
        return Task.CompletedTask;
    }

    private static Task UseAppInstance_RemainsSupported()
    {
        var app = new InstanceApp();
        var application = TesseraApplication.CreateBuilder()
            .UseApp(app)
            .Build();

        TestAssert.ReferenceSame(app, application.App, "UseApp(TesseraApp) should continue using the supplied app instance.");
        return Task.CompletedTask;
    }

    private static Task BuildWithoutUseApp_ThrowsClearError()
    {
        var builder = TesseraApplication.CreateBuilder();

        try
        {
            _ = builder.Build();
            throw new InvalidOperationException("Expected Build() without UseApp(...) to throw.");
        }
        catch (InvalidOperationException ex)
        {
            TestAssert.True(
                ex.Message.Contains("Call UseApp(...) before Build().", StringComparison.Ordinal),
                "Build() should explain that UseApp(...) must be configured first.");
        }

        return Task.CompletedTask;
    }

    private sealed class FactoryApp : TesseraApp
    {
        public override TesseraEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From("factory");
    }

    private sealed class InstanceApp : TesseraApp
    {
        public override TesseraEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From("instance");
    }
}
