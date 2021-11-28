namespace SqlDataExtractor;

public interface IDbDataReader : IAsyncDisposable
{
    Task<bool> ReadAsync();
    int GetOrdinal(string columnName);
    bool IsDBNull(int ordinal);
    short GetInt16(int ordinal);
    int GetInt32(int ordinal);
    long GetInt64(int ordinal);
    float GetFloat(int ordinal);
    double GetDouble(int ordinal);
    decimal GetDecimal(int ordinal);
    string GetString(int ordinal);
    bool GetBoolean(int ordinal);
    DateTime GetDateTime(int ordinal);
    DateTimeOffset GetDateTimeOffset(int ordinal);
    TimeSpan GetTimeSpan(int ordinal);
    Guid GetGuid(int ordinal);
}
