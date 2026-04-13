namespace Tessera.Tests;

public sealed record TestCase(string Name, Func<Task> Execute);

internal sealed class TestRunner
{
    private readonly List<TestCase> _cases = [];

    public void Add(TestCase testCase)
    {
        _cases.Add(testCase);
    }

    public void AddRange(IEnumerable<TestCase> testCases)
    {
        _cases.AddRange(testCases);
    }

    public async Task<int> RunAsync()
    {
        var failures = new List<string>();

        foreach (var testCase in _cases)
        {
            try
            {
                await testCase.Execute();
                await Console.Out.WriteLineAsync($"[PASS] {testCase.Name}");
            }
            catch (Exception ex)
            {
                failures.Add($"{testCase.Name}: {ex.Message}");
                await Console.Out.WriteLineAsync($"[FAIL] {testCase.Name}");
            }
        }

        if (failures.Count == 0)
        {
            await Console.Out.WriteLineAsync("Tessera tests passed.");
            return 0;
        }

        await Console.Error.WriteLineAsync("Tessera tests failed:");
        foreach (var failure in failures)
        {
            await Console.Error.WriteLineAsync($"- {failure}");
        }

        return 1;
    }
}

internal static class TestAssert
{
    public static void Equal<T>(T expected, T actual, string message)
        where T : IEquatable<T>
    {
        if (!expected.Equals(actual))
        {
            throw new InvalidOperationException($"{message}. Expected={expected}, Actual={actual}.");
        }
    }

    public static void ReferenceSame(object expected, object actual, string message)
    {
        if (!ReferenceEquals(expected, actual))
        {
            throw new InvalidOperationException(message);
        }
    }

    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
