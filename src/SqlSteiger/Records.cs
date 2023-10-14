namespace SqlSteiger;

using System.Data;

public readonly record struct DatabaseColumnMetadata
(
    string Name,
    SqlDbType SqlDataType,
    Type CSharpDataType,
    bool IsNullable,
    bool IsIdentity,
    bool IsPrimaryKeyPart);

public readonly record struct DatabaseTableMetadata
(
    string Schema,
    string Name,
    DatabaseColumnMetadata[] Columns);

public readonly record struct InsertQueryBuildingBlocks
(
    DatabaseTableMetadata TableMetadata,
    IEnumerable<Dictionary<string, object?>> DataRows);

public readonly record struct InsertQueryConfiguration
(
    int MaxRowBatchSize);
