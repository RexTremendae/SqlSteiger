using System.Text;

namespace SqlDataExtractor;

public static class InsertQueryBuildingBlocksExtensions
{
    public static IEnumerable<string> CreateInsertQuery(this InsertQueryBuildingBlocks buildingBlocks)
    {
        var defaultConfiguration = new InsertQueryConfiguration
        (
            MaxRowBatchSize: 1000
        );

        return CreateInsertQuery(buildingBlocks, defaultConfiguration);
    }

    public static IEnumerable<string> CreateInsertQuery(this InsertQueryBuildingBlocks buildingBlocks, InsertQueryConfiguration configuration)
    {
        var tableMetadata = buildingBlocks.TableMetadata;
        var identityColumns = tableMetadata.Columns
            .Where(c => c.IsIdentity)
            .Select(c => c.Name);

        var (insert, values) = tableMetadata.CreateInsertQueryParts(buildingBlocks.DataRows);

        var totalRows = 0;
        while (totalRows < values.Length)
        {
            var queryBuilder = new StringBuilder();
            foreach (var col in identityColumns)
            {
                queryBuilder.AppendLine($"SET IDENTITY_INSERT {tableMetadata.Name} ON;");
            }

            queryBuilder.AppendLine(insert);
            var isLastRow = false;
            for (int i = 0; !isLastRow; i++)
            {
                isLastRow = i >= configuration.MaxRowBatchSize - 1 || totalRows + i >= values.Length - 1;
                var rowEnding = isLastRow
                    ? ';'
                    : ',';

                queryBuilder.AppendLine(values[totalRows + i] + rowEnding);
            }
            totalRows += configuration.MaxRowBatchSize;

            foreach (var col in identityColumns)
            {
                queryBuilder.AppendLine($"SET IDENTITY_INSERT {tableMetadata.Name} OFF;");
            }

            queryBuilder.AppendLine("GO");
            yield return queryBuilder.ToString();
        }
    }
}
