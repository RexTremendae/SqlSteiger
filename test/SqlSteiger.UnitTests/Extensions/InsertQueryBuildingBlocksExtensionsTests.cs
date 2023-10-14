namespace SqlSteiger.UnitTests;

using System.Data;
using FluentAssertions;
using Xunit;

public class InsertQueryBuildingBlocksExtensionsTests
{
    private static readonly string LF = Environment.NewLine;

    [Fact]
    public void CreateInsertQuery_NoData()
    {
        // Arrange
        var databaseTableMetadata = new DatabaseTableMetadata("dbo", "Table", Array.Empty<DatabaseColumnMetadata>());
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
            new DatabaseColumnMetadata("IntColumn", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("TextColumn", SqlDbType.Text, typeof(string), IsNullable: true, IsIdentity: false, IsPrimaryKeyPart: false),
        };

        var databaseTableMetadata = new DatabaseTableMetadata("dbo", "Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?>
            {
                { "IntColumn", 1 },
                { "TextColumn", "Txt" },
            },
            new Dictionary<string, object?>
            {
                { "IntColumn", 2 },
                { "TextColumn", null },
            },
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks)
            .ToArray();

        // Assert
        query.Length.Should().Be(1);
        query.First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"INSERT INTO [dbo].[Table] ([IntColumn], [TextColumn]) VALUES{LF}" +
            $"(1, 'Txt'),{LF}" +
            $"(2, NULL);{LF}" +
            $"GO{LF}");
    }

    [Fact]
    public void CreateInsertQuery_IdentityIntColumn()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true, IsPrimaryKeyPart: false),
        };

        var databaseTableMetadata = new DatabaseTableMetadata("dbo", "Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?>
            {
                { "Column", 1 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 2 },
            },
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks)
            .ToArray();

        // Assert
        query.Length.Should().Be(1);
        query.First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(1),{LF}" +
            $"(2);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
    }

    [Fact]
    public void CreateInsertQuery_MaxOneRowPerBatch()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true, IsPrimaryKeyPart: false),
        };

        var databaseTableMetadata = new DatabaseTableMetadata("dbo", "Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?>
            {
                { "Column", 1 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 2 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 3 },
            },
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks, new InsertQueryConfiguration(MaxRowBatchSize: 1))
            .ToArray();

        // Assert
        query.Length.Should().Be(3);
        query.First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(1);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
        query.Skip(1).First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(2);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
        query.Skip(2).First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(3);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
    }

    [Fact]
    public void CreateInsertQuery_MaxTwoRowsPerBatch()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true, IsPrimaryKeyPart: false),
        };

        var databaseTableMetadata = new DatabaseTableMetadata("dbo", "Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?>
            {
                { "Column", 1 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 2 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 3 },
            },
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks, new InsertQueryConfiguration(MaxRowBatchSize: 2))
            .ToArray();

        // Assert
        query.Length.Should().Be(2);
        query.First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(1),{LF}" +
            $"(2);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
        query.Skip(1).First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(3);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
    }

    [Fact]
    public void CreateInsertQuery_MaxThreeRowsPerBatch()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("Column", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true, IsPrimaryKeyPart: false),
        };

        var databaseTableMetadata = new DatabaseTableMetadata("dbo", "Table", columnMetadata);
        var dataRows = new Dictionary<string, object?>[]
        {
            new Dictionary<string, object?>
            {
                { "Column", 1 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 2 },
            },
            new Dictionary<string, object?>
            {
                { "Column", 3 },
            },
        };
        var buildingBlocks = new InsertQueryBuildingBlocks(databaseTableMetadata, dataRows);

        // Act
        var query = InsertQueryBuildingBlocksExtensions
            .CreateInsertQuery(buildingBlocks, new InsertQueryConfiguration(MaxRowBatchSize: 3))
            .ToArray();

        // Assert
        query.Length.Should().Be(1);
        query.First().Should().Be(
            $"-- Table: Table --{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] ON;{LF}" +
            $"INSERT INTO [dbo].[Table] ([Column]) VALUES{LF}" +
            $"(1),{LF}" +
            $"(2),{LF}" +
            $"(3);{LF}" +
            $"SET IDENTITY_INSERT [dbo].[Table] OFF;{LF}" +
            $"GO{LF}");
    }
}
