using Microsoft.Data.SqlClient;
using SqlDataExtractor;
using SqlDataExtractor.SqlDatabase;

var connectionBuilder = new SqlConnectionStringBuilder();

connectionBuilder.DataSource = "localhost";
connectionBuilder.UserID = "sa";
connectionBuilder.Password = "";
connectionBuilder.IntegratedSecurity = false;
connectionBuilder.InitialCatalog = "";

// Seems to be needed when connecting to a local SQL Server instance running in docker
connectionBuilder.TrustServerCertificate = true;

var table = "";
var keyColumn = "";
var keyColumnValues = new object[] {};

await using IDbConnection connection = new SqlDbConnection(connectionBuilder.ToString());

await connection.OpenAsync();
var databaseStructureExtractor = new DatabaseStructureExtractor(connection);

var tableMap = await databaseStructureExtractor.ExtractTableMapAsync();
var tables = tableMap.Select(t => t.Value);
var foreignKeyMap = await databaseStructureExtractor.ExtractForeignKeyMapAsync();

Informator.PrintTables(tables);
Informator.PrintRelations(foreignKeyMap);

await Informator.PrintDataAsync(connection, tables);

var crawler = new DependencyCrawler(foreignKeyMap, tableMap);

foreach (var t in await crawler.GetInsertQueriesBuildingBlocksAsync(connection, table, keyColumn, keyColumnValues))
{
    var identityColumns = t.tableMetadata.Columns
        .Where(c => c.IsIdentity)
        .Select(c => c.Name);

    foreach (var col in identityColumns)
    {
        Console.WriteLine($"SET IDENTITY_INSERT {t.tableMetadata.Name} ON;");
    }

    var insertQuery = t.tableMetadata.CreateinsertQuery(t.dataRows);
    Console.WriteLine(insertQuery.insert);
    for (int i = 0; i < insertQuery.values.Length; i++)
    {
        var rowEnding = i == insertQuery.values.Length - 1
            ? ';'
            : ',';

        Console.WriteLine(insertQuery.values[i] + rowEnding);
    }

    foreach (var col in identityColumns)
    {
        Console.WriteLine($"SET IDENTITY_INSERT {t.tableMetadata.Name} OFF;");
    }

    Console.WriteLine();
}
