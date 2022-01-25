namespace SqlDataExtractor;

public static class StringExtensions
{
    public static Type MapToCSharpType(this string sqlDataType)
    {
        return sqlDataType.ToLower() switch
        {
            "bigint"           => typeof(long),
            "bit"              => typeof(bool),
            "char"             => typeof(string),
            "date"             => typeof(DateTime),
            "datetime"         => typeof(DateTime),
            "datetime2"        => typeof(DateTime),
            "datetimeoffset"   => typeof(DateTimeOffset),
            "decimal"          => typeof(decimal),
            "float"            => typeof(double),
            "int"              => typeof(int),
            "money"            => typeof(decimal),
            "nchar"            => typeof(string),
            "ntext"            => typeof(string),
            "numeric"          => typeof(decimal),
            "nvarchar"         => typeof(string),
            "real"             => typeof(float),
            "rowversion"       => typeof(IgnoredDataType),
            "smalldatetime"    => typeof(DateTime),
            "smallint"         => typeof(short),
            "smallmoney"       => typeof(decimal),
            "sysname"          => typeof(string),
            "text"             => typeof(string),
            "time"             => typeof(TimeSpan),
            "timestamp"        => typeof(IgnoredDataType),
            "uniqueidentifier" => typeof(Guid),
            "varbinary"        => typeof(byte[]),
            "varchar"          => typeof(string),

            _ => throw new InvalidOperationException($"No mapping defined for SQL type {sqlDataType}.")
        };
    }
}

