namespace SqlDX;

using System.Data;

public record DatabaseColumnMetadata (string Name, SqlDbType SqlDataType, Type CSharpDataType, bool IsNullable, bool IsIdentity, bool IsPrimaryKeyPart);
public record DatabaseTableMetadata (string Schema, string Name, DatabaseColumnMetadata[] Columns);
public record InsertQueryBuildingBlocks (DatabaseTableMetadata TableMetadata, IEnumerable<Dictionary<string, object?>> DataRows);
public record InsertQueryConfiguration (int MaxRowBatchSize);
