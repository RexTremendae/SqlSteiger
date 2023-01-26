namespace SqlDX;

using System.Data;

public static class StringExtensions
{
    public static SqlDbType MapToSqlDbType(this string sqlDbType)
    {
        if (sqlDbType.Equals("numeric", StringComparison.OrdinalIgnoreCase))
        {
            return SqlDbType.Decimal;
        }

        if (sqlDbType.Equals("rowversion", StringComparison.OrdinalIgnoreCase))
        {
            return SqlDbType.Timestamp;
        }

        if (sqlDbType.Equals("Sysname", StringComparison.OrdinalIgnoreCase))
        {
            return SqlDbType.NVarChar;
        }

        if (!Enum.TryParse<SqlDbType>(sqlDbType, ignoreCase: true, out var parsedSqlDbType))
        {
            throw new InvalidOperationException($"No mapping found for type '{sqlDbType}'.");
        }

        return parsedSqlDbType;
    }
}

