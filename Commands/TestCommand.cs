using System.CommandLine;
using Newtonsoft.Json;
using xmle.Models;
using xmle.Services;
using xmle.Utils;

namespace xmle.Commands;

public class TestCommand : Command
{
    private readonly IConfigService config;
    private readonly ICsvParser csvParser;
    private readonly TextWriter writer;
    private readonly INewXmlService xmlService;

    public Argument<string> XmlPathArgument { get; set; } = new("xml-path") { };

    public Option<string> RootPathOption { get; set; } = new("root-path", "-r", "--root")
    {
        Required = true
    };
    public Option<string> RowPathOption { get; set; } = new("row-path", "-n", "--node")
    {
        Required = true
    };
    public Option<string[]> ColumnPathsOption { get; set; } = new("column-paths", "-c", "--col")
    {
        Required = true
    };
    public Option<string[]> TitlesOption { get; set; } = new("titles", "-t", "--title")
    {
    };

    public TestCommand(IConfigService config, ICsvParser csvParser, TextWriter writer, INewXmlService xmlService) : base("test", "Temporary Command")
    {
        this.config = config;
        this.csvParser = csvParser;
        this.writer = writer;
        this.xmlService = xmlService;

        Add(XmlPathArgument);
        Add(RootPathOption);
        Add(RowPathOption);
        Add(ColumnPathsOption);
        Add(TitlesOption);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var parsedValues = GetParsedValues(result);

        Console.WriteLine(xmlService.ViewXmlAsJson(parsedValues));
    }

    private ViewXmlAsJsonDto GetParsedValues(ParseResult result)
    {
        string xmlPath = result.GetRequiredValue(XmlPathArgument);
        string rootPath = result.GetRequiredValue(RootPathOption);
        string rowPath = result.GetRequiredValue(RowPathOption);
        string[] columnPaths = result.GetRequiredValue(ColumnPathsOption);
        string[]? titles = result.GetValue(TitlesOption);
        return new ViewXmlAsJsonDto(xmlPath, rootPath, rowPath, columnPaths, titles);
    }
}