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

var tables = (await databaseStructureExtractor.ExtractTableMap()).Select(t => t.Value);
var foreignKeys = await databaseStructureExtractor.ExtractForeignKeyMap();

Informator.PrintTables(tables);
Informator.PrintRelations(foreignKeys);
await Informator.PrintDataAsync(connection, tables);
