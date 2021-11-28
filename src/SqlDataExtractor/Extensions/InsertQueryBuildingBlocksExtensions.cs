using System.Text;

namespace SqlDataExtractor;

public static class InsertQueryBuildingBlocksExtensions
{
    public static string CreateInsertQuery(this InsertQueryBuildingBlocks buildingBlocks)
    {
        var queryBuilder = new StringBuilder();

        var tableMetadata = buildingBlocks.tableMetadata;
        var identityColumns = tableMetadata.Columns
            .Where(c => c.IsIdentity)
            .Select(c => c.Name);

        foreach (var col in identityColumns)
        {
            queryBuilder.AppendLine($"SET IDENTITY_INSERT {tableMetadata.Name} ON;");
        }

        var (insert, values) = tableMetadata.CreateInsertQueryParts(buildingBlocks.dataRows);

        queryBuilder.AppendLine(insert);
        for (int i = 0; i < values.Length; i++)
        {
            var rowEnding = i == values.Length - 1
                ? ';'
                : ',';

            queryBuilder.AppendLine(values[i] + rowEnding);
        }

        foreach (var col in identityColumns)
        {
            queryBuilder.AppendLine($"SET IDENTITY_INSERT {tableMetadata.Name} OFF;");
        }

        return queryBuilder.ToString();
    }
}
