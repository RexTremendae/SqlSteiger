using FluentAssertions;
using SqlDataExtractor.UnitTests.TestDataSets;
using Xunit;

namespace SqlDataExtractor.UnitTests;

public class DependencyCrawlerTests
{
    [Theory]
    [InlineData(0, 1, 2)]
    [InlineData(0, 2, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 2, 0)]
    [InlineData(2, 1, 0)]
    [InlineData(2, 0, 1)]
    public async Task InsertQueriesAreOrderedByDependency (int a, int b, int c)
    {
        // Arrange
        var connectionMock = new DbConnectionMock();
        var tables = new (DatabaseTableMetadata tableMetadata, IEnumerable<object?[]> dataRows)[]
        {
            (Presidents.PeopleTable, Presidents.PeopleTableData),
            (Presidents.PrecidencyTable, Presidents.PrecidencyTableData),
            (Presidents.StatesTable, Presidents.StatesTableData)
        };

        connectionMock.AddTable(tables[a].tableMetadata, tables[a].dataRows);
        connectionMock.AddTable(tables[b].tableMetadata, tables[b].dataRows);
        connectionMock.AddTable(tables[c].tableMetadata, tables[c].dataRows);

        connectionMock.AddForeignKey((Presidents.PrecidencyTableName, "PersonId"), (Presidents.PeopleTableName, "Id"));
        connectionMock.AddForeignKey((Presidents.PeopleTableName, "BirthState"),   (Presidents.StatesTableName, "Id"));

        var dependencyCrawler = new DependencyCrawler(connectionMock.ForeignKeyMap, connectionMock.TableMetadataMap);

        // Act
        var insertTables = (await dependencyCrawler.GetInsertQueriesBuildingBlocksAsync
            (connectionMock, Presidents.PrecidencyTableName, "Id", new object[] { 1 }))
            .Select(b => b.tableMetadata.Name)
            .ToArray();

        // Assert
        insertTables.Length.Should().Be(3);
        insertTables[0].Should().Be(Presidents.StatesTableName);
        insertTables[1].Should().Be(Presidents.PeopleTableName);
        insertTables[2].Should().Be(Presidents.PrecidencyTableName);
    }
}
