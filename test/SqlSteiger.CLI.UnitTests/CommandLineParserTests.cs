namespace SqlSteiger.CLI.UnitTests;

using SqlSteiger.CLI.CommandLine;

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
            var result = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            result.Options.Switch
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
            [ "-s1", ExpectedValue ],
            [ $"-s1={ExpectedValue}" ],
            [ "-s2", ExpectedValue ],
            [ $"-s2={ExpectedValue}" ]
        })
        {
            // Act
            var result = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            result.Options.NamedStringOption
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
            var result = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            result.Options.NoNameStringOption
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
            var result = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            result.Options.IntOption
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
            var result = CommandLineParser.Parse<TestOptions>(args);

            // Assert
            result.Options.LongOption
                .Should().Be(expected: ExpectedValue, because: "['" + string.Join("', '", args) + "']");
        }
    }

    [Fact]
    public void MixedOptionsTest()
    {
        // Arrange
        var args = new[] { "--switch", "-s1", "string value", "--int-option=13" };

        // Act
        var result = CommandLineParser.Parse<TestOptions>(args);

        // Assert
        result.Options.Switch
            .Should().BeTrue();

        result.Options.NamedStringOption
            .Should().Be("string value");

        result.Options.NoNameStringOption
            .Should().BeEmpty();

        result.Options.IntOption
            .Should().Be(13);

        result.Options.LongOption
            .Should().Be(0);
    }

    [Theory]
    [InlineData("-s1 -w")]
    [InlineData("--string-option-with-name")]
    [InlineData("-w true")]
    [InlineData("--string-option-with-name s1 s2")]
    [InlineData("-s1 -w --int-option")]
    [InlineData("--undefined-option")]
    public void InvalidOptionsTest(string args)
    {
        // Act
        var result = CommandLineParser.Parse<TestOptions>(args.Split(' '));

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeEmpty();
    }
}

public class TestOptions
{
    [CommandLineOption(shortName: "w")]
    public bool Switch { get; set; }

    [CommandLineOption(longName: "string-option-with-name", shortNames: ["s1", "s2"])]
    public string NamedStringOption { get; set; } = string.Empty;

    [CommandLineOption()]
    public string NoNameStringOption { get; set; } = string.Empty;

    [CommandLineOption()]
    public int IntOption { get; set; }

    [CommandLineOption()]
    public long LongOption { get; set; }
}
