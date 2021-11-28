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

        var insertQuery = tableMetadata.CreateinsertQuery(buildingBlocks.dataRows);

        queryBuilder.AppendLine(insertQuery.insert);
        for (int i = 0; i < insertQuery.values.Length; i++)
        {
            var rowEnding = i == insertQuery.values.Length - 1
                ? ';'
                : ',';

            queryBuilder.AppendLine(insertQuery.values[i] + rowEnding);
        }

        foreach (var col in identityColumns)
        {
            queryBuilder.AppendLine($"SET IDENTITY_INSERT {tableMetadata.Name} OFF;");
        }

        return queryBuilder.ToString();
    }
}
