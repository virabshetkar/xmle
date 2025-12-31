using System.CommandLine;

using xmle.Models;
using xmle.Services;

public class ViewXmlAsJsonCommand : Command
{
    private readonly TextWriter writer;
    private readonly IXmlService xmlService;

    private Argument<string> xmlPathArgument = new("xml-path") { };

    private Option<string> rootPathOption;
    private Option<string> rowPathOption;
    private Option<string[]> columnPathsOption;
    private Option<string[]> titlesOption;

    public ViewXmlAsJsonCommand(IConfigService config, TextWriter writer, IXmlService xmlService) : base("view-xml", "View Xml as Json")
    {
        this.writer = writer;
        this.xmlService = xmlService;

        rootPathOption = new("root-path", "-r", "--root")
        {
            Required = true,
            DefaultValueFactory = (res) => { return config.GetConfig()?.RootPath ?? ""; }
        };

        rowPathOption = new("row-path", "-n", "--node")
        {
            Required = true,
            DefaultValueFactory = res => config.GetConfig()?.RowPath ?? ""
        };

        columnPathsOption = new("column-paths", "-c", "--col")
        {
            Required = true,
            DefaultValueFactory = res => config.GetConfig()?.ColumnPaths ?? []
        };

        titlesOption = new("titles", "-t", "--title")
        {
            DefaultValueFactory = res => config.GetConfig()?.Titles ?? []
        };

        Add(xmlPathArgument);
        Add(rootPathOption);
        Add(rowPathOption);
        Add(columnPathsOption);
        Add(titlesOption);

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        var parsedValues = GetParsedValues(result);

        writer.WriteLine(xmlService.ViewXmlAsJson(parsedValues));
    }

    private ViewXmlAsJsonRequest GetParsedValues(ParseResult result)
    {
        string xmlPath = result.GetRequiredValue(xmlPathArgument);
        string rootPath = result.GetRequiredValue(rootPathOption);
        string rowPath = result.GetRequiredValue(rowPathOption);
        string[] columnPaths = result.GetRequiredValue(columnPathsOption);
        string[]? titles = result.GetValue(titlesOption);
        return new ViewXmlAsJsonRequest(xmlPath, rootPath, rowPath, columnPaths, titles);
    }
}