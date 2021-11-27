using Microsoft.Data.SqlClient;
using SqlDataExtractor;
using SqlDataExtractor.SqlDatabase;
using static System.Console;

var connectionBuilder = new SqlConnectionStringBuilder();

connectionBuilder.DataSource = "localhost";
connectionBuilder.UserID = "sa";
connectionBuilder.Password = "";
connectionBuilder.IntegratedSecurity = false;
connectionBuilder.InitialCatalog = "";

// Seems to be needed when connecting to a local SQL Server instance running in docker
connectionBuilder.TrustServerCertificate = true;

await using IDbConnection connection = new SqlDbConnection(connectionBuilder.ToString());

await connection.OpenAsync();
var databaseStructureExtractor = new DatabaseStructureExtractor(connection);

var tables = await databaseStructureExtractor.ExtractTableMap();
var foreignKeys = await databaseStructureExtractor.ExtractForeignKeyMap();

WriteLine("----------");
WriteLine("- Tables -");
WriteLine("----------");
foreach (var (name, tbl) in tables)
{
    var columnListing = string.Join(", ",
        tbl.Columns.Select(c => $"{c.Name} [{c.SqlDataType}::{c.CSharpDataType}]"));
    WriteLine($"{tbl.Name} ({columnListing})");
}
WriteLine();

WriteLine("-------------");
WriteLine("- Relations -");
WriteLine("-------------");
foreach (var (from, to) in foreignKeys)
{
    WriteLine($"{from} => {to}");
}
WriteLine();


foreach (var (_, tbl) in tables)
{
    var data = new List<string[]>(new[] { tbl.Columns.Select(c => c.Name).ToArray() });
    var maxSize = tbl.Columns.Select(c => c.Name.Length).ToArray();

    var cmd = connection.CreateCommand(tbl.CreateSelectQuery());
    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        var dataRow = new List<string>();
        for (int i = 0; i < tbl.Columns.Length; i++)
        {
            var col = tbl.Columns[i];
            dataRow.Add(reader.GetValue(col.Name, col.CSharpDataType)?.ToString() ?? "<NULL>");
            maxSize[i] = Math.Max(maxSize[i], dataRow[i].Length);
        }
        data.Add(dataRow.ToArray());
    }

    WriteLine($"- Data: {tbl.Name} -");
    WriteLine("".PadLeft(maxSize.Sum() + tbl.Columns.Length*3 + 1, '-'));

    var firstRow = true;
    foreach (var row in data)
    {
        Write("|");
        for (int i = 0; i < maxSize.Length; i ++)
        {
            Write($" {row[i].PadRight(maxSize[i])} |");
        }
        WriteLine();

        if (firstRow)
        {
            WriteLine("".PadLeft(maxSize.Sum() + row.Length*3 + 1, '-'));
            firstRow = false;
        }
    }
    WriteLine("".PadLeft(maxSize.Sum() + tbl.Columns.Length*3 + 1, '-'));
    WriteLine();
}