using FluentAssertions;
using Xunit;

namespace SqlDataExtractor.UnitTests;

public class InsertQueryBuildingBlocksExtensionsTests
{
    [Fact]
    public void CreateInsertQuery_NoData()
    {
        // Arrange
        var databaseTableMetadata = new DatabaseTableMetadata("Table", Array.Empty<DatabaseColumnMetadata>());
        var dataRows = Array.Empty<Dictionary<string, object?>>();
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions.CreateInsertQuery(buildingBlocks);

        // Assert
        query.Should().Be(NewLine);
    }

    [Fact]
    public void CreateInsertQuery_IntAndNullableTextColumns()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("IntColumn", "int", typeof(int), IsNullable: false, IsIdentity: false),
            new DatabaseColumnMetadata("TextColumn", "text", typeof(string), IsNullable: true, IsIdentity: false)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?> {
                { "IntColumn", 1 },
                { "TextColumn", "Txt" }
            },
            new Dictionary<string, object?> {
                { "IntColumn", 2 },
                { "TextColumn", null }
            }
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions.CreateInsertQuery(buildingBlocks);

        // Assert
        query.Should().Be(
            $"INSERT INTO dbo.Table ([IntColumn], [TextColumn]) VALUES{NewLine}" +
            $"(1, 'Txt'),{NewLine}" +
            $"(2, NULL);{NewLine}"
        );
    }

    [Fact]
    public void CreateInsertQuery_IdentityIntColumn()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", "int", typeof(int), IsNullable: false, IsIdentity: true)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?> {
                { "Column", 1 }
            },
            new Dictionary<string, object?> {
                { "Column", 2 }
            }
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions.CreateInsertQuery(buildingBlocks);

        // Assert
        query.Should().Be(
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(1),{NewLine}" +
            $"(2);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}"
        );
    }

    static readonly string NewLine = Environment.NewLine;
}
