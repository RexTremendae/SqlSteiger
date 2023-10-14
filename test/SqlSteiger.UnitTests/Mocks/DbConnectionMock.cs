namespace SqlSteiger.UnitTests.Mocks;

using ForeignKeyMap = Dictionary<(string Schema, string Table, string Column), (string Schema, string Table, string Column)>;
using TableMetadataMap = Dictionary<(string Schema, string Table), DatabaseTableMetadata>;

public class DbConnectionMock : IDbConnection
{
    private readonly Dictionary<(string, string), IEnumerable<object?[]>> _tableData;

    public DbConnectionMock()
    {
        ForeignKeyMap = new ForeignKeyMap();
        TableMetadataMap = new TableMetadataMap();
        _tableData = new();
    }

    public ForeignKeyMap ForeignKeyMap { get; }

    public TableMetadataMap TableMetadataMap { get; }

    public void AddTable(DatabaseTableMetadata tableMetadata, IEnumerable<object?[]> dataRows)
    {
        TableMetadataMap.Add((tableMetadata.Schema, tableMetadata.Name), tableMetadata);
        _tableData.Add((tableMetadata.Schema, tableMetadata.Name), dataRows);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<IDbDataReader> ExecuteReaderAsync(string commandText)
    {
        foreach (var (schema, table) in TableMetadataMap.Keys)
        {
            var fromPart = $"FROM [{schema}].[{table}]";
            if (commandText.Contains($"{fromPart};") || commandText.Contains(fromPart + Environment.NewLine))
            {
                return Task.FromResult<IDbDataReader>(new DbReaderMock(
                    TableMetadataMap[(schema, table)].Columns.Select(c => c.Name),
                    _tableData[(schema, table)]));
            }
        }

        throw new InvalidOperationException();
    }

    public Task OpenAsync()
    {
        return Task.CompletedTask;
    }

    public void AddForeignKey((string Schema, string Table, string Column) from, (string Schema, string Table, string Column) to)
    {
        ForeignKeyMap.Add(from, to);
    }
}
