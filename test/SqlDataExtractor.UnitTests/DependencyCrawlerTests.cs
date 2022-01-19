using SqlDataExtractor.UnitTests.Mocks;
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
    public async Task InsertQueries_OrderedByDependency_PresidentsDataSet (int a, int b, int c)
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

    [Theory]
    [InlineData(0, 1, 2, 3)]
    [InlineData(0, 2, 1, 3)]
    [InlineData(0, 3, 2, 1)]
    [InlineData(1, 0, 2, 3)]
    [InlineData(1, 2, 0, 3)]
    [InlineData(2, 1, 0, 3)]
    [InlineData(2, 0, 1, 3)]
    [InlineData(3, 0, 1, 2)]
    [InlineData(3, 1, 0, 2)]
    [InlineData(3, 2, 1, 0)]
    public async Task InsertQueries_OrderedByDependency_RockBandsDataSet (int a, int b, int c, int d)
    {
        // Arrange
        var connectionMock = new DbConnectionMock();
        var tables = new (DatabaseTableMetadata tableMetadata, IEnumerable<object?[]> dataRows)[]
        {
            (RockBands.PeopleTable,       RockBands.PeopleTableData),
            (RockBands.BandsMembersTable, RockBands.BandsMembersTableData),
            (RockBands.AlbumsTable,       RockBands.AlbumsTableData),
            (RockBands.BandsTable,        RockBands.BandsTableData)
        };

        connectionMock.AddTable(tables[a].tableMetadata, tables[a].dataRows);
        connectionMock.AddTable(tables[b].tableMetadata, tables[b].dataRows);
        connectionMock.AddTable(tables[c].tableMetadata, tables[c].dataRows);
        connectionMock.AddTable(tables[d].tableMetadata, tables[d].dataRows);

        connectionMock.AddForeignKey((RockBands.AlbumsTableName, "BandId"),         (RockBands.BandsTableName, "Id"));
        connectionMock.AddForeignKey((RockBands.BandsMembersTableName, "BandId"),   (RockBands.BandsTableName, "Id"));
        connectionMock.AddForeignKey((RockBands.BandsMembersTableName, "PersonId"), (RockBands.PeopleTableName, "Id"));

        var dependencyCrawler = new DependencyCrawler(connectionMock.ForeignKeyMap, connectionMock.TableMetadataMap);

        // Act
        var insertTables = (await dependencyCrawler.GetInsertQueriesBuildingBlocksAsync
            (connectionMock, RockBands.BandsTableName, "Id", new object[] { 1 }))
            .Select(b => b.tableMetadata.Name)
            .ToArray();

        // Assert
        insertTables.Length.Should().Be(4);
        insertTables[0].Should().Be(RockBands.PeopleTableName);
        insertTables[1].Should().Be(RockBands.BandsTableName);
        insertTables[2].Should().Be(RockBands.BandsMembersTableName);
        insertTables[3].Should().Be(RockBands.AlbumsTableName);
    }
}
