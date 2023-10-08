using static System.Console;

namespace SqlDX;

using ForeignKeyMap = Dictionary<(string schema, string table, string column), (string schema, string table, string column)>;

public static class Informator
{
    private static readonly ConsoleColor FrameColor = ConsoleColor.White;

    public static void PrintTitle(string title)
    {
        ForegroundColor = FrameColor;
        WriteLine("".PadLeft(title.Length + 6, '-'));
        WriteLine($"-- {title} --");
        WriteLine("".PadLeft(title.Length + 6, '-'));
        ResetColor();
    }

    public static void PrintSubtitle(string subtitle)
    {
        ForegroundColor = ConsoleColor.White;
        WriteLine($"-- {subtitle} --");
    }

    public static void PrintTables(IEnumerable<DatabaseTableMetadata> tables)
    {
        PrintTitle("Tables");
        foreach (var tbl in tables)
        {
            var columnListing = string.Join(", ",
                tbl.Columns.Select(c => $"{c.Name} [{c.SqlDataType}::{c.CSharpDataType}]"));
            WriteLine($"{tbl.Schema}.{tbl.Name} ({columnListing})");
        }
        WriteLine();
    }

    public static void PrintRelations(ForeignKeyMap foreignKeys)
    {
        ForegroundColor = FrameColor;
        PrintTitle("Relations");
        foreach (var (from, to) in foreignKeys)
        {
            WriteLine($"{from.schema}.{from.table}.{from.column} => {to.schema}.{to.table}.{to.column}");
        }
        WriteLine();
    }

    public static async Task PrintDataAsync(IDbConnection connection, IEnumerable<DatabaseTableMetadata> tables, int? maxRows = null, int? maxColumnWidth = null)
    {
        const string NullMarker = "<NULL>";
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
                data.Add(dataRow.ToArray());
            }

            if (maxColumnWidth.HasValue)
            {
                for (int i = 0; i < tbl.Columns.Length; i++)
                {
                    maxSize[i] = int.Min(maxSize[i], maxColumnWidth.Value);
                }
            }

            PrintSubtitle($"Data: {tbl.Schema}.{tbl.Name}");
            ForegroundColor = ConsoleColor.White;
            WriteLine("".PadLeft(maxSize.Sum() + (tbl.Columns.Length * 3) + 1, '-'));

            var firstRow = true;
            foreach (var row in data)
            {
                ForegroundColor = ConsoleColor.White;
                Write("|");
                ResetColor();
                for (int i = 0; i < maxSize.Length; i++)
                {
                    if (row[i] == null)
                    {
                        ForegroundColor = ConsoleColor.DarkGray;
                    }

                    var printableData = (row[i] ?? NullMarker).PadRight(maxSize[i]);
                    if (printableData.Length > maxSize[i])
                    {
                        printableData = printableData[..maxSize[i]];
                    }

                    Write($" {printableData} ");
                    ForegroundColor = ConsoleColor.White;
                    Write("|");
                    ResetColor();
                }
                WriteLine();

                if (firstRow)
                {
                    ForegroundColor = ConsoleColor.White;
                    WriteLine("".PadLeft(maxSize.Sum() + (row.Length * 3) + 1, '-'));
                    firstRow = false;
                    ResetColor();
                }
            }
            ForegroundColor = ConsoleColor.White;
            WriteLine("".PadLeft(maxSize.Sum() + (tbl.Columns.Length * 3) + 1, '-'));
            ResetColor();
            WriteLine();
        }
    }
}
