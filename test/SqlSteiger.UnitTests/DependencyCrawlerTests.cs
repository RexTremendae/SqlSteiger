namespace SqlSteiger.UnitTests;

using FluentAssertions;
using SqlSteiger.UnitTests.Mocks;
using SqlSteiger.UnitTests.TestDataSets;
using Xunit;

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

        connectionMock.AddForeignKey(
            (Presidents.SchemaName, Presidents.PrecidencyTableName, "PersonId"),
            (Presidents.SchemaName, Presidents.PeopleTableName, "Id"));

        connectionMock.AddForeignKey(
            (Presidents.SchemaName, Presidents.PeopleTableName, "BirthState"),
            (Presidents.SchemaName, Presidents.StatesTableName, "Id"));

        var dependencyCrawler = new DependencyCrawler(connectionMock.ForeignKeyMap, connectionMock.TableMetadataMap);

        // Act
        var insertTables = (await dependencyCrawler.GetInsertQueriesBuildingBlocksAsync(
            connection: connectionMock,
            startingSchema: Presidents.SchemaName,
            startingTable: Presidents.PrecidencyTableName,
            keyColumn: "Id",
            keyColumnValues: new object[] { 1 }))
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

        connectionMock.AddForeignKey(
            (RockBands.SchemaName, RockBands.AlbumsTableName, "BandId"),
            (RockBands.SchemaName, RockBands.BandsTableName, "Id"));

        connectionMock.AddForeignKey(
            (RockBands.SchemaName, RockBands.BandsMembersTableName, "BandId"),
            (RockBands.SchemaName, RockBands.BandsTableName, "Id"));

        connectionMock.AddForeignKey(
            (RockBands.SchemaName, RockBands.BandsMembersTableName, "PersonId"),
            (RockBands.SchemaName, RockBands.PeopleTableName, "Id"));

        var dependencyCrawler = new DependencyCrawler(connectionMock.ForeignKeyMap, connectionMock.TableMetadataMap);

        // Act
        var insertTables = (await dependencyCrawler.GetInsertQueriesBuildingBlocksAsync(
            connection: connectionMock,
            startingSchema: RockBands.SchemaName,
            startingTable: RockBands.BandsTableName,
            keyColumn: "Id",
            keyColumnValues: new object[] { 1 }))
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
