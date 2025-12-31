using System.CommandLine;

using xmle.Models;
using xmle.Services;

public class ViewXmlAsJsonCommand : Command
{
    private readonly IConfigService config;
    private readonly TextWriter writer;
    private readonly IXmlService xmlService;

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

    public ViewXmlAsJsonCommand(IConfigService config, TextWriter writer, IXmlService xmlService) : base("view-xml", "View Xml as Json")
    {
        this.config = config;
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

        writer.WriteLine(xmlService.ViewXmlAsJson(parsedValues));
    }

    private ViewXmlAsJsonRequest GetParsedValues(ParseResult result)
    {
        string xmlPath = result.GetRequiredValue(XmlPathArgument);
        string rootPath = result.GetRequiredValue(RootPathOption);
        string rowPath = result.GetRequiredValue(RowPathOption);
        string[] columnPaths = result.GetRequiredValue(ColumnPathsOption);
        string[]? titles = result.GetValue(TitlesOption);
        return new ViewXmlAsJsonRequest(xmlPath, rootPath, rowPath, columnPaths, titles);
    }
}