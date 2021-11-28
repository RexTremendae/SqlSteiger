namespace SqlDataExtractor;

public record DatabaseColumnMetadata (string Name, string SqlDataType, Type CSharpDataType, bool IsNullable, bool IsIdentity);
public record DatabaseTableMetadata (string Name, DatabaseColumnMetadata[] Columns);
public record InsertQueryBuildingBlocks (DatabaseTableMetadata tableMetadata, IEnumerable<Dictionary<string, object?>> dataRows);
