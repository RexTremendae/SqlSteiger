using Microsoft.Data.SqlClient;
using SqlSteiger;
using SqlSteiger.SqlDatabase;

#pragma warning disable SA1122  // Use string.Empty for empty string

var connectionBuilder = new SqlConnectionStringBuilder()
{
    DataSource = "localhost",
    UserID = "sa",
    Password = "",
    IntegratedSecurity = false,
    InitialCatalog = "",

    // Seems to be needed when connecting to a local SQL Server instance running in docker
    TrustServerCertificate = true,
};

var schema = "";
var table = "";
var keyColumn = "";
var keyColumnValues = new object[] { };

#pragma warning restore SA1122

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
    foreach (var query in buildingBlocks.CreateInsertQuery())
    {
        Console.WriteLine(query);
    }
}
