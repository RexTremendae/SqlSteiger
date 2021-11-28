using FluentAssertions;
using Xunit;

namespace SqlDataExtractor.UnitTests;

public class ObjectExtensionsTests
{
    [Theory]
    [InlineData(typeof(string),  null, "NULL")]
    [InlineData(typeof(int),     null, "NULL")]
    [InlineData(typeof(float),   null, "NULL")]
    [InlineData(typeof(double),  null, "NULL")]
    [InlineData(typeof(decimal), null, "NULL")]
    [InlineData(typeof(bool),    null, "NULL")]
    public void ToQueryValue_NullValue(Type type, object? value, string expected)
    {
        // Arrange

        // Act
        var queryValue = ObjectExtensions.ToQueryValue(value, type);

        // Assert
        queryValue.Should().Be(expected);
    }

    [Theory]
    [InlineData(typeof(string), "",         "''")]
    [InlineData(typeof(string), "a string", "'a string'")]
    [InlineData(typeof(bool),   true,       "1")]
    [InlineData(typeof(bool),   false,      "0")]
    public void ToQueryValue_NonNullValue(Type type, object? value, string expected)
    {
        // Arrange

        // Act
        var queryValue = ObjectExtensions.ToQueryValue(value, type);

        // Assert
        queryValue.Should().Be(expected);
    }
}
