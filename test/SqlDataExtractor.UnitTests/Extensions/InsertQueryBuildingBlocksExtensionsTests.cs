namespace SqlDataExtractor.UnitTests;

using System.Data;
using FluentAssertions;
using Xunit;

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
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks)
            .ToArray();

        // Assert
        query.Length.Should().Be(0);
    }

    [Fact]
    public void CreateInsertQuery_IntAndNullableTextColumns()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("IntColumn", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false),
            new DatabaseColumnMetadata("TextColumn", SqlDbType.Text, typeof(string), IsNullable: true, IsIdentity: false)
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
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks)
            .ToArray();

        // Assert
        query.Length.Should().Be(1);
        query.First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"INSERT INTO dbo.Table ([IntColumn], [TextColumn]) VALUES{NewLine}" +
            $"(1, 'Txt'),{NewLine}" +
            $"(2, NULL);{NewLine}" +
            $"GO{NewLine}"
        );
    }

    [Fact]
    public void CreateInsertQuery_IdentityIntColumn()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true)
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
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks)
            .ToArray();

        // Assert
        query.Length.Should().Be(1);
        query.First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(1),{NewLine}" +
            $"(2);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
    }

    [Fact]
    public void CreateInsertQuery_MaxOneRowPerBatch()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?> {
                { "Column", 1 }
            },
            new Dictionary<string, object?> {
                { "Column", 2 }
            },
            new Dictionary<string, object?> {
                { "Column", 3 }
            }
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks, new InsertQueryConfiguration(MaxRowBatchSize: 1))
            .ToArray();

        // Assert
        query.Length.Should().Be(3);
        query.First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(1);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
        query.Skip(1).First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(2);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
        query.Skip(2).First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(3);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
    }

    [Fact]
    public void CreateInsertQuery_MaxTwoRowsPerBatch()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?> {
                { "Column", 1 }
            },
            new Dictionary<string, object?> {
                { "Column", 2 }
            },
            new Dictionary<string, object?> {
                { "Column", 3 }
            }
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks, new InsertQueryConfiguration(MaxRowBatchSize: 2))
            .ToArray();

        // Assert
        query.Length.Should().Be(2);
        query.First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(1),{NewLine}" +
            $"(2);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
        query.Skip(1).First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(3);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
    }

    [Fact]
    public void CreateInsertQuery_MaxThreeRowsPerBatch()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?> {
                { "Column", 1 }
            },
            new Dictionary<string, object?> {
                { "Column", 2 }
            },
            new Dictionary<string, object?> {
                { "Column", 3 }
            }
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks, new InsertQueryConfiguration(MaxRowBatchSize: 3))
            .ToArray();

        // Assert
        query.Length.Should().Be(1);
        query.First().Should().Be(
            $"-- Table: Table --{NewLine}" +
            $"SET IDENTITY_INSERT Table ON;{NewLine}" +
            $"INSERT INTO dbo.Table ([Column]) VALUES{NewLine}" +
            $"(1),{NewLine}" +
            $"(2),{NewLine}" +
            $"(3);{NewLine}" +
            $"SET IDENTITY_INSERT Table OFF;{NewLine}" +
            $"GO{NewLine}"
        );
    }

    static readonly string NewLine = Environment.NewLine;
}
