using System.Text;

namespace SqlDataExtractor;

public static class DatabaseTableMetadataExtensions
{
    public static string CreateSelectQuery(this DatabaseTableMetadata tableMetadata, string? keyColumn = null, IEnumerable<object>? keyColumnFilter = null)
    {
        var columns = string.Join(", ", tableMetadata.Columns.Select(c => $"[{c.Name}]"));

        var filterRow = "";
        if (keyColumn != null && (keyColumnFilter?.Any() ?? false))
        {
            var filterList = string.Join(", ", keyColumnFilter.Select(f => f.ToQueryValue(f.GetType())));
            filterRow = $"WHERE {keyColumn} IN ({filterList})";
        }

        var query = $@"
SELECT {columns}
FROM dbo.{tableMetadata.Name}
{filterRow}
";

        return query;
    }

    public static (string insert, string[] values) CreateinsertQuery(this DatabaseTableMetadata tableMetadata, IEnumerable<Dictionary<string, object?>> tableData)
    {
        var columnListing = string.Join(", ", tableMetadata.Columns.Select(c => $"[{c.Name}]"));
        var queryInsert = $"INSERT INTO dbo.{tableMetadata.Name} ({columnListing}) VALUES";

        var queryValues = new List<string>();
        foreach (var rowData in tableData)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append('(');
            bool firstColumn = true;
            foreach (var column in tableMetadata.Columns)
            {
                if (firstColumn) firstColumn = false;
                else queryBuilder.Append(", ");

                var value = rowData[column.Name];
                queryBuilder.Append(value.ToQueryValue(column.CSharpDataType));
            }
            queryBuilder.Append(')');
            queryValues.Add(queryBuilder.ToString());
        }

        return queryValues.Any()
            ? (queryInsert, queryValues.ToArray())
            : (string.Empty, Array.Empty<string>());
    }
}

