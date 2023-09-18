namespace SqlDX.UnitTests.Mocks;

using ForeignKeyMap = Dictionary<(string schema, string table, string column), (string schema, string table, string column)>;
using TableMetadataMap = Dictionary<(string schema, string table), DatabaseTableMetadata>;

public class DbConnectionMock : IDbConnection
{
    public ForeignKeyMap ForeignKeyMap { get; }
    public TableMetadataMap TableMetadataMap { get; }
    private readonly Dictionary<(string, string), IEnumerable<object?[]>> _tableData;

    public DbConnectionMock()
    {
        ForeignKeyMap = new ForeignKeyMap();
        TableMetadataMap = new TableMetadataMap();
        _tableData = new();
    }

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

    public void AddForeignKey((string schema, string table, string column) from, (string schema, string table, string column) to)
    {
        ForeignKeyMap.Add(from, to);
    }
}
