namespace SqlDX;

using ForeignKeyMap = Dictionary<(string Schema, string Table, string Column), (string Schema, string Table, string Column)>;
using TableMetadataMap = Dictionary<(string Schema, string Table), DatabaseTableMetadata>;

public class DatabaseStructureExtractor
{
    private readonly IDbConnection _connection;

    public DatabaseStructureExtractor(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<TableMetadataMap> ExtractTableMapAsync()
    {
        var tables = new Dictionary<(string Schema, string Table), List<DatabaseColumnMetadata>>();
        await using var reader = await _connection.ExecuteReaderAsync(SqlQueries.TableColumnsQuery);

        while (await reader.ReadAsync())
        {
            var schemaName = reader.GetString(SqlQueries.SchemaName);
            var tableName = reader.GetString(SqlQueries.TableName);
            var columnName = reader.GetString(SqlQueries.ColumnName);
            var sqlDataTypeString = reader.GetString(SqlQueries.SqlDataType);
            var isNullable = reader.GetBoolean(SqlQueries.IsNullable);
            var isIdentity = reader.GetBoolean(SqlQueries.IsIdentity);
            var isPrimaryKeyPart = reader.GetBoolean(SqlQueries.IsPrimaryKeyPart);
            var isUserDefined = reader.GetBoolean(SqlQueries.IsUserDefined);

            if (schemaName == null) throw new InvalidOperationException($"{SqlQueries.SchemaName} is null.");
            if (tableName == null) throw new InvalidOperationException($"{SqlQueries.TableName} is null.");
            if (columnName == null) throw new InvalidOperationException($"{SqlQueries.ColumnName} is null.");
            if (sqlDataTypeString == null) throw new InvalidOperationException($"{SqlQueries.SqlDataType} is null.");
            if (isNullable == null) throw new InvalidOperationException($"{SqlQueries.IsNullable} is null.");
            if (isIdentity == null) throw new InvalidOperationException($"{SqlQueries.IsIdentity} is null.");
            if (isPrimaryKeyPart == null) throw new InvalidOperationException($"{SqlQueries.IsPrimaryKeyPart} is null.");
            if (isUserDefined == null) throw new InvalidOperationException($"{SqlQueries.IsUserDefined} is null.");

            if (!tables.TryGetValue((schemaName, tableName), out var columnList))
            {
                columnList = new();
                tables.Add((schemaName, tableName), columnList);
            }

            var sqlDataType = isUserDefined.Value ? System.Data.SqlDbType.Udt : sqlDataTypeString.MapToSqlDbType();
            var csDataType = sqlDataType.MapToCSharpType();
            if (csDataType == typeof(IgnoredDataType))
            {
                continue;
            }

            columnList.Add(
                new DatabaseColumnMetadata(
                    Name: columnName,
                    SqlDataType: sqlDataType,
                    CSharpDataType: csDataType,
                    IsNullable: isNullable.Value,
                    IsIdentity: isIdentity.Value,
                    IsPrimaryKeyPart: isPrimaryKeyPart.Value));
        }

        return tables.ToDictionary(
            key => (key.Key.Schema, key.Key.Table),
            value => new DatabaseTableMetadata(Schema: $"{value.Key.Schema}", Name: $"{value.Key.Table}", Columns: value.Value.ToArray()));
    }

    public async Task<ForeignKeyMap> ExtractForeignKeyMapAsync()
    {
        var foreignKeyMap = new ForeignKeyMap();
        await using var reader = await _connection.ExecuteReaderAsync(SqlQueries.ForeignKeyQuery);

        while (await reader.ReadAsync())
        {
            var schemaName = reader.GetString(SqlQueries.SchemaName);
            var tableName = reader.GetString(SqlQueries.TableName);
            var columnName = reader.GetString(SqlQueries.ColumnName);
            var referencedSchemaName = reader.GetString(SqlQueries.ReferencedSchemaName);
            var referencedTableName = reader.GetString(SqlQueries.ReferencedTableName);
            var referencedColumnName = reader.GetString(SqlQueries.ReferencedColumnName);

            if (schemaName == null) throw new InvalidOperationException("SchemaName is null.");
            if (tableName == null) throw new InvalidOperationException("TableName is null.");
            if (columnName == null) throw new InvalidOperationException("ColumnName is null.");
            if (referencedSchemaName == null) throw new InvalidOperationException("ReferencedSchemaName is null.");
            if (referencedTableName == null) throw new InvalidOperationException("ReferencedTableName is null.");
            if (referencedColumnName == null) throw new InvalidOperationException("ReferencedColumnName is null.");

            foreignKeyMap.Add((schemaName, tableName, columnName), (referencedSchemaName, referencedTableName, referencedColumnName));
        }

        return foreignKeyMap;
    }
}
