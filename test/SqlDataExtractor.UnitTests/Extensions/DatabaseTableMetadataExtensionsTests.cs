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
            new DatabaseColumnMetadata("IntColumn", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("TextColumn", SqlDbType.Text, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);

        // Act
        var query = DatabaseTableMetadataExtensions.CreateSelectQuery(databaseTableMetadata);

        // Assert
        query.Should().Be(
            $"SELECT [IntColumn], [TextColumn]{LF}" +
            $"FROM dbo.Table;{LF}"
        );
    }

    [Fact]
    public void CreateSelectQuery_IntAndNullableTextColumns_Filtered()
    {
        // Arrange
        var columnMetadata = new[]
        {
            new DatabaseColumnMetadata("IntColumn", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("TextColumn", SqlDbType.Text, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false)
        };

        var databaseTableMetadata = new DatabaseTableMetadata("Table", columnMetadata);
        var keyColumn = "IntColumn";
        var keyColumnFilter = new object[] { 6, 9 };

        // Act
        var query = DatabaseTableMetadataExtensions.CreateSelectQuery(databaseTableMetadata, keyColumn, keyColumnFilter);

        // Assert
        query.Should().Be(
            $"SELECT [IntColumn], [TextColumn]{LF}" +
            $"FROM dbo.Table{LF}" +
            $"WHERE IntColumn IN (6, 9);{LF}"
        );
    }

    static readonly string LF = Environment.NewLine;
}
