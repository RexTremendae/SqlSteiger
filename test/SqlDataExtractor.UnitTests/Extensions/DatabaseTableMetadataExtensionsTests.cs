namespace SqlDataExtractor.UnitTests.Extensions;

using System.Data;
using FluentAssertions;
using Xunit;

public class DatabaseTableMetadataExtensionsTests
{
    [Fact]
    public void CreateSelectQuery_IntAndNullableTextColumns_NoFilter()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("IntColumn", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false),
            new DatabaseColumnMetadata("TextColumn", SqlDbType.Text, typeof(string), IsNullable: false, IsIdentity: false)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);

        // Act
        var query = DatabaseTableMetadataExtensions.CreateSelectQuery(databaseTableMetadata);

        // Assert
        query.Should().Be(
            $"SELECT [IntColumn], [TextColumn]{NewLine}" +
            $"FROM dbo.Table;{NewLine}"
        );
    }

    [Fact]
    public void CreateSelectQuery_IntAndNullableTextColumns_Filtered()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("IntColumn", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false),
            new DatabaseColumnMetadata("TextColumn", SqlDbType.Text, typeof(string), IsNullable: false, IsIdentity: false)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var keyColumn = "IntColumn";
        var keyColumnFilter = new object[] { 6, 9 };

        // Act
        var query = DatabaseTableMetadataExtensions.CreateSelectQuery(databaseTableMetadata, keyColumn, keyColumnFilter);

        // Assert
        query.Should().Be(
            $"SELECT [IntColumn], [TextColumn]{NewLine}" +
            $"FROM dbo.Table{NewLine}" +
            $"WHERE IntColumn IN (6, 9);{NewLine}"
        );
    }

    static readonly string NewLine = Environment.NewLine;
}
