namespace SqlSteiger.Console.UnitTests;

using SqlSteiger.Console.CommandLine;

public class CommandLineParserTests
{
    [Fact]
    public void SwitchOptionTests()
    {
        foreach (var args in new string[][] {
            [ "--switch" ],
            [ "-w" ],
        })
        {
            // Act
            var options = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            options.Switch
                .Should().Be(true, because: "['" + string.Join("', '", args) + "']");
        }
    }

    [Fact]
    public void ExplicitlyNamedStringOptionTests()
    {
        const string ExpectedValue = "a string";

        foreach (var args in new string[][] {
            [ "--string-option-with-name", ExpectedValue ],
            [ $"--string-option-with-name={ExpectedValue}" ],
            [ "-s", ExpectedValue ],
            [ $"-s={ExpectedValue}" ]
        })
        {
            // Act
            var options = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            options.NamedStringOption
                .Should().Be(expected: ExpectedValue, because: "['" + string.Join("', '", args) + "']");
        }
    }

    [Fact]
    public void DefaultNamedStringOptionTests()
    {
        const string ExpectedValue = "a string";

        foreach (var args in new string[][] {
            [ "--no-name-string-option", ExpectedValue ],
            [ $"--no-name-string-option={ExpectedValue}" ],
        })
        {
            // Act
            var options = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            options.NoNameStringOption
                .Should().Be(expected: ExpectedValue, because: "['" + string.Join("', '", args) + "']");
        }
    }

    [Fact]
    public void IntOptionTests()
    {
        const int ExpectedValue = 31;

        foreach (var args in new string[][] {
            [ "--int-option", $"{ExpectedValue}" ],
            [ $"--int-option={ExpectedValue}" ],
        })
        {
            // Act
            var options = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            options.IntOption
                .Should().Be(expected: ExpectedValue, because: "['" + string.Join("', '", args) + "']");
        }
    }

    [Fact]
    public void LongOptionTests()
    {
        const long ExpectedValue = 47L;

        foreach (var args in new string[][] {
            [ "--long-option", $"{ExpectedValue}" ],
            [ $"--long-option={ExpectedValue}" ],
        })
        {
            // Act
            var options = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            options.LongOption
                .Should().Be(expected: ExpectedValue, because: "['" + string.Join("', '", args) + "']");
        }
    }

    [Fact]
    public void MixedOptionsTest()
    {
        // Arrange
        var args = new[] { "--switch", "-s", "string value", "--int-option=13" };

        // Act
        var options = CommandLineParser.Parse<TestOptions>(args);

        // Assert
        options.Switch
            .Should().BeTrue();

        options.NamedStringOption
            .Should().Be("string value");

        options.NoNameStringOption
            .Should().BeEmpty();

        options.IntOption
            .Should().Be(13);

        options.LongOption
            .Should().Be(0);
    }

    [Theory]
    [InlineData("-s -w")]
    [InlineData("--string-option-with-name")]
    [InlineData("-w true")]
    [InlineData("--string-option-with-name s1 s2")]
    [InlineData("-s -w --int-option")]
    [InlineData("--undefined-option")]
    public void InvalidOptionsTest(string args)
    {
        // Act
        var action = () => CommandLineParser.Parse<TestOptions>(args.Split(' '));

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }
}

public class TestOptions
{
    [CommandLineOption(shortName: "w")]
    public bool Switch { get; set; }

    [CommandLineOption(longName: "string-option-with-name", shortName: "s")]
    public string NamedStringOption { get; set; } = string.Empty;

    [CommandLineOption()]
    public string NoNameStringOption { get; set; } = string.Empty;

    [CommandLineOption()]
    public int IntOption { get; set; }

    [CommandLineOption()]
    public long LongOption { get; set; }
}
