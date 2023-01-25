namespace SqlDataExtractor;

using ForeignKeyMap = Dictionary<(string table, string column), (string table, string column)>;
using TableMetadataMap = Dictionary<string, DatabaseTableMetadata>;

public class DatabaseStructureExtractor
{
    private readonly IDbConnection _connection;

    public DatabaseStructureExtractor(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<TableMetadataMap> ExtractTableMapAsync()
    {
        var tables = new Dictionary<string, List<DatabaseColumnMetadata>>();
        await using var reader = await _connection.ExecuteReaderAsync(TableColumnsQuery);

        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(TableName);
            var columnName = reader.GetString(ColumnName);
            var sqlDataTypeString = reader.GetString(SqlDataType);
            var isNullable = reader.GetBoolean(IsNullable);
            var isIdentity = reader.GetBoolean(IsIdentity);
            var isPrimaryKeyPart = reader.GetBoolean(IsPrimaryKeyPart);

            if (tableName == null) throw new InvalidOperationException($"{TableName} is null.");
            if (columnName == null) throw new InvalidOperationException($"{ColumnName} is null.");
            if (sqlDataTypeString == null) throw new InvalidOperationException($"{SqlDataType} is null.");
            if (isNullable == null) throw new InvalidOperationException($"{IsNullable} is null.");
            if (isIdentity == null) throw new InvalidOperationException($"{IsIdentity} is null.");
            if (isPrimaryKeyPart == null) throw new InvalidOperationException($"{IsPrimaryKeyPart} is null.");

            if (!tables.TryGetValue(tableName, out var columnList))
            {
                columnList = new();
                tables.Add(tableName, columnList);
            }

            var sqlDataType = sqlDataTypeString.MapToSqlDbType();
            var csDataType = sqlDataType.MapToCSharpType();
            if (csDataType == typeof(IgnoredDataType))
            {
                continue;
            }

            columnList.Add(new DatabaseColumnMetadata(
                Name: columnName,
                SqlDataType: sqlDataType,
                CSharpDataType: csDataType,
                IsNullable: isNullable.Value,
                IsIdentity: isIdentity.Value,
                IsPrimaryKeyPart: isPrimaryKeyPart.Value
            ));
        }

        return tables.ToDictionary(
            key => key.Key,
            value => new DatabaseTableMetadata(Name: value.Key, Columns: value.Value.ToArray()));
    }

    public async Task<ForeignKeyMap> ExtractForeignKeyMapAsync()
    {
        var foreignKeyMap = new ForeignKeyMap();
        await using var reader = await _connection.ExecuteReaderAsync(ForeignKeyQuery);

        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(TableName);
            var columnName = reader.GetString(ColumnName);
            var referencedTableName = reader.GetString(ReferencedTableName);
            var referencedColumnName = reader.GetString(ReferencedColumnName);

            if (tableName == null) throw new InvalidOperationException("TableName is null.");
            if (columnName == null) throw new InvalidOperationException("ColumnName is null.");
            if (referencedTableName == null) throw new InvalidOperationException("ReferencedTableName is null.");
            if (referencedColumnName == null) throw new InvalidOperationException("ReferencedColumnName is null.");

            foreignKeyMap.Add((tableName, columnName), (referencedTableName, referencedColumnName));
        }

        return foreignKeyMap;
    }

    private const string TableName = nameof(TableName);
    private const string ColumnName = nameof(ColumnName);
    private const string ReferencedTableName = nameof(ReferencedTableName);
    private const string ReferencedColumnName = nameof(ReferencedColumnName);
    private const string SqlDataType = nameof(SqlDataType);
    private const string IsNullable = nameof(IsNullable);
    private const string IsIdentity = nameof(IsIdentity);
    private const string IsPrimaryKeyPart = nameof(IsPrimaryKeyPart);

    private const string TableColumnsQuery = @$"
WITH PrimaryKeys AS (
SELECT
    col.TABLE_NAME AS [{TableName}],
    col.COLUMN_NAME AS [{ColumnName}]
FROM
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS tbl
INNER JOIN
    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE col ON col.CONSTRAINT_NAME = tbl.CONSTRAINT_NAME
WHERE
    col.TABLE_NAME = tbl.TABLE_NAME
    AND tbl.CONSTRAINT_TYPE = 'PRIMARY KEY'
)
SELECT
    tbl.[name] AS [{TableName}],
    col.[name] AS [{ColumnName}],
    sqltype.[name] AS [{SqlDataType}],
    col.[is_nullable] AS [{IsNullable}],
    col.[is_identity] AS [{IsIdentity}],
	CAST (CASE WHEN pk.ColumnName IS NULL THEN 0 ELSE 1 END AS BIT) AS [{IsPrimaryKeyPart}]
FROM sys.columns col
INNER JOIN sys.tables tbl
    ON tbl.object_id = col.object_id
INNER JOIN sys.types sqltype
    ON col.system_type_id = sqltype.system_type_id
    AND col.user_type_id = sqltype.user_type_id
LEFT JOIN PrimaryKeys pk ON pk.TableName = tbl.[name] AND pk.ColumnName = col.[name]
;";

        private const string ForeignKeyQuery = @$"
SELECT
    OBJECT_NAME(f.parent_object_id) AS [{TableName}],
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS [{ColumnName}],
    OBJECT_NAME(f.referenced_object_id) AS [{ReferencedTableName}],
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS [{ReferencedColumnName}]
FROM
    sys.foreign_keys AS f
INNER JOIN
    sys.foreign_key_columns AS fc
    ON f.OBJECT_ID = fc.constraint_object_id
INNER JOIN
    sys.tables t
    ON t.OBJECT_ID = fc.referenced_object_id
;";
}
