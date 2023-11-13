using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System;
using System.Windows.Input;
using Moq;
using NUnit.Framework;
using Xunit;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<App>().Run(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IWordReversalService, WordReversalService>();
            services.AddSingleton<App>();
        });
}
sealed class WordReversalService : IWordReversalService
{
    public string ReverseWords(string line)
    {
        string[] words = line.Split(' ');
        Array.Reverse(words);
        return string.Join(' ', words);
    }
}
public interface IWordReversalService
{
     string ReverseWords(string line);
}

public class App
{
    private readonly IWordReversalService _wordReversalService;
  
    public App(IWordReversalService wordReversalService)
    {
        _wordReversalService = wordReversalService;
    }
    public void Run(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Please enter input and output files");
            return;
        }

        ProcessReversalWords(args[1], args[0]);
       
    }
    void ProcessReversalWords(string inputFileName, string outputFileName)
    {
        try
        {
            using (var writer = new StreamWriter(inputFileName))
            {
                using (var reader = new StreamReader(outputFileName))
                {
                    Console.SetOut(writer);

                    Console.SetIn(reader);

                    int casesNumber = int.Parse(Console.ReadLine());

                    if (casesNumber <= 25 && casesNumber >= 1)
                    {

                        for (int caseNumber = 1; caseNumber <= casesNumber; caseNumber++)
                        {
                            string inputLine = reader.ReadLine();
                            string reversedLine = _wordReversalService.ReverseWords(inputLine);
                            writer.WriteLine($"Case{caseNumber} {reversedLine}");
                        }
                    }
                    else
                    {
                        writer.WriteLine($"Invalid Case number");
                    }
                }
            }
        }
        catch (IOException e)
        {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine(e.Message);
            return;
        }
    }
}

public class WordReversalServiceTests
{
    private readonly Mock<IWordReversalService> _mockWordReversalService;

    public WordReversalServiceTests()
    {
        _mockWordReversalService = new Mock<IWordReversalService>();
    }

    [Fact]
    public void ReverseWords_ReversesTheOrderOfWordsInTheLine()
    {
        string input = "My name is Aluwani Nethavhakone";
        string expectedOutput = "Nethavhakone Aluwani is name My";
        // Arrange
        _mockWordReversalService.Setup(x => x.ReverseWords(input))
            .Returns(expectedOutput);

        // Act
        var actualReversedSentence = _mockWordReversalService.Object.ReverseWords(input);

        // Assert
        Xunit.Assert.Equal(expectedOutput, actualReversedSentence);
    }

    [Fact]
    public void ReverseWords_HandlesEmptyLines()
    {
        // Arrange
        _mockWordReversalService.Setup(x => x.ReverseWords(string.Empty))
            .Returns(string.Empty);

        // Act
        var actualReversedSentence = _mockWordReversalService.Object.ReverseWords(string.Empty);

        // Assert
        Xunit.Assert.Equal(string.Empty, actualReversedSentence);
    }

    [Fact]
    public void ReverseWords_HandlesLinesWithOnlyOneWord()
    {
        string input = "Nethavhakone";
        string expectedOutput = "Nethavhakone";
        // Arrange
        _mockWordReversalService.Setup(x => x.ReverseWords(input))
            .Returns(expectedOutput);

        // Act
        var actualReversedSentence = _mockWordReversalService.Object.ReverseWords(input);

        // Assert
        Xunit.Assert.Equal(expectedOutput, actualReversedSentence);
    }
}


