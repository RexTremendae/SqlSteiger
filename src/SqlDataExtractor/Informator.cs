using static System.Console;

namespace SqlDX;

using ForeignKeyMap = Dictionary<(string table, string column), (string table, string column)>;

public static class Informator
{
    private static ConsoleColor FrameColor = ConsoleColor.White;

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
            WriteLine($"{tbl.Name} ({columnListing})");
        }
        WriteLine();
    }

    public static void PrintRelations(ForeignKeyMap foreignKeys)
    {
        ForegroundColor = FrameColor;
        PrintTitle("Relations");
        foreach (var (from, to) in foreignKeys)
        {
            WriteLine($"{from.table}.{from.column} => {to.table}.{to.column}");
        }
        WriteLine();
    }

    public static async Task PrintDataAsync(IDbConnection connection, IEnumerable<DatabaseTableMetadata> tables)
    {
        const string NullMarker = "<NULL>";
        foreach (var tbl in tables)
        {
            var data = new List<string?[]>(new[] { tbl.Columns.Select(c => c.Name).ToArray() });
            var maxSize = tbl.Columns.Select(c => c.Name.Length).ToArray();

            await using var reader = await connection.ExecuteReaderAsync(tbl.CreateSelectQuery());
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

            PrintSubtitle($"Data: {tbl.Name}");
            ForegroundColor = ConsoleColor.White;
            WriteLine("".PadLeft(maxSize.Sum() + tbl.Columns.Length*3 + 1, '-'));

            var firstRow = true;
            foreach (var row in data)
            {
                ForegroundColor = ConsoleColor.White;
                Write("|");
                ResetColor();
                for (int i = 0; i < maxSize.Length; i ++)
                {
                    if (row[i] == null)
                    {
                        ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Write($" {(row[i] ?? NullMarker).PadRight(maxSize[i])} ");
                    ForegroundColor = ConsoleColor.White;
                    Write("|");
                    ResetColor();
                }
                WriteLine();

                if (firstRow)
                {
                    ForegroundColor = ConsoleColor.White;
                    WriteLine("".PadLeft(maxSize.Sum() + row.Length*3 + 1, '-'));
                    firstRow = false;
                    ResetColor();
                }
            }
            ForegroundColor = ConsoleColor.White;
            WriteLine("".PadLeft(maxSize.Sum() + tbl.Columns.Length*3 + 1, '-'));
            ResetColor();
            WriteLine();
        }
    }
}
