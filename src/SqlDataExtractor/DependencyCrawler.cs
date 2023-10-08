namespace SqlDX;

using System.Data;
using ForeignKeyMap = Dictionary<(string schema, string table, string column), (string schema, string table, string column)>;
using TableMetadataMap = Dictionary<(string schema, string name), DatabaseTableMetadata>;

public class DependencyCrawler
{
    private readonly TableMetadataMap _tables;
    private readonly ForeignKeyMap _foreignKeys;
    private readonly List<(string schema, string table, string keyColumn, object keyColumnValue)> _queue;
    private readonly HashSet<(string schema, string table, string keyColumn, object keyColumnValue)> _visited;

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
            if (_queue.Count > 120) break;

            var dequeued = _queue.First();
            _queue.RemoveAt(0);

            _visited.Add((dequeued.schema, dequeued.table, dequeued.keyColumn, dequeued.keyColumnValue));

            var selectQuery = _tables[(dequeued.schema, dequeued.table)].CreateSelectQuery(
                keyColumn: dequeued.keyColumn,
                keyColumnFilter: new[] { dequeued.keyColumnValue });

            var data = new List<Dictionary<string, object?>>();

            await using var reader = await connection.ExecuteReaderAsync(selectQuery);

            while (await reader.ReadAsync())
            {
                var dataRow = new Dictionary<string, object?>();
                data.Add(dataRow);

                foreach (var col in _tables[(dequeued.schema, dequeued.table)].Columns)
                {
                    dataRow.Add(col.Name, reader.GetValue(col));
                }
            }

            insertQueryTableData.Add(new InsertQueryBuildingBlocks(_tables[(dequeued.schema, dequeued.table)], data));

            // Add child dependencies
            foreach (var (from, to) in _foreignKeys.Where(fk =>
                fk.Key.table == dequeued.table &&
                fk.Key.schema == dequeued.schema))
            {
                keyColumnValues = data.Select(d => d[from.column]).Cast<object>().ToArray();
                Enqueue(to.schema, to.table, to.column, keyColumnValues);
            }

            // Add parent dependencies
            foreach (var (from, to) in _foreignKeys.Where(fk =>
                fk.Value.table == dequeued.table &&
                fk.Value.schema == dequeued.schema))
            {
                keyColumnValues = data.Select(d => d[to.column]).Cast<object>().ToArray();
                Enqueue(from.schema, from.table, from.column, keyColumnValues);
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
        var sortedTableNames = new HashSet<(string schema, string table)>();

        var tableRelations = new Dictionary<(string schema, string table), HashSet<(string schema, string table)>>();
        foreach ((var from, var to) in _foreignKeys)
        {
            if (!tableRelations.TryGetValue((from.schema, from.table), out var toTables))
            {
                toTables = new();
                tableRelations.Add((from.schema, from.table), toTables);
            }

            toTables.Add((to.schema, to.table));
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

                if (!shouldSortCurrent) continue;
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
            // var unsortedTables = string.Join(", ", unsorted.Select(_ => $"[{_.TableMetadata.Schema}].[{_.TableMetadata.Name}]"));
            // throw new InvalidOperationException($"Could not sort all tables. Unsorted tables: {unsortedTables}");
        }

        return sorted;
    }
}
