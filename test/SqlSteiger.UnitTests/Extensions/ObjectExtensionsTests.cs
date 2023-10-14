using FluentAssertions;
using Xunit;

namespace SqlSteiger.UnitTests;

public class ObjectExtensionsTests
{
    [Fact]
    public void ToQueryValue_NullValue()
    {
        // Arrange

        // Act
        var queryValue = ObjectExtensions.ToQueryValue(null);

        // Assert
        queryValue.Should().Be("NULL");
    }

    public static IEnumerable<object?[]> GetTestData() => new object?[][]
    {
        new object?[] { "",           "''"},
        new object?[] { "a string",   "'a string'" },
        new object?[] { true,         "1" },
        new object?[] { false,        "0" },
        new object?[] { (short) 123,  "123" },
        new object?[] { -42,          "-42" },
        new object?[] { 35_004_001L,  "35004001" },
        new object?[] { 0.256F,       "0.256" },
        new object?[] { -1.33D,       "-1.33" },
        new object?[] { 101.99M,      "101.99" },
        new object?[] { new DateTime(2019, 12, 11, 10, 59, 58),
                        "'2019-12-11 10:59:58'" },
        new object?[] { new DateTimeOffset(2021, 01, 02, 10, 30, 29, TimeSpan.FromMinutes(62)),
                        "'2021-01-02 10:30:29 +01:02'" },
        new object?[] { new TimeSpan(1, 2, 3),
                        "'01:02:03'" },
        new object?[] { new Guid("6e9d585d-1011-4d40-9ae5-72e3b687032b"),
                        "'6e9d585d-1011-4d40-9ae5-72e3b687032b'" },
        new object?[] { new Guid("6E9D585D-1011-4D40-9AE5-72E3B687032B"),
                        "'6e9d585d-1011-4d40-9ae5-72e3b687032b'" }
    };

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void ToQueryValue_NonNullValue(object? value, string expected)
    {
        // Arrange

        // Act
        var queryValue = ObjectExtensions.ToQueryValue(value);

        // Assert
        queryValue.Should().Be(expected);
    }
}
