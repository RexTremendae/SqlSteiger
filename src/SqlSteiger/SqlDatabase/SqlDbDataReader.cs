namespace SqlSteiger.SqlDatabase;

using Microsoft.Data.SqlClient;

public class SqlDbDataReader : IDbDataReader
{
    private readonly SqlDataReader _dbDataReader;

    public SqlDbDataReader(SqlDataReader dbDataReader)
    {
        _dbDataReader = dbDataReader;
    }

    public bool IsDBNull(string columnName)
    {
        return _dbDataReader.IsDBNull(GetOrdinal(columnName));
    }

    public async Task<bool> ReadAsync()
    {
        return await _dbDataReader.ReadAsync();
    }

    public object? GetValue(DatabaseColumnMetadata columnMetadata)
    {
        return GetValue(columnMetadata.Name, columnMetadata.CSharpDataType);
    }

    public object? GetValue(string columnName, Type csharpDataType)
    {
        return csharpDataType switch
        {
            var t when t == typeof(short)           => GetInt16(columnName),
            var t when t == typeof(int)             => GetInt32(columnName),
            var t when t == typeof(long)            => GetInt64(columnName),
            var t when t == typeof(float)           => GetFloat(columnName),
            var t when t == typeof(double)          => GetDouble(columnName),
            var t when t == typeof(decimal)         => GetDecimal(columnName),
            var t when t == typeof(string)          => GetString(columnName),
            var t when t == typeof(bool)            => GetBoolean(columnName),
            var t when t == typeof(DateTime)        => GetDateTime(columnName),
            var t when t == typeof(DateTimeOffset)  => GetDateTimeOffset(columnName),
            var t when t == typeof(TimeSpan)        => GetTimeSpan(columnName),
            var t when t == typeof(Guid)            => GetGuid(columnName),
            var t when t == typeof(byte[])          => GetByteArray(columnName),
            var t when t == typeof(byte)            => GetByte(columnName),
            _ => throw new InvalidOperationException($"No definition for how to read type {csharpDataType.Name}"),
        };
    }

    public short? GetInt16(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetInt16(ordinal)
            : null;
    }

    public int? GetInt32(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetInt32(ordinal)
            : null;
    }

    public long? GetInt64(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetInt64(ordinal)
            : null;
    }

    public float? GetFloat(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetFloat(ordinal)
            : null;
    }

    public double? GetDouble(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetDouble(ordinal)
            : null;
    }

    public decimal? GetDecimal(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetDecimal(ordinal)
            : null;
    }

    public string? GetString(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetString(ordinal)
            : null;
    }

    public bool? GetBoolean(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetBoolean(ordinal)
            : null;
    }

    public DateTime? GetDateTime(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetDateTime(ordinal)
            : null;
    }

    public DateTimeOffset? GetDateTimeOffset(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetDateTimeOffset(ordinal)
            : null;
    }

    public TimeSpan? GetTimeSpan(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetTimeSpan(ordinal)
            : null;
    }

    public Guid? GetGuid(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? _dbDataReader.GetGuid(ordinal)
            : null;
    }

    public byte? GetByte(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? (byte)_dbDataReader.GetValue(ordinal)
            : null;
    }

    public byte[]? GetByteArray(string columnName)
    {
        return HasValue(columnName, out var ordinal)
            ? (byte[])_dbDataReader.GetValue(ordinal)
            : null;
    }

    public async ValueTask DisposeAsync()
    {
        await _dbDataReader.DisposeAsync();
    }

    private bool HasValue(string columnName, out int ordinal)
    {
        ordinal = GetOrdinal(columnName);
        return !_dbDataReader.IsDBNull(ordinal);
    }

    private int GetOrdinal(string columnName)
    {
        return _dbDataReader.GetOrdinal(columnName);
    }
}
