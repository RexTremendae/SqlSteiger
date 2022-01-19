namespace SqlDataExtractor.UnitTests.Mocks;

using ForeignKeyMap = Dictionary<(string table, string column), (string table, string column)>;
using TableMetadataMap = Dictionary<string, DatabaseTableMetadata>;

public class DbConnectionMock : IDbConnection
{
    public ForeignKeyMap ForeignKeyMap { get; }
    public TableMetadataMap TableMetadataMap { get; }
    private readonly Dictionary<string, IEnumerable<object?[]>> _tableData;

    public DbConnectionMock()
    {
        ForeignKeyMap = new ForeignKeyMap();
        TableMetadataMap = new TableMetadataMap();
        _tableData = new();
    }

    public void AddTable(DatabaseTableMetadata tableMetadata, IEnumerable<object?[]> dataRows)
    {
        TableMetadataMap.Add(tableMetadata.Name, tableMetadata);
        _tableData.Add(tableMetadata.Name, dataRows);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<IDbDataReader> ExecuteReaderAsync(string commandText)
    {
        foreach (var table in TableMetadataMap.Keys)
        {
            var fromPart = $"FROM dbo.{table}";
            if (commandText.Contains($"{fromPart};") || commandText.Contains(fromPart + Environment.NewLine))
            {
                return Task.FromResult<IDbDataReader>(new DbReaderMock(
                    TableMetadataMap[table].Columns.Select(c => c.Name),
                    _tableData[table]));
            }
        }

        throw new InvalidOperationException();
    }

    public Task OpenAsync()
    {
        return Task.CompletedTask;
    }

    public void AddForeignKey((string table, string column) from, (string table, string column) to)
    {
        ForeignKeyMap.Add(from, to);
    }
}
