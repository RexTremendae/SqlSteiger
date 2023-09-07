namespace SqlDX;

using System.Data;

public static class StringExtensions
{
    public static SqlDbType MapToSqlDbType(this string sqlDbType)
    {
        return sqlDbType switch
        {
            var x when x.Equals("numeric",     StringComparison.OrdinalIgnoreCase) => SqlDbType.Decimal,
            var x when x.Equals("rowversion",  StringComparison.OrdinalIgnoreCase) => SqlDbType.Timestamp,
            var x when x.Equals("sysname",     StringComparison.OrdinalIgnoreCase) => SqlDbType.NVarChar,

            // These special types are not really user defined, but it makes it easier to handle for now (they will be ignored)
            var x when x.Equals("geography",   StringComparison.OrdinalIgnoreCase) => SqlDbType.Udt,
            var x when x.Equals("geometry",    StringComparison.OrdinalIgnoreCase) => SqlDbType.Udt,
            var x when x.Equals("hierarchyid", StringComparison.OrdinalIgnoreCase) => SqlDbType.Udt,

            _ => Enum.TryParse<SqlDbType>(sqlDbType, ignoreCase: true, out var parsedSqlDbType)
                ? parsedSqlDbType
                : throw new InvalidOperationException($"No mapping found for type '{sqlDbType}'.")
        };
    }
}
