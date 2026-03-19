using Microsoft.Extensions.DependencyInjection;

namespace TeaSharp.Tests;

internal static class TeaApplicationBuilderDiTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TeaApplicationBuilder_UseAppGeneric_ResolvesConstructorDependenciesFromServicesCollection",
            UseAppGeneric_ResolvesConstructorDependenciesFromServicesCollection);
        yield return new TestCase(
            "TeaApplicationBuilder_ConfigureServices_RegistersDependenciesConsumedByApp",
            ConfigureServices_RegistersDependenciesConsumedByApp);
        yield return new TestCase(
            "TeaApplicationBuilder_UseAppInstance_RemainsSupported",
            UseAppInstance_RemainsSupported);
        yield return new TestCase(
            "TeaApplicationBuilder_BuildWithoutUseApp_ThrowsClearError",
            BuildWithoutUseApp_ThrowsClearError);
    }

    private static Task UseAppGeneric_ResolvesConstructorDependenciesFromServicesCollection()
    {
        var application = Tea.CreateBuilder()
            .ConfigureServices(static services => services.AddSingleton<ITestDependency>(new TestDependency("services")))
            .UseApp<DependencyInjectedApp>()
            .Build();

        var app = (DependencyInjectedApp)application.App;
        TestAssert.Equal("services", app.DependencyValue, "UseApp<TApp>() should construct the app through DI.");
        return Task.CompletedTask;
    }

    private static Task ConfigureServices_RegistersDependenciesConsumedByApp()
    {
        var application = Tea.CreateBuilder()
            .ConfigureServices(static services =>
            {
                services.AddSingleton<ITestDependency>(new TestDependency("configure-callback"));
            })
            .UseApp<DependencyInjectedApp>()
            .Build();

        var app = (DependencyInjectedApp)application.App;
        TestAssert.Equal(
            "configure-callback",
            app.DependencyValue,
            "ConfigureServices should register constructor dependencies used by UseApp<TApp>().");
        return Task.CompletedTask;
    }

    private static Task UseAppInstance_RemainsSupported()
    {
        var app = new InstanceApp();
        var application = Tea.CreateBuilder()
            .UseApp(app)
            .Build();

        TestAssert.ReferenceSame(app, application.App, "UseApp(TeaApp) should continue using the supplied app instance.");
        return Task.CompletedTask;
    }

    private static Task BuildWithoutUseApp_ThrowsClearError()
    {
        var builder = Tea.CreateBuilder();

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

    private interface ITestDependency
    {
        string Value { get; }
    }

    private sealed class TestDependency(string value) : ITestDependency
    {
        public string Value { get; } = value;
    }

    private sealed class DependencyInjectedApp(ITestDependency dependency) : TeaApp
    {
        public string DependencyValue { get; } = dependency.Value;

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From("di");
    }

    private sealed class InstanceApp : TeaApp
    {
        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From("instance");
    }
}
