namespace SqlDataExtractor
{
    public static class DbDataReaderExtensions
    {
        public static object? GetValue(this IDbDataReader reader, DatabaseColumnMetadata columnMetadata)
        {
            return GetValue(reader, columnMetadata.Name, columnMetadata.CSharpDataType);
        }

        public static object? GetValue(this IDbDataReader reader, string columnName, Type csharpDataType)
        {
            return csharpDataType switch
            {
                var t when t == typeof(short) => GetInt16(reader, columnName),
                var t when t == typeof(int) => GetInt32(reader, columnName),
                var t when t == typeof(long) => GetInt64(reader, columnName),
                var t when t == typeof(float) => GetFloat(reader, columnName),
                var t when t == typeof(double) => GetDouble(reader, columnName),
                var t when t == typeof(decimal) => GetDecimal(reader, columnName),
                var t when t == typeof(string) => GetString(reader, columnName),
                var t when t == typeof(bool) => GetBoolean(reader, columnName),
                var t when t == typeof(DateTime) => GetDateTime(reader, columnName),
                var t when t == typeof(DateTimeOffset) => GetDateTimeOffset(reader, columnName),
                var t when t == typeof(TimeSpan) => GetTimeSpan(reader, columnName),
                var t when t == typeof(Guid) => GetGuid(reader, columnName),
                _ => throw new InvalidOperationException($"No definition for how to read type {csharpDataType.Name}")
            };
        }

        public static short? GetInt16(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetInt16(ordinal);
        }

        public static int? GetInt32(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetInt32(ordinal);
        }

        public static long? GetInt64(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetInt64(ordinal);
        }

        public static float? GetFloat(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetFloat(ordinal);
        }

        public static double? GetDouble(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetDouble(ordinal);
        }

        public static decimal? GetDecimal(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetDecimal(ordinal);
        }

        public static string? GetString(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetString(ordinal);
        }

        public static bool? GetBoolean(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetBoolean(ordinal);
        }

        public static DateTime? GetDateTime(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetDateTime(ordinal);
        }

        public static DateTimeOffset? GetDateTimeOffset(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetDateTimeOffset(ordinal);
        }

        public static TimeSpan? GetTimeSpan(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetTimeSpan(ordinal);
        }

        public static Guid? GetGuid(this IDbDataReader reader, string columnName)
        {
            if (!HasValue(reader, columnName, out var ordinal))
            {
                return null;
            }

            return reader.GetGuid(ordinal);
        }

        private static bool HasValue(IDbDataReader reader, string columnName, out int ordinal)
        {
            ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return false;
            }

            return true;
        }
    }
}
