namespace SqlDataExtractor;

public interface IDbDataReader : IAsyncDisposable
{
    Task<bool> ReadAsync();
    object? GetValue(DatabaseColumnMetadata columnMetadata);
    object? GetValue(string columnName, Type csharpDataType);
    string? GetString(string columnName);
    bool? GetBoolean(string columnName);
    bool IsDBNull(string columnName);
}
