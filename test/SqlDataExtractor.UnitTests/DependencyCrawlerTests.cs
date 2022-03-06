using SqlDataExtractor.UnitTests.Mocks;
using FluentAssertions;
using SqlDataExtractor.UnitTests.TestDataSets;
using Xunit;

namespace SqlDataExtractor.UnitTests;

public class DependencyCrawlerTests
{
    [Fact]
    public async Task InsertQueries_OrderedByDependency_PresidentsDataSet()
    {
        // Arrange
        var connectionMock = new DbConnectionMock();

        connectionMock.AddTable(Presidents.PeopleTable,     Presidents.PeopleTableData);
        connectionMock.AddTable(Presidents.PrecidencyTable, Presidents.PrecidencyTableData);
        connectionMock.AddTable(Presidents.StatesTable,     Presidents.StatesTableData);

        connectionMock.AddForeignKey((Presidents.PrecidencyTableName, "PersonId"), (Presidents.PeopleTableName, "Id"));
        connectionMock.AddForeignKey((Presidents.PeopleTableName, "BirthState"),   (Presidents.StatesTableName, "Id"));

        var dependencyCrawler = new DependencyCrawler(connectionMock.ForeignKeyMap, connectionMock.TableMetadataMap);

        // Act
        var insertTables = (await dependencyCrawler.GetInsertQueriesBuildingBlocksAsync
            (connectionMock, Presidents.PrecidencyTableName, "Id", new object[] { 1 }))
            .Select(b => b.TableMetadata.Name)
            .ToArray();

        // Assert
        insertTables.Length.Should().Be(3);
        insertTables[0].Should().Be(Presidents.StatesTableName);
        insertTables[1].Should().Be(Presidents.PeopleTableName);
        insertTables[2].Should().Be(Presidents.PrecidencyTableName);
    }

    [Fact]
    public async Task InsertQueries_OrderedByDependency_RockBandsDataSet()
    {
        // Arrange
        var connectionMock = new DbConnectionMock();

        connectionMock.AddTable(RockBands.PeopleTable,       RockBands.PeopleTableData);
        connectionMock.AddTable(RockBands.BandsMembersTable, RockBands.BandsMembersTableData);
        connectionMock.AddTable(RockBands.AlbumsTable,       RockBands.AlbumsTableData);
        connectionMock.AddTable(RockBands.BandsTable,        RockBands.BandsTableData);

        connectionMock.AddForeignKey((RockBands.AlbumsTableName, "BandId"),         (RockBands.BandsTableName, "Id"));
        connectionMock.AddForeignKey((RockBands.BandsMembersTableName, "BandId"),   (RockBands.BandsTableName, "Id"));
        connectionMock.AddForeignKey((RockBands.BandsMembersTableName, "PersonId"), (RockBands.PeopleTableName, "Id"));

        var dependencyCrawler = new DependencyCrawler(connectionMock.ForeignKeyMap, connectionMock.TableMetadataMap);

        // Act
        var insertTables = (await dependencyCrawler.GetInsertQueriesBuildingBlocksAsync
            (connectionMock, RockBands.BandsTableName, "Id", new object[] { 1 }))
            .Select(b => b.TableMetadata.Name)
            .ToArray();

        // Assert
        insertTables.Length.Should().Be(4);
        insertTables[0].Should().Be(RockBands.BandsTableName);
        insertTables[1].Should().Be(RockBands.AlbumsTableName);
        insertTables[2].Should().Be(RockBands.PeopleTableName);
        insertTables[3].Should().Be(RockBands.BandsMembersTableName);
    }
}
