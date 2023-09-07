using Microsoft.Data.SqlClient;
using SqlDX;
using SqlDX.SqlDatabase;

var connectionBuilder = new SqlConnectionStringBuilder();

connectionBuilder.DataSource = "localhost";
connectionBuilder.UserID = "sa";
connectionBuilder.Password = "";
connectionBuilder.IntegratedSecurity = false;
connectionBuilder.InitialCatalog = "";

// Seems to be needed when connecting to a local SQL Server instance running in docker
connectionBuilder.TrustServerCertificate = true;

var schema = "";
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

Informator.PrintTitle("Insert queries");
foreach (var buildingBlocks in await crawler.GetInsertQueriesBuildingBlocksAsync(connection, schema, table, keyColumn, keyColumnValues))
{
    foreach(var query in buildingBlocks.CreateInsertQuery())
    {
        Console.WriteLine(query);
    }
}
