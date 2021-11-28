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
            var filterList = string.Join(", ", keyColumnFilter.Select(f => f.ToQueryValue()));
            filterRow = $"WHERE {keyColumn} IN ({filterList})";
        }

        var queryLines = new List<string>(new[]
        {
            $"SELECT {columns}",
            $"FROM dbo.{tableMetadata.Name}"
        });

        if (!string.IsNullOrEmpty(filterRow))
        {
            queryLines.Add(filterRow);
        }

        var queryBuilder = new StringBuilder();
        for (int i = 0; i < queryLines.Count; i++)
        {
            var line = queryLines[i];
            if (i == queryLines.Count - 1)
            {
                line += ";";
            }
            queryBuilder.AppendLine(line);
        }

        return queryBuilder.ToString();
    }

    public static (string insert, string[] values) CreateInsertQueryParts(this DatabaseTableMetadata tableMetadata, IEnumerable<Dictionary<string, object?>> tableData)
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
                queryBuilder.Append(value.ToQueryValue());
            }
            queryBuilder.Append(')');
            queryValues.Add(queryBuilder.ToString());
        }

        return queryValues.Any()
            ? (queryInsert, queryValues.ToArray())
            : (string.Empty, Array.Empty<string>());
    }
}

