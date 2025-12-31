using System.CommandLine;

using xmle.Models;
using xmle.Services;

namespace xmle.Commands;

public class XmlToCsvCommand : Command
{
    private readonly IXmlToCsvService service;
    private readonly Argument<string> xmlPathArgument = new("xml-path") { };
    private readonly Argument<string> csvPathArgument = new("csv-path") { };

    private readonly Option<string> rootPathOption = new("root-path", "-r", "--root") { Required = true };
    private readonly Option<string> rowPathOption = new("row-path", "-n", "--node") { Required = true };
    private readonly Option<string[]> columnPathsOption = new("col-paths", "-c", "--col") { Required = true };
    private readonly Option<string[]> titlesOption = new("titles", "-t", "--title") { Required = false };

    public XmlToCsvCommand(IXmlToCsvService service) : base("xml-csv", "Convert xml to csv")
    {
        Add(xmlPathArgument);
        Add(csvPathArgument);

        Add(rootPathOption);
        Add(rowPathOption);
        Add(columnPathsOption);
        Add(titlesOption);

        SetAction(ActionHandler);
        this.service = service;
    }

    private void ActionHandler(ParseResult result)
    {
        var input = GetParsedValues(result);
        service.CovertXmlToCsv(input);
    }

    private XmlToCsvRequest GetParsedValues(ParseResult res)
    {
        var xmlFilePath = res.GetRequiredValue(xmlPathArgument);
        var csvFilePath = res.GetRequiredValue(csvPathArgument);
        var rootPath = res.GetRequiredValue(rootPathOption);
        var rowPath = res.GetRequiredValue(rowPathOption);
        var columnPaths = res.GetRequiredValue(columnPathsOption);
        var titles = res.GetRequiredValue(titlesOption);

        return new XmlToCsvRequest(xmlFilePath, csvFilePath, rootPath, rowPath, columnPaths, titles);
    }
}