namespace SqlDX;

using System.Data;
using ForeignKeyMap = Dictionary<(string Schema, string Table, string Column), (string Schema, string Table, string Column)>;
using TableMetadataMap = Dictionary<(string Schema, string Name), DatabaseTableMetadata>;

public class DependencyCrawler
{
    private readonly TableMetadataMap _tables;
    private readonly ForeignKeyMap _foreignKeys;
    private readonly List<(string Schema, string Table, string KeyColumn, object KeyColumnValue)> _queue;
    private readonly HashSet<(string Schema, string Table, string KeyColumn, object KeyColumnValue)> _visited;

    public DependencyCrawler(ForeignKeyMap foreignKeys, TableMetadataMap tables)
    {
        _tables = tables;
        _foreignKeys = foreignKeys;
        _queue = new();
        _visited = new();
    }

    public async Task<IEnumerable<InsertQueryBuildingBlocks>> GetInsertQueriesBuildingBlocksAsync(
        IDbConnection connection,
        string startingSchema,
        string startingTable,
        string keyColumn,
        params object[] keyColumnValues)
    {
        _queue.Clear();
        _visited.Clear();

        Enqueue(startingSchema, startingTable, keyColumn, keyColumnValues);

        var insertQueryTableData = new List<InsertQueryBuildingBlocks>();

        while (_queue.Any())
        {
            if (_queue.Count > 120)
            {
                break;
            }

            var (dequeuedSchema, dequeuedTable, dequeuedKeyColumn, dequeuedKeyColumnValue) = _queue.First();
            _queue.RemoveAt(0);

            _visited.Add((dequeuedSchema, dequeuedTable, dequeuedKeyColumn, dequeuedKeyColumnValue));

            var selectQuery = _tables[(dequeuedSchema, dequeuedTable)].CreateSelectQuery(
                keyColumn: dequeuedKeyColumn,
                keyColumnFilter: new[] { dequeuedKeyColumnValue });

            var data = new List<Dictionary<string, object?>>();

            await using var reader = await connection.ExecuteReaderAsync(selectQuery);

            while (await reader.ReadAsync())
            {
                var dataRow = new Dictionary<string, object?>();
                data.Add(dataRow);

                foreach (var col in _tables[(dequeuedSchema, dequeuedTable)].Columns)
                {
                    dataRow.Add(col.Name, reader.GetValue(col));
                }
            }

            insertQueryTableData.Add(new InsertQueryBuildingBlocks(_tables[(dequeuedSchema, dequeuedTable)], data));

            // Add child dependencies
            foreach (var (from, to) in _foreignKeys.Where(fk =>
                fk.Key.Table == dequeuedTable &&
                fk.Key.Schema == dequeuedSchema))
            {
                keyColumnValues = data.Select(d => d[from.Column]).Cast<object>().ToArray();
                Enqueue(to.Schema, to.Table, to.Column, keyColumnValues);
            }

            // Add parent dependencies
            foreach (var (from, to) in _foreignKeys.Where(fk =>
                fk.Value.Table == dequeuedTable &&
                fk.Value.Schema == dequeuedSchema))
            {
                keyColumnValues = data.Select(d => d[to.Column]).Cast<object>().ToArray();
                Enqueue(from.Schema, from.Table, from.Column, keyColumnValues);
            }
        }

        return OrderByDependencies(insertQueryTableData);
    }

    private void Enqueue(string schema, string table, string keyColumn, object[] keyColumnValues)
    {
        foreach (var value in keyColumnValues)
        {
            if (_visited.Contains((schema, table, keyColumn, value)))
            {
                continue;
            }

            _queue.Add((schema, table, keyColumn, value));
            _visited.Add((schema, table, keyColumn, value));
        }
    }

    private IEnumerable<InsertQueryBuildingBlocks> OrderByDependencies(IEnumerable<InsertQueryBuildingBlocks> insertQueryBuildingBlocks)
    {
        var sorted = new List<InsertQueryBuildingBlocks>();
        var unsorted = insertQueryBuildingBlocks.ToList();
        var sortedTableNames = new HashSet<(string Schema, string Table)>();

        var tableRelations = new Dictionary<(string Schema, string Table), HashSet<(string Schema, string Table)>>();
        foreach ((var from, var to) in _foreignKeys)
        {
            if (!tableRelations.TryGetValue((from.Schema, from.Table), out var toTables))
            {
                toTables = new();
                tableRelations.Add((from.Schema, from.Table), toTables);
            }

            toTables.Add((to.Schema, to.Table));
        }

        var anyChange = true;
        while (anyChange)
        {
            anyChange = false;

            for (int idx = 0; idx < unsorted.Count; idx++)
            {
                var current = unsorted[idx];
                var currentTableKey = (current.TableMetadata.Schema, current.TableMetadata.Name);
                if (!tableRelations.TryGetValue(currentTableKey, out var referendedTables))
                {
                    sorted.Add(current);
                    sortedTableNames.Add(currentTableKey);
                    anyChange = true;
                    unsorted.RemoveAt(idx);
                    idx--;
                    continue;
                }

                var shouldSortCurrent = true;
                foreach (var referenced in referendedTables)
                {
                    if (!sortedTableNames.Contains(referenced))
                    {
                        shouldSortCurrent = false;
                        break;
                    }
                }

                if (!shouldSortCurrent)
                {
                    continue;
                }

                sorted.Add(current);
                sortedTableNames.Add(currentTableKey);
                anyChange = true;
                unsorted.RemoveAt(idx);
                idx--;
            }
        }

        if (unsorted.Any())
        {
            sorted.AddRange(unsorted);
            /*
            var unsortedTables = string.Join(", ", unsorted.Select(_ => $"[{_.TableMetadata.Schema}].[{_.TableMetadata.Name}]"));
            throw new InvalidOperationException($"Could not sort all tables. Unsorted tables: {unsortedTables}");
            */
        }

        return sorted;
    }
}
