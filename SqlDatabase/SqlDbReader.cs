using Microsoft.Data.SqlClient;

namespace SqlDataExtractor.SqlDatabase
{
    public class SqlDbReader : IDbDataReader
    {
        private SqlDataReader _dbDataReader;

        public SqlDbReader(SqlDataReader dbDataReader)
        {
            _dbDataReader = dbDataReader;
        }

        public bool IsDBNull(int ordinal)
        {
            return _dbDataReader.IsDBNull(ordinal);
        }

        public async Task<bool> ReadAsync()
        {
            return await _dbDataReader.ReadAsync();
        }

        public short GetInt16(int ordinal)
        {
            return _dbDataReader.GetInt16(ordinal);
        }

        public int GetInt32(int ordinal)
        {
            return _dbDataReader.GetInt32(ordinal);
        }

        public long GetInt64(int ordinal)
        {
            return _dbDataReader.GetInt64(ordinal);
        }

        public float GetFloat(int ordinal)
        {
            return _dbDataReader.GetFloat(ordinal);
        }

        public double GetDouble(int ordinal)
        {
            return _dbDataReader.GetDouble(ordinal);
        }

        public decimal GetDecimal(int ordinal)
        {
            return _dbDataReader.GetDecimal(ordinal);
        }

        public bool GetBoolean(int ordinal)
        {
            return _dbDataReader.GetBoolean(ordinal);
        }

        public string GetString(int ordinal)
        {
            return _dbDataReader.GetString(ordinal);
        }

        public DateTime GetDateTime(int ordinal)
        {
            return _dbDataReader.GetDateTime(ordinal);
        }

        public DateTimeOffset GetDateTimeOffset(int ordinal)
        {
            return _dbDataReader.GetDateTimeOffset(ordinal);
        }

        public TimeSpan GetTimeSpan(int ordinal)
        {
            return _dbDataReader.GetTimeSpan(ordinal);
        }

        public Guid GetGuid(int ordinal)
        {
            return _dbDataReader.GetGuid(ordinal);
        }

        public int GetOrdinal(string columnName)
        {
            return _dbDataReader.GetOrdinal(columnName);
        }

        public async ValueTask DisposeAsync()
        {
            await _dbDataReader.DisposeAsync();
        }
    }
}