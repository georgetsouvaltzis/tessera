using Tessera.Controls;

namespace Tessera.Benchmarks;

internal static class BenchmarkDataFactory
{
    public static IReadOnlyList<DataGridColumn> CreateColumns(int count, int width = 12)
    {
        var columns = new DataGridColumn[count];
        for (var index = 0; index < count; index++)
        {
            columns[index] = new DataGridColumn($"c{index + 1}", $"Col {index + 1}")
            {
                Width = width,
                IsSortable = true,
                SortComparer = static (left, right) => string.CompareOrdinal(left, right)
            };
        }

        return columns;
    }

    public static IReadOnlyList<IReadOnlyList<string>> CreateRows(int rowCount, int columnCount, int seed)
    {
        var random = new Random(seed);
        var rows = new IReadOnlyList<string>[rowCount];
        for (var row = 0; row < rowCount; row++)
        {
            var cells = new string[columnCount];
            for (var column = 0; column < columnCount; column++)
            {
                cells[column] = $"R{row:D4}-C{column:D2}-{random.Next(1000, 9999)}";
            }

            rows[row] = cells;
        }

        return rows;
    }

    public static IReadOnlyList<KeyValueListEntry> CreateInspectorEntries(int count)
    {
        var entries = new KeyValueListEntry[count];
        for (var index = 0; index < count; index++)
        {
            entries[index] = new KeyValueListEntry($"Prop{index:D2}", $"Value-{index * 7:D4}");
        }

        return entries;
    }
}
