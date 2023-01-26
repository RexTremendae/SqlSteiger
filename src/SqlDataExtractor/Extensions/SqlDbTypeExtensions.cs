namespace SqlDX;

using System.Data;

public static class SqlDbTypeExtensions
{
    public static Type MapToCSharpType(this SqlDbType sqlDataType)
    {
        return sqlDataType switch
        {
            SqlDbType.BigInt           => typeof(long),
            SqlDbType.Bit              => typeof(bool),
            SqlDbType.Char             => typeof(string),
            SqlDbType.Date             => typeof(DateTime),
            SqlDbType.DateTime         => typeof(DateTime),
            SqlDbType.DateTime2        => typeof(DateTime),
            SqlDbType.DateTimeOffset   => typeof(DateTimeOffset),
            SqlDbType.Decimal          => typeof(decimal),
            SqlDbType.Float            => typeof(double),
            SqlDbType.Int              => typeof(int),
            SqlDbType.Money            => typeof(decimal),
            SqlDbType.NChar            => typeof(string),
            SqlDbType.NText            => typeof(string),
            SqlDbType.NVarChar         => typeof(string),
            SqlDbType.Real             => typeof(float),
            SqlDbType.SmallDateTime    => typeof(DateTime),
            SqlDbType.SmallInt         => typeof(short),
            SqlDbType.SmallMoney       => typeof(decimal),
            SqlDbType.Text             => typeof(string),
            SqlDbType.Time             => typeof(TimeSpan),
            SqlDbType.Timestamp        => typeof(IgnoredDataType),
            SqlDbType.UniqueIdentifier => typeof(Guid),
            SqlDbType.VarBinary        => typeof(byte[]),
            SqlDbType.VarChar          => typeof(string),

            _ => throw new InvalidOperationException($"No mapping defined for SQL type {sqlDataType}.")
        };
    }
}

