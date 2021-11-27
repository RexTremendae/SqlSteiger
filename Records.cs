public record DatabaseColumnMetadata (string Name, string SqlDataType, Type CSharpDataType, bool IsNullable);
public record DatabaseTableMetadata (string Name, DatabaseColumnMetadata[] Columns);
