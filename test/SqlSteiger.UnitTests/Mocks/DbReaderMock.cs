namespace SqlSteiger.UnitTests.Mocks;

public class DbReaderMock : IDbDataReader
{
    private readonly List<object?[]> _columnData;
    private readonly List<string> _columnNames;
    private int _rowIndex;

    public DbReaderMock(IEnumerable<string> columnNames, IEnumerable<object?[]> columnData)
    {
        _columnData = columnData.ToList();
        _columnNames = columnNames.ToList();
        _rowIndex = -1;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public int GetOrdinal(string columnName)
    {
        int ordinal = 0;
        foreach (var col in _columnNames)
        {
            if (col == columnName)
            {
                return ordinal;
            }
            ordinal++;
        }

        throw new InvalidOperationException();
    }

    public object? GetValue(DatabaseColumnMetadata columnMetadata)
    {
        return _columnData[_rowIndex][GetOrdinal(columnMetadata.Name)];
    }

    public object? GetValue(string columnName, Type csharpDataType)
    {
        return _columnData[_rowIndex][GetOrdinal(columnName)];
    }

    public bool? GetBoolean(string columnName) =>
        _columnData[_rowIndex][GetOrdinal(columnName)] is bool value ? value
        : throw new InvalidDataException();

    public string? GetString(string columnName) =>
        _columnData[_rowIndex][GetOrdinal(columnName)] is string value ? value
        : throw new InvalidDataException();

    public bool IsDBNull(string columnName)
    {
        return _columnData[GetOrdinal(columnName)] == null;
    }

    public Task<bool> ReadAsync()
    {
        _rowIndex++;
        return Task.FromResult(_rowIndex < _columnData.Count);
    }
}
