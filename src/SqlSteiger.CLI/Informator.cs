namespace SqlSteiger.CLI;

using static SqlSteiger.CLI.ColorWriter;

using ForeignKeyMap = Dictionary<(string Schema, string Table, string Column), (string Schema, string Table, string Column)>;

public static class Informator
{
    private const string NullMarker = "<NULL>";

    private static readonly ConsoleColor FrameColor = ConsoleColor.White;
    private static readonly ConsoleColor NullColor = ConsoleColor.DarkGray;
    private static readonly ConsoleColor DataColor = ConsoleColor.Gray;

    public static void PrintTitle(string title)
    {
        WriteLine(string.Empty.PadLeft(title.Length + 6, '-'), FrameColor);
        WriteLine($"-- {title} --", FrameColor);
        WriteLine(string.Empty.PadLeft(title.Length + 6, '-'), FrameColor);
    }

    public static void PrintSubtitle(string subtitle)
    {
        WriteLine($"-- {subtitle} --", FrameColor);
    }

    public static void PrintTables(IEnumerable<DatabaseTableMetadata> tables)
    {
        PrintTitle("Tables");
        foreach (var tbl in tables)
        {
            var columnListing = string.Join(
                ", ",
                tbl.Columns.Select(c => $"{c.Name} [{c.SqlDataType}::{c.CSharpDataType}]"));
            WriteLine($"{tbl.Schema}.{tbl.Name} ({columnListing})");
        }

        WriteLine();
    }

    public static void PrintRelations(ForeignKeyMap foreignKeys)
    {
        PrintTitle("Relations");
        foreach (var (from, to) in foreignKeys)
        {
            Write($"{from.Schema}.{from.Table}.{from.Column}");
            Write(" => ", FrameColor);
            WriteLine($"{to.Schema}.{to.Table}.{to.Column}");
        }

        WriteLine();
    }

    public static async Task PrintDataAsync(
        IDbConnection connection,
        IEnumerable<DatabaseTableMetadata> tables,
        int? maxRows = null,
        int? maxColumnWidth = null)
    {
        foreach (var tbl in tables)
        {
            var data = new List<string?[]>(new[] { tbl.Columns.Select(c => c.Name).ToArray() });
            var maxSize = tbl.Columns.Select(c => c.Name.Length).ToArray();

            await using var reader = await connection.ExecuteReaderAsync(tbl.CreateSelectQuery(maxRows: maxRows));
            while (await reader.ReadAsync())
            {
                var dataRow = new List<string?>();
                for (int i = 0; i < tbl.Columns.Length; i++)
                {
                    var col = tbl.Columns[i];
                    dataRow.Add(reader.GetValue(col.Name, col.CSharpDataType)?.ToString());
                    maxSize[i] = Math.Max(maxSize[i], dataRow[i]?.Length ?? NullMarker.Length);
                }

                data.Add([.. dataRow]);
            }

            if (maxColumnWidth.HasValue)
            {
                for (int i = 0; i < tbl.Columns.Length; i++)
                {
                    maxSize[i] = int.Min(maxSize[i], maxColumnWidth.Value);
                }
            }

            PrintSubtitle($"Data: {tbl.Schema}.{tbl.Name}");
            WriteLine(
                string.Empty.PadLeft(maxSize.Sum() + (tbl.Columns.Length * 3) + 1, '-'),
                FrameColor);

            var firstRow = true;
            foreach (var row in data)
            {
                Write("|", FrameColor);
                for (int i = 0; i < maxSize.Length; i++)
                {
                    var printableData = (row[i] ?? NullMarker).PadRight(maxSize[i]);
                    if (printableData.Length > maxSize[i])
                    {
                        printableData = printableData[..maxSize[i]];
                    }

                    Write($" {printableData} ", row[i] == null ? NullColor : DataColor);
                    Write("|", FrameColor);
                }

                WriteLine();

                if (firstRow)
                {
                    WriteLine(
                        string.Empty.PadLeft(maxSize.Sum() + (row.Length * 3) + 1, '-'),
                        FrameColor);
                    firstRow = false;
                }
            }

            WriteLine(
                string.Empty.PadLeft(maxSize.Sum() + (tbl.Columns.Length * 3) + 1, '-'),
                FrameColor);
            WriteLine();
        }
    }
}
