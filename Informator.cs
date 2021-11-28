using static System.Console;

namespace SqlDataExtractor
{
    using ForeignKeyMap = Dictionary<(string table, string column), (string table, string column)>;

    public static class Informator
    {
        private static ConsoleColor FrameColor = ConsoleColor.White;

        public static void PrintTables(IEnumerable<DatabaseTableMetadata> tables)
        {
            ForegroundColor = FrameColor;
            WriteLine("----------");
            WriteLine("- Tables -");
            WriteLine("----------");
            ResetColor();
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
            WriteLine("-------------");
            WriteLine("- Relations -");
            WriteLine("-------------");
            ResetColor();
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

                ForegroundColor = ConsoleColor.White;
                WriteLine($"- Data: {tbl.Name} -");
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
}