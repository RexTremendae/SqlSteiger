namespace SqlSteiger.CLI.UnitTests.CommandLine;

using SqlSteiger.CLI.CommandLine;

public class CommandLineInfoTests
{
    [Fact]
    public void PrintUsageShouldNotThrow()
    {
        // Act
        var action = CommandLineInfo.PrintUsage<CommandLineOptions>;

        // Assert
        action.Should().NotThrow();
    }
}
