namespace SqlDX.SqlDatabase;

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
            _ => throw new InvalidOperationException($"No definition for how to read type {csharpDataType.Name}")
        };
    }

    public short? GetInt16(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetInt16(ordinal);
    }

    public int? GetInt32(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetInt32(ordinal);
    }

    public long? GetInt64(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetInt64(ordinal);
    }

    public float? GetFloat(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetFloat(ordinal);
    }

    public double? GetDouble(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetDouble(ordinal);
    }

    public decimal? GetDecimal(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetDecimal(ordinal);
    }

    public string? GetString(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetString(ordinal);
    }

    public bool? GetBoolean(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetBoolean(ordinal);
    }

    public DateTime? GetDateTime(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetDateTime(ordinal);
    }

    public DateTimeOffset? GetDateTimeOffset(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetDateTimeOffset(ordinal);
    }

    public TimeSpan? GetTimeSpan(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetTimeSpan(ordinal);
    }

    public Guid? GetGuid(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return _dbDataReader.GetGuid(ordinal);
    }

    public byte? GetByte(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return (byte)_dbDataReader.GetValue(ordinal);
    }

    public byte[]? GetByteArray(string columnName)
    {
        if (!HasValue(columnName, out var ordinal))
        {
            return null;
        }

        return (byte[])_dbDataReader.GetValue(ordinal);
    }

    private bool HasValue(string columnName, out int ordinal)
    {
        ordinal = GetOrdinal(columnName);
        if (_dbDataReader.IsDBNull(ordinal))
        {
            return false;
        }

        return true;
    }

    private int GetOrdinal(string columnName)
    {
        return _dbDataReader.GetOrdinal(columnName);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbDataReader.DisposeAsync();
    }
}
