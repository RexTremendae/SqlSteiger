namespace SqlDX;

public static class SqlQueries
{
    public const string SchemaName           = nameof(SchemaName);
    public const string TableName            = nameof(TableName);
    public const string ColumnName           = nameof(ColumnName);
    public const string ReferencedSchemaName = nameof(ReferencedSchemaName);
    public const string ReferencedTableName  = nameof(ReferencedTableName);
    public const string ReferencedColumnName = nameof(ReferencedColumnName);
    public const string SqlDataType          = nameof(SqlDataType);
    public const string IsNullable           = nameof(IsNullable);
    public const string IsIdentity           = nameof(IsIdentity);
    public const string IsPrimaryKeyPart     = nameof(IsPrimaryKeyPart);
    public const string IsUserDefined        = nameof(IsUserDefined);

    public const string TableColumnsQuery = $"""
        WITH PrimaryKeys AS (
        SELECT
            tbl.TABLE_SCHEMA AS [SchemaName],
            col.TABLE_NAME AS [TableName],
            col.COLUMN_NAME AS [ColumnName]
        FROM
            INFORMATION_SCHEMA.TABLE_CONSTRAINTS tbl
        INNER JOIN
            INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE col ON col.CONSTRAINT_NAME = tbl.CONSTRAINT_NAME
        WHERE
            col.TABLE_NAME = tbl.TABLE_NAME
            AND tbl.CONSTRAINT_TYPE = 'PRIMARY KEY'
        )
        SELECT
            schm.[name] AS [{SchemaName}],
            tbl.[name] AS [{TableName}],
            col.[name] AS [{ColumnName}],
            sqltype.[name] AS [{SqlDataType}],
            sqltype.[is_user_defined] AS [{IsUserDefined}],
            col.[is_nullable] AS [{IsNullable}],
            col.[is_identity] AS [{IsIdentity}],
            CAST (CASE WHEN pk.ColumnName IS NULL THEN 0 ELSE 1 END AS BIT) AS [{IsPrimaryKeyPart}]
        FROM sys.columns col
        INNER JOIN sys.tables tbl
            ON tbl.object_id = col.object_id
        INNER JOIN sys.schemas schm
            ON schm.schema_id = tbl.schema_id
        INNER JOIN sys.types sqltype
            ON col.system_type_id = sqltype.system_type_id
            AND col.user_type_id = sqltype.user_type_id
        LEFT JOIN PrimaryKeys pk ON pk.TableName = tbl.[name] AND pk.ColumnName = col.[name]
        ;
        """;

    public const string ForeignKeyQuery = $"""
        SELECT
            SCHEMA_NAME(f.schema_id) [{SchemaName}],
            OBJECT_NAME(f.parent_object_id) AS [{TableName}],
            COL_NAME(fc.parent_object_id, fc.parent_column_id) AS [{ColumnName}],
            s.[name] [{ReferencedSchemaName}],
            t.[name] [{ReferencedTableName}],
            COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS [{ReferencedColumnName}]
        FROM
            sys.foreign_keys AS f
        INNER JOIN
            sys.foreign_key_columns AS fc
            ON f.object_id = fc.constraint_object_id
        INNER JOIN
            sys.tables t
            ON t.object_id = f.referenced_object_id
        INNER JOIN
            sys.schemas s
            ON t.schema_id = s.schema_id
        ;
        """;
}
