namespace SqlDataExtractor;

public record DatabaseColumnMetadata (string Name, string SqlDataType, Type CSharpDataType, bool IsNullable, bool IsIdentity);
public record DatabaseTableMetadata (string Name, DatabaseColumnMetadata[] Columns);
public record InsertQueryBuildingBlocks (DatabaseTableMetadata TableMetadata, IEnumerable<Dictionary<string, object?>> DataRows);
public record InsertQueryConfiguration (int MaxRowBatchSize);
