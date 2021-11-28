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
var table = "";
var keyColumn = "";
var keyColumnValues = new object[] {};

foreach (var t in await crawler.GetInsertQueriesBuildingBlocksAsync(connection, table, keyColumn, keyColumnValues))
{
    var insertQuery = t.tableMetadata.CreateinsertQuery(t.dataRows);
    Console.WriteLine(insertQuery.insert);
    for (int i = 0; i < insertQuery.values.Length; i++)
    {
        var rowEnding = i == insertQuery.values.Length - 1
            ? ';'
            : ',';

        Console.WriteLine(insertQuery.values[i] + rowEnding);
    }
    Console.WriteLine();
}
